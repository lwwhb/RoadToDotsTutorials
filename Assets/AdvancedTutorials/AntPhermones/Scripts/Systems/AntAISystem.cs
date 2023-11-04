using DOTS.DOD;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AntPhermonesSystemGroup))]
    [CreateAfter(typeof(AntSpawnerSystem))]
    [UpdateAfter(typeof(AntSpawnerSystem))]
    public partial struct AntAISystem : ISystem, ISystemStartStop
    {
        private EntityQuery resourcesQuery;
        private EntityQuery homesQuery;
        private NativeArray<float2> resourcesPosArray;
        private NativeArray<float2> homesPosArray;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LevelSettings>();
            state.RequireForUpdate<Home>();
            state.RequireForUpdate<Resource>();
            state.RequireForUpdate<Ant>();
           
        }
        public void OnStartRunning(ref SystemState state)
        {
            resourcesQuery = state.GetEntityQuery( typeof(Resource));
            homesQuery = state.GetEntityQuery( typeof(Home));
            resourcesPosArray = resourcesQuery.ToComponentDataArray<Resource>(Allocator.Persistent).Reinterpret<float2>();
            homesPosArray = homesQuery.ToComponentDataArray<Home>(Allocator.Persistent).Reinterpret<float2>();
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
            var resourceJob = new ResourceDetectionJob
            {
                random = random,
                obstacleSize = settings.obstacleSize,
                mapSize = settings.mapSize,
                sizeScale = settings.sizeScale,
                bucketResolution = settings.bucketResolution,
                buckets = SystemAPI.GetSingletonBuffer<Bucket>().AsNativeArray(),
                resourcesPosition = resourcesPosArray,
                homesPosition= homesPosArray
            };
            var resourceJobHandle = resourceJob.ScheduleParallel(obstacleJobHandle);
            
            // 处理蚂蚁移动逻辑
            var combinedDependency = JobHandle.CombineDependencies(pheromoneDetectionJobHandle, obstacleJobHandle, resourceJobHandle);
            var antTransformJob = new AntTransformJob();
            var dynamicsJobHandle = antTransformJob.ScheduleParallel(combinedDependency);
            
            // 处理蚂蚁留下的信息素
            pheromoneDetectionJobHandle.Complete(); 
            var nativePheromones = pheromones.AsNativeArray();
            var pheromoneDropJob = new PheromoneDropJob
            {
                deltaTime = SystemAPI.Time.fixedDeltaTime,
                mapSize = (int)settings.mapSize,
                pheromones = nativePheromones
            };
            var pheromoneDropJobHandle = pheromoneDropJob.ScheduleParallel(dynamicsJobHandle);
            pheromoneDropJobHandle.Complete(); 
            
            // 处理信息素衰减
            var pheromoneDecayJob = new PheromoneDecayJob
            {
                pheromoneDecayRate = settings.pheromoneDecayRate,
                pheromones = pheromones
            };
            state.Dependency = pheromoneDecayJob.ScheduleParallel(pheromones.Length, 100, pheromoneDropJobHandle);
        }

        public void OnStopRunning(ref SystemState state)
        {
            resourcesPosArray.Dispose();
            homesPosArray.Dispose();
        }
    }
}