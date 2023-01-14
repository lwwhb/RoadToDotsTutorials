using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.DOD.LESSON7
{
    [BurstCompile]
    partial struct StopCubesRotateChunkJob : IJobChunk
    {
        public float deltaTime;
        public float elapsedTime;
        public float2 leftRightBound;
        public ComponentTypeHandle<LocalTransform> transformTypeHandle;
        public ComponentTypeHandle<RotateSpeed> rotateSpeedTypeHandle;
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var chunkTransforms = chunk.GetNativeArray(ref transformTypeHandle);
            var chunkRotateSpeeds = chunk.GetNativeArray(ref rotateSpeedTypeHandle);
            var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out var i))
            {
                bool enabled = chunk.IsComponentEnabled(ref rotateSpeedTypeHandle, i);
                if(enabled)
                {
                    if (chunkTransforms[i].Position.x > leftRightBound.x &&
                        chunkTransforms[i].Position.x < leftRightBound.y)
                    {
                        chunk.SetComponentEnabled(ref rotateSpeedTypeHandle, i, false);
                    }
                    else
                    {
                        var speed = chunkRotateSpeeds[i];
                        chunkTransforms[i] = chunkTransforms[i].RotateY(speed.rotateSpeed * deltaTime);
                    }
                }
                else
                {
                    if (chunkTransforms[i].Position.x < leftRightBound.x ||
                        chunkTransforms[i].Position.x > leftRightBound.y)
                    {
                        chunk.SetComponentEnabled(ref rotateSpeedTypeHandle, i, true);
                        var trans = chunkTransforms[i];
                        trans.Scale = 1.0f;
                        chunkTransforms[i] = trans;
                    }
                    else
                    {
                        var trans = chunkTransforms[i];
                        trans.Scale = math.sin(elapsedTime*4)*0.3f + 1.0f;
                        chunkTransforms[i] = trans;
                    }
                }
            }
        }
    }
}
