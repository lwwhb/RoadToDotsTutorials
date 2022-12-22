using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace DOTS.DOD.LESSON8
{
    struct CubeGeneratorByScript : IComponentData
    {
        public Entity cubeEntityProtoType;
        public int cubeCount;
    }

    public class CreateEntitiesByScript : MonoBehaviour
    {
        public Mesh mesh = null;
        public Material material = null;
        [Range(1, 10)] public int CubeCount = 6;
        void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;
            Entity entity = entityManager.CreateEntity();
            
            entityManager.AddComponent<CubeGeneratorByScript>(entity);
            
            /*Entity cubeEntity = entityManager.CreateEntity();
            entityManager.AddComponent<LocalToWorldTransform>(cubeEntity);
            var renderMesh = new RenderMeshArray(
                new [] { material },
                new [] { mesh });
            var renderMeshDescription = new RenderMeshDescription(ShadowCastingMode.On);
            RenderMeshUtility.AddComponents(cubeEntity, entityManager, renderMeshDescription, renderMesh, MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

            var cubePrototype = entityManager.GetComponentData<CubeGeneratorByScript>(entity);
            cubePrototype.cubeEntityProtoType = cubeEntity;
            cubePrototype.cubeCount = CubeCount;
            entityManager.SetComponentData(entity, cubePrototype);*/
        }
        
    }
    
    [BurstCompile]
    [UpdateInGroup(typeof(CreateEntitiesByScriptsSystemGroup))]
    public partial struct CubeGenerateByScriptSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CubeGeneratorByScript>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var generator = SystemAPI.GetSingleton<CubeGeneratorByScript>();
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
