using DOTS.DOD.LESSON3;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace DOTS.DOD.LESSON5
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(RandomGenerateCubesSystemGroup))]
    [UpdateAfter(typeof(RandomCubeGenerateSystem))]
    public partial struct RandomCubeMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RotateAndMoveSpeed>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            EntityCommandBuffer.ParallelWriter ecbParallel = ecb.AsParallelWriter();
            var job = new CubeRotateAndMoveEntityJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                ecbParallel = ecbParallel
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
            state.Dependency.Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
