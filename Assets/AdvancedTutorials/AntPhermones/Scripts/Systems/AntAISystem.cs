using DOTS.DOD;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AntPhermonesSystemGroup))]
    [UpdateAfter(typeof(AntSpawnerSystem))]
    public partial struct AntAISystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LevelSettings>();
            state.RequireForUpdate<Ant>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var settings = SystemAPI.GetSingleton<LevelSettings>();
            var random = SystemAPI.GetSingletonRW<RandomSingleton>();
            var pheromones = SystemAPI.GetSingletonBuffer<Pheromone>();

            // 随机朝向
            var steeringJob = new RandomSteeringJob
            {
                random = random
            };
            var steeringJobHandle = steeringJob.ScheduleParallel(state.Dependency);
            
            // 检测信息素
            var pheromoneDetectionJob = new PheromoneDetectionJob
            {
                mapSize = (int)settings.mapSize,
                pheromones = pheromones
            };
            state.Dependency= pheromoneDetectionJob.ScheduleParallel(steeringJobHandle);
        }
    }
}
