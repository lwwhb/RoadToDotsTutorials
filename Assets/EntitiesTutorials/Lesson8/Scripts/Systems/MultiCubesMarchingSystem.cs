using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.DOD.LESSON8
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(MultiCubesMarchSystemGroup))]
    [UpdateAfter(typeof(MultiCubesGenerateSystem))]
    public partial struct MultiCubesMarchingSystem : ISystem
    {
        EntityQuery cubesQuery;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var queryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, CubeSharedComponentData, SharingGroup>();
            //var queryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, CubeComponentData, SharingGroup>();
            cubesQuery = state.GetEntityQuery(queryBuilder);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            double elapsedTime = SystemAPI.Time.ElapsedTime;
            var generator = SystemAPI.GetSingleton<MultiCubesGenerator>();
            cubesQuery.SetSharedComponentFilter(new SharingGroup { group = 1 });
            var cubeEntities = cubesQuery.ToEntityArray(Allocator.Temp);
            var localTransforms = cubesQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            for (int i = 0; i < cubeEntities.Length; i++)
            {
                var data = state.EntityManager.GetSharedComponent<CubeSharedComponentData>(cubeEntities[i]);
                //var data = state.EntityManager.GetComponentData<CubeComponentData>(cubeEntities[i]);
                LocalTransform temp = localTransforms[i];
                
                if (temp.Position.x > generator.cubeTargetPos.x)
                {
                    state.EntityManager.DestroyEntity(cubeEntities[i]);
                }
                else
                {
                    temp.Position += data.moveSpeed * deltaTime * new float3(1, (float)math.sin(elapsedTime*20), 0);
                    temp = temp.RotateY(data.rotateSpeed * deltaTime);
                    localTransforms[i] = temp;
                    state.EntityManager.SetComponentData(cubeEntities[i], localTransforms[i]);
                }
            }
            localTransforms.Dispose();
            cubeEntities.Dispose();
        }
    }
}
