using DOTS.DOD.LESSON0;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace DOTS.DOD.LESSON2
{
    struct RotateCubeWithJobChunk : IJobChunk
    {
        public float deltaTime;
        public ComponentTypeHandle<LocalTransform> TransformTypeHandle;
        [ReadOnly]public ComponentTypeHandle<RotateSpeed> RotationSpeedTypeHandle;
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var chunkTransforms = chunk.GetNativeArray(ref TransformTypeHandle);
            var chunkRotationSpeeds = chunk.GetNativeArray(ref RotationSpeedTypeHandle);
            
            var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out var i))
            {
                var speed = chunkRotationSpeeds[i];
                chunkTransforms[i] = chunkTransforms[i].RotateY(speed.rotateSpeed * deltaTime);
            }

            /*for (int i = 0, chunkEntityCount = chunk.Count; i < chunkEntityCount; i++)
            {
                var speed = chunkRotationSpeeds[i];
                chunkTransforms[i] = chunkTransforms[i].RotateY(speed.rotateSpeed * deltaTime);
            }*/
        }
    }
    
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(CubeRotateWithIJobChunkSystemGroup))]
    public partial struct CubeRotateWithIJobChunkSystem : ISystem
    {
        EntityQuery rotateCubes;
        ComponentTypeHandle<LocalTransform> transformTypeHandle;
        ComponentTypeHandle<RotateSpeed> rotationSpeedTypeHandle;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var queryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<RotateSpeed, LocalTransform>();
            rotateCubes = state.GetEntityQuery(queryBuilder);

            transformTypeHandle = state.GetComponentTypeHandle<LocalTransform>();
            rotationSpeedTypeHandle = state.GetComponentTypeHandle<RotateSpeed>(true);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            transformTypeHandle.Update(ref state);
            rotationSpeedTypeHandle.Update(ref state);

            var job = new RotateCubeWithJobChunk
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                TransformTypeHandle = transformTypeHandle,
                RotationSpeedTypeHandle = rotationSpeedTypeHandle
            };
            state.Dependency = job.ScheduleParallel(rotateCubes, state.Dependency);
        }
    }
}

