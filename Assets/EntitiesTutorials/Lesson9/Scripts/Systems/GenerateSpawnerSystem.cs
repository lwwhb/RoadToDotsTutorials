using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.DOD.LESSON9
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(SpawnerBlobSystemGroup))]
    public partial struct GenerateSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnerGenerator>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var generator = SystemAPI.GetSingleton<SpawnerGenerator>();
            var spawners = CollectionHelper.CreateNativeArray<Entity>(4*generator.halfCountX*generator.halfCountZ, Allocator.Temp);
            state.EntityManager.Instantiate(generator.spawnerProtoType, spawners);

            int count = 0;
            foreach (var cube in spawners)
            {
                int x = count % (generator.halfCountX * 2) - generator.halfCountX;
                int z = count / (generator.halfCountX * 2) - generator.halfCountZ;
                var position = new float3(x*1.1f, 0, z*1.1f);
                
                var transform = SystemAPI.GetAspectRW<TransformAspect>(cube);
                transform.LocalPosition = position;
                count++;
            }

            spawners.Dispose();
            // 此System只在启动时运行一次，所以在第一次更新后关闭它。
            state.Enabled = false;
        }
    }
}
