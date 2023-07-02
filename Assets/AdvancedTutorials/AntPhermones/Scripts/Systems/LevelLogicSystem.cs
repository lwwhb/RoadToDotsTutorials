using DOTS.DOD;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AntPhermonesSystemGroup))]
    public partial struct LevelLogicSystem : ISystem, ISystemStartStop
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LevelSettings>();
            
        }
        
        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            var levelSettings = SystemAPI.GetSingletonRW<LevelSettings>();
            GenerateColonies(ref state, ref levelSettings.ValueRW);
            GenerateResources(ref state, levelSettings.ValueRO);
            GenerateObstacles(ref state, levelSettings.ValueRO);
            GeneratePheromones(ref state, levelSettings.ValueRO);
            state.Enabled = false;
        }
        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
        }
        
        [BurstCompile]
        private void GenerateColonies(ref SystemState state, ref LevelSettings settings)
        {
            int c =  (int)math.ceil(math.sqrt(settings.colonyNum));
            float s = 1.0f / c;
            settings.sizeScale = s;
            for (int i = 0; i < settings.colonyNum; i++)
            {
                float x = i % c + 0.5f;
                float y = i / c + 0.5f;
                var home = state.EntityManager.Instantiate(settings.homePrefab);
                var localTransform = SystemAPI.GetComponentRW<LocalTransform>(home);
                localTransform.ValueRW.Position = new float3(x*s, y*s, 0);
                localTransform.ValueRW.Scale = 4.0f*s/settings.mapSize;
            }
        }
        
        [BurstCompile]
        private void GenerateResources(ref SystemState state, LevelSettings settings)
        {
            RefRW<RandomSingleton> random = SystemAPI.GetSingletonRW<RandomSingleton>();
            int c =  (int)math.ceil(math.sqrt(settings.colonyNum));
            float s = settings.sizeScale;//1.0f / c;
            for (int i = 0; i < settings.colonyNum; i++)
            {
                float x = i % c + 0.5f;
                float y = i / c + 0.5f;
                float resourceAngle = random.ValueRW.random.NextFloat() * 2f * math.PI;
                var resource = state.EntityManager.Instantiate(settings.resourcePrefab);
                var localTransform = SystemAPI.GetComponentRW<LocalTransform>(resource);
                localTransform.ValueRW.Position = new float3(
                    x*s + 0.45f * s * math.cos(resourceAngle),
                    y*s + 0.45f * s * math.sin(resourceAngle), 
                    0);
                localTransform.ValueRW.Scale = 4.0f*s/settings.mapSize;
            }
        }

        [BurstCompile]
        private void GenerateObstacles(ref SystemState state, LevelSettings settings)
        {
            RefRW<RandomSingleton> random = SystemAPI.GetSingletonRW<RandomSingleton>();
            int c =  (int)math.ceil(math.sqrt(settings.colonyNum));
            float s = settings.sizeScale;//float s = 1.0f / c;
            int index = 0;
            foreach (var antSpawner in SystemAPI.Query<AntSpawnerSettings>())
            {
                float x = index % c + 0.5f;
                float y = index / c + 0.5f;
                int ringCount = antSpawner.ringCount;
                NativeList<float2> obstaclePositions = new NativeList<float2>(Allocator.Temp);
                for (int i = 1; i <= antSpawner.ringCount; ++i)
                {
                    float ringRadius = (i / (ringCount + 1f)) * (0.5f*s);
                    float circumference = ringRadius * 2f * math.PI;
                    int maxCount = (int)math.ceil(circumference / (antSpawner.blobData.Value.obstacleSize*s/settings.mapSize));
                    int offset = random.ValueRW.random.NextInt(0,maxCount);
                    int holeCount = random.ValueRW.random.NextInt(1,3);

                    for (int j = 0; j < maxCount; ++j)
                    {
                        float fillRatio = (float)j / maxCount;
                        if (((fillRatio * holeCount) % 1f) < antSpawner.blobData.Value.maxObstaclesFillRatio)
                        {
                            float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
                            var obstacle = state.EntityManager.Instantiate(settings.obstaclePrefab);

                            float2 obstaclePosition = new float2(x*s + math.cos(angle) * ringRadius, y*s + math.sin(angle) * ringRadius);

                            var localTransform = SystemAPI.GetComponentRW<LocalTransform>(obstacle);
                            localTransform.ValueRW.Position = new float3(obstaclePosition.x, obstaclePosition.y, 0);
                            localTransform.ValueRW.Scale = 4.0f*s/settings.mapSize;
                            obstaclePositions.Add(obstaclePosition);
                        }
                    }
                }
                index++;
            }
        }

        [BurstCompile]
        private void GeneratePheromones(ref SystemState state, LevelSettings settings)
        {
            var pheromones = state.EntityManager.CreateEntity();
            var pheromonesBuffer = state.EntityManager.AddBuffer<Pheromone>(pheromones);
            pheromonesBuffer.Length = (int)settings.mapSize * (int)settings.mapSize;
            for (var i = 0; i < pheromonesBuffer.Length; i++)
            {
                pheromonesBuffer[i] = new Pheromone { strength = 0f, colonyID = -1 };
            }
        }
    }
}
