using DOTS.DOD;
using Mono.Cecil;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

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
            var pheromoneDetectionJobHandle = pheromoneDetectionJob.ScheduleParallel(steeringJobHandle);
            
            //检测障碍物
            var obstacleJob = new ObstacleDetectionJob
            {
                mapSize = settings.mapSize,
                obstacleSize = settings.obstacleSize,
                distance = settings.wallSteerDistance,
                steeringStrength = settings.wallSteerStrength,
                bucketResolution = settings.bucketResolution,
                buckets = SystemAPI.GetSingletonBuffer<Bucket>().AsNativeArray(),
                wallPushbackUnits = settings.wallPushbackUnits
            };
            var obstacleJobHandle = obstacleJob.ScheduleParallel(pheromoneDetectionJobHandle);
            
            // 检测食物
            /*var resourceTransform = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<Resource>());
            var homePosition = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<Home>());
            var resourceJob = new ResourceDetectionJob
            {
                obstacleSize = settings.obstacleSize,
                mapSize = settings.mapSize,
                steeringStrength = settings.resourceSteerStrength,
                bucketResolution = settings.bucketResolution,
                buckets = SystemAPI.GetSingletonBuffer<Bucket>().AsNativeArray(),
                homePosition = new float2(homePosition.Position.x, homePosition.Position.y),
                resourcePosition = new float2(resourceTransform.Position.x, resourceTransform.Position.y)
            };
            var resourceJobHandle = resourceJob.ScheduleParallel(obstacleJobHandle);*/
            
            // Dynamics
            //var combinedDependency = JobHandle.CombineDependencies(pheromoneDetectionJobHandle, obstacleJobHandle, resourceJobHandle);

            var antTransformJob = new AntTransformJob();
            //state.Dependency = antTransformJob.ScheduleParallel(combinedDependency);
            state.Dependency = antTransformJob.ScheduleParallel(obstacleJobHandle);
            //
        }
    }
}
