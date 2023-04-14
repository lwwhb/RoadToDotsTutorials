using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace DOTS.DOD.GRAPHICS.LESSON2
{
    [BurstCompile]
    public struct SpawnJob : IJobParallelFor
    {
        public Entity prototype;
        public int halfCountX;
        public int halfCountZ;
        public bool useDisableRendering;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute(int index)
        {
            var e = Ecb.Instantiate(index, prototype);
            Ecb.SetComponent(index, e, new LocalToWorld { Value = ComputeTransform(index, e) });
        }

        public float4x4 ComputeTransform(int index, Entity e)
        {
            int x = index % (halfCountX * 2) - halfCountX;
            int z = index / (halfCountX * 2) - halfCountZ;
            float4x4 M = float4x4.TRS(
                new float3(x*1.1f, 0, z*1.1f),
                quaternion.identity,
                new float3(1));
            if (useDisableRendering && math.sqrt(x*x + z*z) > 30)
                Ecb.AddComponent<DisableRendering>(index, e);
            return M;
        }
    }
    public class CreateColoredWaveCubes : MonoBehaviour
    {
        [Range(10, 100)] public int xHalfCount = 40;
        [Range(10, 100)] public int zHalfCount = 40;
        public bool useDisableRendering = false;
        public Mesh mesh;
        public Material material;
        public Mesh[] changeMeshes;
        public Material[] changeMaterials;
        
        void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;
            EntityCommandBuffer ecbJob = new EntityCommandBuffer(Allocator.TempJob);

            var filterSettings = RenderFilterSettings.Default;
            filterSettings.ShadowCastingMode = ShadowCastingMode.Off;
            filterSettings.ReceiveShadows = false;

            var renderMeshArray = new RenderMeshArray(new[] { material }, new[] { mesh });
            var renderMeshDescription = new RenderMeshDescription
            {
                FilterSettings = filterSettings,
                LightProbeUsage = LightProbeUsage.Off,
            };
  
            var prototype = entityManager.CreateEntity();
            RenderMeshUtility.AddComponents(
                prototype,
                entityManager,
                renderMeshDescription,
                renderMeshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
            entityManager.AddComponentData(prototype, new CustomColor { customColor = new float4(0, 1, 1, 1) });
            entityManager.AddComponentData(prototype, new CustomMeshAndMaterial
            {
                sphere = changeMeshes[0],   
                capsule = changeMeshes[1],
                cylinder = changeMeshes[2],
                red = changeMaterials[0],
                green = changeMaterials[1],
                blue = changeMaterials[2]
            });
            var spawnJob = new SpawnJob
            {
                prototype = prototype,
                Ecb = ecbJob.AsParallelWriter(),
                halfCountX = xHalfCount,
                halfCountZ = zHalfCount,
                useDisableRendering = useDisableRendering
            };
            var spawnHandle = spawnJob.Schedule(4*xHalfCount*zHalfCount, 128);
            spawnHandle.Complete();

            ecbJob.Playback(entityManager);
            ecbJob.Dispose();
            entityManager.DestroyEntity(prototype);
        }
    }
}
