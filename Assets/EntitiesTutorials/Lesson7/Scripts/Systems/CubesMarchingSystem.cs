using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Transforms;

namespace DOTS.DOD.LESSON7
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(CubesMarchSystemGroup))]
    [UpdateAfter(typeof(CubesGeneratorSystem))]
    public partial struct CubesMarchingSystem : ISystem
    {
        static readonly ProfilerMarker profilerMarker = new ProfilerMarker("CubesMarchWithEntity");
        EntityQuery cubesQuery;
        ComponentTypeHandle<LocalTransform> transformTypeHandle;
        ComponentTypeHandle<RotateSpeed> rotateSpeedTypeHandle;
        [BurstCompile]
        public void OnCreate(ref SystemState state) 
        {
            state.RequireForUpdate<MovementSpeed>();
            state.RequireForUpdate<RotateSpeed>();
            
            var queryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<RotateSpeed, LocalTransform>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState);
            cubesQuery = state.GetEntityQuery(queryBuilder);

            transformTypeHandle = state.GetComponentTypeHandle<LocalTransform>();
            rotateSpeedTypeHandle = state.GetComponentTypeHandle<RotateSpeed>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            using (profilerMarker.Auto())
            {
                var generator = SystemAPI.GetSingleton<CubesGenerator>();
                transformTypeHandle.Update(ref state);
                rotateSpeedTypeHandle.Update(ref state);
                var job0 = new StopCubesRotateChunkJob()
                {
                    deltaTime = SystemAPI.Time.DeltaTime,
                    elapsedTime = (float)SystemAPI.Time.ElapsedTime,
                    leftRightBound = new float2(generator.generatorAreaPos.x / 2, generator.targetAreaPos.x / 2),
                    transformTypeHandle = transformTypeHandle,
                    rotateSpeedTypeHandle = rotateSpeedTypeHandle
                };
                state.Dependency = job0.ScheduleParallel(cubesQuery, state.Dependency);

                EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
                EntityCommandBuffer.ParallelWriter ecbParallel = ecb.AsParallelWriter();

                var job1 = new CubesMarchingEntityJob()
                {
                    deltaTime = SystemAPI.Time.DeltaTime,
                    ecbParallel = ecbParallel
                };
                state.Dependency = job1.ScheduleParallel(state.Dependency);
                state.Dependency.Complete();
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }
        }
    }
}
