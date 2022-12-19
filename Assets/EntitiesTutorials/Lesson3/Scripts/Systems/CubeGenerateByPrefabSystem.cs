using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.DOD.LESSON3
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(CreateEntitiesByPrefabSystemGroup))]
    partial struct CubeGenerateByPrefabSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CubeGeneratorByPrefab>();
        }
    
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
    
        }
    
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var generator = SystemAPI.GetSingleton<CubeGeneratorByPrefab>();
            var cubes = CollectionHelper.CreateNativeArray<Entity>(generator.cubeCount, Allocator.Temp);
            state.EntityManager.Instantiate(generator.cubeEntityProtoType, cubes);
            int count = 0;
            foreach (var cube in cubes)
            {
                state.EntityManager.AddComponentData<RotateAndMoveSpeed>(cube, new RotateAndMoveSpeed
                {
                    rotateSpeed = count*math.radians(60.0f),
                    moveSpeed = count
                });
                var position = new float3((count - generator.cubeCount * 0.5f) * 1.2f, 0, 0);
                var transform = SystemAPI.GetAspectRW<TransformAspect>(cube);
                transform.LocalPosition = position;
                count++;
            }
    
            cubes.Dispose();
            // 此System只在启动时运行一次，所以在第一次更新后关闭它。
            state.Enabled = false;
        }
    }
}
