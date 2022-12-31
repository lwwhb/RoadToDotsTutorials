using DOTS.DOD.LESSON3;
using Unity.Burst;
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
            var job = new CubeRotateAndMoveEntityJob
            {
                deltaTime = SystemAPI.Time.DeltaTime
            };
            job.ScheduleParallel();
        }
    }
}
