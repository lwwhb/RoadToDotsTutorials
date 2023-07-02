using Unity.Burst;
using Unity.Entities;
using Unity.Properties;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    /*[RequireMatchingQueriesForUpdate]
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
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var settings = SystemAPI.GetSingleton<LevelSettings>();
            var pheromones = SystemAPI.GetSingletonBuffer<Pheromone>();
        }
    }*/
}
