using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

[BurstCompile]
public unsafe class BatchRendererGroupSetup : MonoBehaviour
{
    [Range(1, 100)] public int xHalfCount = 40;
    [Range(1, 100)] public int zHalfCount = 40;
    public bool cullTest = false;
    public bool motionVectorTest = false;
    [Range(0.1f,3.0f)]public float spacingFactor = 1.1f;
    [Range(1, 16)]public int jobsPerBatch = 2;

    public Mesh mesh;
    public Material material;

    // opengle和opengles上只能使用ConstantBuffer，其他的都是使用RawBuffer
    private bool useConstantBuffer => BatchRendererGroup.BufferTarget == BatchBufferTarget.ConstantBuffer;
    
    private GraphicsBuffer gpuPersistentInstanceData;
    private BatchMeshID meshID;
    private BatchMaterialID materialID;
    private BatchRendererGroup brg;

    private uint instanceCount;         //生成多少个instance
    private uint batchCount;            //生成多少个batch
    private uint maxInstancePerBatch;   //每个batch最多多少个instance
    private BatchID[] batchIDs;          //每个batch的ID
    public struct SRPBatch
    {
        public uint rawBufferOffsetInFloat4;
        public uint instanceCount;
    };
    private SRPBatch[] srpBatches;   //每个batch的信息，包括在InstanceGraphicsBuffer的偏移和instance数量
    private NativeArray<float4> sysmemBuffer; //系统内存的Buffer，用于填充GPU的InstanceGraphicsBuffer
    
    private bool initialized = false;

    //创建一个NativeArray，用于填充GPU的InstanceGraphicsBuffer
    public static T* Malloc<T>(uint count) where T : unmanaged
    {
        return (T*)UnsafeUtility.Malloc(
            UnsafeUtility.SizeOf<T>() * count,
            UnsafeUtility.AlignOf<T>(),
            Allocator.TempJob);
    }
    
    //创建MetadataValue
    static MetadataValue CreateMetadataValue(int nameID, int gpuAddress, bool isOverridden)
    {
        const uint kIsOverriddenBit = 0x80000000;   //最高位是1，表示是Overridden的，0x80000000是2^31
        return new MetadataValue
        {
            NameID = nameID,
            Value = (uint)gpuAddress | (isOverridden ? kIsOverriddenBit : 0),
        };
    }
    
    private void Start()
    {
        brg = new BatchRendererGroup(this.OnPerformCulling, IntPtr.Zero);
        
        meshID = brg.RegisterMesh(mesh);
        materialID = brg.RegisterMaterial(material);
        
        const int kFloat4Size = 16;
        uint kBRGBufferMaxWindowSize = 128 * 1024 * 1024;   //128MB
        uint kBRGBufferAlignment = 16;                      //16字节对齐
        if (useConstantBuffer)
        {
            kBRGBufferMaxWindowSize = (uint)(BatchRendererGroup.GetConstantBufferMaxWindowSize());  //64KB
            kBRGBufferAlignment = (uint)(BatchRendererGroup.GetConstantBufferOffsetAlignment());    //16字节对齐
        }
        //创建InstanceGraphicsBuffer，创建一个或者多个Batch，取决于UBO的大小限制
        instanceCount = (uint)(xHalfCount * zHalfCount * 4);
        //计算每个Instance数据，包括Matrix和Color，有两个4*3的矩阵和一个Color，矩阵是unity_ObjectToWorld，unity_WorldToObject
        const uint kPerInstanceSize = (3*2 + 1);  
        //减4个float4是因为BRG的限制，前64个字节必须是0，计算每个Batch最多能放多少个Instance
        maxInstancePerBatch = ((kBRGBufferMaxWindowSize / kFloat4Size) - 4) / kPerInstanceSize;
        if (maxInstancePerBatch > instanceCount)
            maxInstancePerBatch = instanceCount;
        //计算需要多少个Batch，-1是因为C#的除法是向下取整
        batchCount = (instanceCount + maxInstancePerBatch - 1) / maxInstancePerBatch;
        //计算每个Batch的大小，需要对齐到16字节
        uint batchAlignedSizeInBytes = (((4 + maxInstancePerBatch * kPerInstanceSize)* kFloat4Size) + kBRGBufferAlignment - 1) & (~(kBRGBufferAlignment - 1)); // 4是前64字节必须是0
        //计算InstanceGraphicsBuffer的大小
        uint totalRawBufferSizeInBytes = batchCount * batchAlignedSizeInBytes;

        //创建InstanceGraphicsBuffer
        if (useConstantBuffer)
            gpuPersistentInstanceData = new GraphicsBuffer(GraphicsBuffer.Target.Constant, (int)totalRawBufferSizeInBytes / kFloat4Size, kFloat4Size);
        else
            gpuPersistentInstanceData = new GraphicsBuffer(GraphicsBuffer.Target.Raw, (int)totalRawBufferSizeInBytes / 4, 4);
        
        //创建Batch的MetaData
        var batchMetadata = new NativeArray<MetadataValue>(3, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        //Batch的MetaData
        int objectToWorldID = Shader.PropertyToID("unity_ObjectToWorld");
        int worldToObjectID = Shader.PropertyToID("unity_WorldToObject");
        int colorID = Shader.PropertyToID("_BaseColor");
            
        // 创建系统内存的Buffer，用于填充GPU的InstanceGraphicsBuffer
        sysmemBuffer = new NativeArray<float4>((int)(totalRawBufferSizeInBytes/kFloat4Size), Allocator.Persistent, NativeArrayOptions.ClearMemory);
        // 创建每个Batch的信息，包括在InstanceGraphicsBuffer的偏移和instance数量
        srpBatches = new SRPBatch[batchCount];
        // 创建每个Batch的ID
        batchIDs = new BatchID[batchCount];
        uint left = instanceCount;
        for (uint b=0;b< batchCount;b++)
        {
            uint offset = (b * batchAlignedSizeInBytes) / kFloat4Size;  //计算每个Batch的偏移量
            srpBatches[b].instanceCount = left > maxInstancePerBatch ? maxInstancePerBatch : left;  //计算每个Batch的Instance数量
            srpBatches[b].rawBufferOffsetInFloat4 = offset; //计算每个Batch的偏移量
            sysmemBuffer[(int)offset+0] = new float4(0, 0, 0, 0);  
            sysmemBuffer[(int)offset+1] = new float4(0, 0, 0, 0);
            sysmemBuffer[(int)offset+2] = new float4(0, 0, 0, 0);
            sysmemBuffer[(int)offset+3] = new float4(0, 0, 0, 0);

            batchMetadata[0] = CreateMetadataValue(objectToWorldID, 64, true);       // objectToWorld
            batchMetadata[1] = CreateMetadataValue(worldToObjectID, 64 + (int)srpBatches[b].instanceCount * kFloat4Size * 3 * 1, true); // worldToObject
            batchMetadata[2] = CreateMetadataValue(colorID, 64 + (int)srpBatches[b].instanceCount * kFloat4Size * 3 * 2, true); // colors

            uint batchWindowSize = 0;
            if (useConstantBuffer)
                batchWindowSize = (srpBatches[b].instanceCount * kPerInstanceSize + 4) * kFloat4Size;   // +4是因为前64字节必须是0
            //AddBatch，创建Batch, 参数1是Batch的MetaData，参数2是InstanceGraphicsBuffer，参数3是Batch在InstanceGraphicsBuffer的偏移量，参数4是Batch的大小
            batchIDs[b] = brg.AddBatch(batchMetadata, gpuPersistentInstanceData.bufferHandle, srpBatches[b].rawBufferOffsetInFloat4 * kFloat4Size, batchWindowSize);
            //计算剩余的Instance数量
            left -= srpBatches[b].instanceCount;
        }
        
        // 设置GlobalBounds，这个Bounds是用来做Culling的，如果Instance的Bounds超过这个Bounds，那么就不会被渲染
        Bounds bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(1048576.0f, 1048576.0f, 1048576.0f));  //1048576.0f = 1024*1024
        brg.SetGlobalBounds(bounds);
        // 初始化每个Instance的位置和颜色
        UpdatePositionsAndColors(Vector3.zero);
        // 把系统内存的Buffer拷贝到GPU的InstanceGraphicsBuffer
        gpuPersistentInstanceData.SetData(sysmemBuffer);
        // 初始化完成
        initialized = true;
    }
    
    void UpdatePositionsAndColors(float3 pos)
    {
        for (uint b = 0; b < batchCount; b++)
        {
            uint batchOffset = srpBatches[b].rawBufferOffsetInFloat4 + 4;   //+4是因为前64字节必须是0,batchOffset是每个Batch的偏移量
            uint batchWorldToObjectOffset = batchOffset + srpBatches[b].instanceCount * 3 * 1;  //batchWorldToObjectOffset是每个Batch的WorldToObject偏移量
            uint batchColorOffset = batchOffset + srpBatches[b].instanceCount * 3 * 2;  //batchColorOffset是每个Batch的颜色偏移量
            for (uint i = 0; i < srpBatches[b].instanceCount; i++)
            {
                int instanceId = (int)(b * maxInstancePerBatch + i);
                int x = instanceId % (xHalfCount * 2) - xHalfCount;
                int z = instanceId / (zHalfCount * 2) - xHalfCount;
                float px = x * spacingFactor;
                float pz = z * spacingFactor;
                var distance = math.distance(new float3(pos.x + px, pos.y, pos.z + pz), float3.zero);
                float s = Time.time  * 3f + distance * 0.2f;
                float py = math.sin(s)*15;
                // objectToWorld
                sysmemBuffer[(int)(batchOffset + i * 3 + 0)] = new float4(1, 0, 0, 0);
                sysmemBuffer[(int)(batchOffset + i * 3 + 1)] = new float4(1, 0, 0, 0);
                sysmemBuffer[(int)(batchOffset + i * 3 + 2)] = new float4(1, pos.x + px, pos.y + py, pos.z + pz);
                
                // worldToObject
                sysmemBuffer[(int)(batchWorldToObjectOffset + i * 3 + 0)] = new float4(1, 0, 0, 0);
                sysmemBuffer[(int)(batchWorldToObjectOffset + i * 3 + 1)] = new float4(1, 0, 0, 0);
                sysmemBuffer[(int)(batchWorldToObjectOffset + i * 3 + 2)] = new float4(1, -(px + pos.x), -(py + pos.y), -(pz + pos.z));
                
                Color col = Color.HSVToRGB((math.abs(py) / 15.0f) % 1.0f, 1.0f, 1.0f);
                // 设置颜色
                sysmemBuffer[(int)(batchColorOffset + i)] = new float4((math.sin(s)+1)/2, (math.cos(s*1.1f)+1)/2, (math.sin(s*2.2f)+1)*(math.cos(s*2.2f)+1)/4, 1);
            }
        }
    }

    void Update()
    {
        //UpdatePositionsAndColors(Vector3.zero);
        JobHandle updateInstanceDataJobHandle = default;
        updateInstanceDataJobHandle = UpdatePositionsAndColorsWithJob(Vector3.zero);
        updateInstanceDataJobHandle.Complete();
        gpuPersistentInstanceData.SetData(sysmemBuffer);
    }
    
    private void OnDestroy()
    {
        if (initialized)
        {
            // 清理Batch
            for (uint b=0;b<batchCount;b++)
            {
                brg.RemoveBatch(batchIDs[b]);
            }
            // 清理Mesh和Material
            if (material) 
                brg.UnregisterMaterial(materialID);
            if (mesh) 
                brg.UnregisterMesh(meshID);

            // 清理BatchRendererGroup
            brg.Dispose();
            // 清理GPU的InstanceGraphicsBuffer
            gpuPersistentInstanceData.Dispose();
            // 清理系统内存的Buffer
            sysmemBuffer.Dispose();
        }
    }
    
    [BurstCompile]
    public unsafe JobHandle OnPerformCulling(
        BatchRendererGroup rendererGroup,
        BatchCullingContext cullingContext,
        BatchCullingOutput cullingOutput,
        IntPtr userContext)
    {
        // 如果没有初始化，直接返回
        if (!initialized)
        {
            return new JobHandle();
        }
        // BatchCullingOutputDrawCommands是用来存储DrawCommand的
        BatchCullingOutputDrawCommands drawCommands = new BatchCullingOutputDrawCommands();
        // 初始化BatchCullingOutputDrawCommands
        drawCommands.drawRangeCount = 1;    //DrawRange的数量
        drawCommands.drawRanges = Malloc<BatchDrawRange>(1);    //DrawRange的数组
        drawCommands.drawRanges[0] = new BatchDrawRange //DrawRange的内容
        {
            drawCommandsBegin = 0,              //DrawCommand的起始索引
            drawCommandsCount = batchCount,     //DrawCommand的数量
            filterSettings = new BatchFilterSettings    //DrawCommand的过滤设置
            {
                renderingLayerMask = 1,                 //渲染层级Mask
                layer = 0,                              //Layer
                motionMode = motionVectorTest ? MotionVectorGenerationMode.Object : MotionVectorGenerationMode.Camera,
                shadowCastingMode = ShadowCastingMode.On,
                receiveShadows = true,
                staticShadowCaster = false,
                allDepthSorted = false
            }
        };

        
        drawCommands.visibleInstances = Malloc<int>(instanceCount);     //可见的Instance的数组

        drawCommands.drawCommandCount = (Int32)batchCount;
        drawCommands.drawCommands = Malloc<BatchDrawCommand>(batchCount);

        int radius = (xHalfCount) * (zHalfCount);       // (grid/2)^2
        int radiusO = (radius * 90) / 100;
        int radiusI = (radiusO * 85) / 100;

        uint visOffset = 0;
        for (uint b=0;b<batchCount;b++)
        {
            uint visCount = 0;
            for (uint i = 0; i < srpBatches[b].instanceCount; i++)
            {
                bool visible = true;
                if (cullTest)
                {
                    int instanceId = (int)(b * maxInstancePerBatch + i);
                    int z = instanceId / (zHalfCount*2) - zHalfCount;
                    int x = instanceId % (xHalfCount*2) - xHalfCount;
                    if (math.sqrt(x*x + z*z) > 30)
                        visible = false;
                }
                if (visible)
                {
                    drawCommands.visibleInstances[visOffset + visCount] = (Int32)i;
                    visCount++;
                }
            }

            drawCommands.drawCommands[b] = new BatchDrawCommand
            {
                visibleOffset = visOffset,
                visibleCount = visCount,
                batchID = batchIDs[b],
                materialID = materialID,
                meshID = meshID,
                submeshIndex = 0,
                splitVisibilityMask = 0xff,
                flags = motionVectorTest ? BatchDrawCommandFlags.HasMotion : BatchDrawCommandFlags.None,
                sortingPosition = 0         //排序位置
            };
            visOffset += visCount;          //更新可见的Instance的偏移量
        }

        drawCommands.instanceSortingPositions = null;               //排序位置的数组
        drawCommands.instanceSortingPositionFloatCount = 0;         //排序位置的数量

        cullingOutput.drawCommands[0] = drawCommands;
        
        return new JobHandle();
    }

    [BurstCompile]
    private unsafe struct UpdateInstanceDataJob : IJobFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<float4> sysmemBuffer;
        public float3 originPos;
        public int xHalfCount;
        public int zHalfCount;
        public float spacingFactor;
        public uint batchOffset;
        public uint batchWorldToObjectOffset;
        public uint batchColorOffset;
        public uint instanceCount;
        public uint batchIndex;
        public float escapeTime;
        public void Execute(int index)
        {
            int instanceId = (int)(batchIndex * instanceCount + index);
            int x = instanceId % (xHalfCount * 2) - xHalfCount; 
            int z = instanceId / (zHalfCount * 2) - xHalfCount;
            float px = x * spacingFactor;
            float pz = z * spacingFactor;
            var distance = math.distance(new float3(originPos.x + px, originPos.y, originPos.z + pz), float3.zero);
            float s = escapeTime * 3f + distance * 0.2f;
            float py = math.sin(s)*15; 
            // objectToWorld
            sysmemBuffer[(int)(batchOffset + index * 3 + 0)] = new float4(1, 0, 0, 0);
            sysmemBuffer[(int)(batchOffset + index * 3 + 1)] = new float4(1, 0, 0, 0);
            sysmemBuffer[(int)(batchOffset + index * 3 + 2)] = new float4(1, originPos.x + px, originPos.y + py, originPos.z + pz);
            // worldToObject
            sysmemBuffer[(int)(batchWorldToObjectOffset + index * 3 + 0)] = new float4(1, 0, 0, 0);
            sysmemBuffer[(int)(batchWorldToObjectOffset + index * 3 + 1)] = new float4(1, 0, 0, 0);
            sysmemBuffer[(int)(batchWorldToObjectOffset + index * 3 + 2)] = new float4(1, -(px + originPos.x), -(py + originPos.y), -(pz + originPos.z));
            // color
            Color col = Color.HSVToRGB((math.abs(py) / 15.0f) % 1.0f, 1.0f, 1.0f);
            sysmemBuffer[(int)(batchColorOffset + index)] = new float4((math.sin(s)+1)/2, (math.cos(s*1.1f)+1)/2, (math.sin(s*2.2f)+1)*(math.cos(s*2.2f)+1)/4, 1);
        }
    }
    
    [BurstCompile]
    private JobHandle UpdatePositionsAndColorsWithJob(Vector3 originPos)
    {
        NativeArray<JobHandle> handles = new NativeArray<JobHandle>((int)batchCount, Allocator.Temp);
        for (int b = 0; b < batchCount; b++)
        {
            uint batchOffset = srpBatches[b].rawBufferOffsetInFloat4 + 4;   //+4是因为前64字节必须是0,batchOffset是每个Batch的偏移量
            uint batchWorldToObjectOffset = batchOffset + srpBatches[b].instanceCount * 3 * 1;  //batchWorldToObjectOffset是每个Batch的WorldToObject偏移量
            uint batchColorOffset = batchOffset + srpBatches[b].instanceCount * 3 * 2;  //batchColorOffset是每个Batch的颜色偏移量
            handles[b] = new UpdateInstanceDataJob
            {
                sysmemBuffer = sysmemBuffer,
                xHalfCount = xHalfCount,
                zHalfCount = zHalfCount,
                spacingFactor = spacingFactor,
                batchOffset = batchOffset,
                batchWorldToObjectOffset = batchWorldToObjectOffset,
                batchColorOffset = batchColorOffset,
                instanceCount = srpBatches[b].instanceCount,
                batchIndex = (uint)b,
                escapeTime = Time.time
            }.ScheduleParallel((int)srpBatches[b].instanceCount, 16, new JobHandle());
        }
        JobHandle.ScheduleBatchedJobs();
        return JobHandle.CombineDependencies(handles);
    }

    /*[BurstCompile]
    private unsafe struct UploadInstanceDataJob : IJobFor
    {
        public void Execute(int index)
        {
            throw new NotImplementedException();
        }
    }*/
}
