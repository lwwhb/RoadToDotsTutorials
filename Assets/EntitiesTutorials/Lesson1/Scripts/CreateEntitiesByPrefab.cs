using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.DOD.LESSON1
{
    public class CubeAuthoring : MonoBehaviour
    {
        public GameObject cubePrefab = null;
        [Range(1, 10)] public int CubeCount = 6;
    }

    struct CubeGeneratorByPrefab : IComponentData
    {
        public Entity cubeEntityProtoType;
        public int cubeCount;
    }

    class CubeBaker : Baker<CubeAuthoring>
    {
        public override void Bake(CubeAuthoring authoring)
        {
            AddComponent(new CubeGeneratorByPrefab
            {
                cubeEntityProtoType = GetEntity(authoring.cubePrefab),
                cubeCount = authoring.CubeCount
            });
        }
    }

    [BurstCompile]
    [UpdateInGroup(typeof(CreateEntitiesByPrefabSystemGroup))]
    public partial struct CubeGenerateByPrefabSystem : ISystem
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
                var position = new float3((count - generator.cubeCount*0.5f)*1.1f , 0, 0);
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
