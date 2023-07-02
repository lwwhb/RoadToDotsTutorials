using DOTS.DOD;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AntPhermonesSystemGroup))]
    [UpdateAfter(typeof(LevelLogicSystem))]
    public partial struct AntSpawnerSystem : ISystem, ISystemStartStop
    {
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LevelSettings>();
            state.RequireForUpdate<AntSpawnerSettings>();
        }
        
        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            GenerateAnts(ref state);
            InitialAnts(ref state);
            state.Enabled = false;
        }
        
        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
        }

        [BurstCompile]
        private void GenerateAnts(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            int colonyID = 0;
            foreach (var settings in SystemAPI.Query<RefRO<AntSpawnerSettings>>())
            {
                var ants = state.EntityManager.Instantiate(settings.ValueRO.antPrefab, settings.ValueRO.antCount, Allocator.Temp);
                ecb.SetSharedComponent(ants, new ColonyID { id = colonyID });
                colonyID++;
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        private void InitialAnts(ref SystemState state)
        {
            LevelSettings levelSettings = SystemAPI.GetSingleton<LevelSettings>();
            float s = levelSettings.sizeScale;
            RefRW<RandomSingleton> random = SystemAPI.GetSingletonRW<RandomSingleton>();
            int colonyID = 0;
            foreach (var (transform, settings) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<AntSpawnerSettings>>())
            {
                float2 spawnerPosition = transform.ValueRO.Position.xy;
                foreach (var (localTransform, position, direction,  speed) 
                         in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Position>, RefRW<Direction>, RefRW<Speed>>().WithAll<Ant>().WithSharedComponentFilter<ColonyID>(new ColonyID{ id = colonyID }))
                {
                    position.ValueRW.position = spawnerPosition + new float2(random.ValueRW.random.NextFloat(-5f,5f)*s,random.ValueRW.random.NextFloat(-5f,5f)*s)/levelSettings.mapSize;
                    direction.ValueRW.direction = random.ValueRW.random.NextFloat(0, 360.0f);
                    speed.ValueRW.speed = settings.ValueRO.antMaxSpeed;
                    localTransform.ValueRW.Position =
                        new float3(position.ValueRW.position.x, position.ValueRW.position.y, 0);
                    localTransform.ValueRW.Scale = settings.ValueRO.antScale*s/levelSettings.mapSize;
                }
                colonyID++;
            }
        }
    }
}
