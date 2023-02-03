using DOTS.DOD.LESSON5;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.DOD.LESSON7
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(CubesMarchSystemGroup))]
    public partial struct CubesGeneratorSystem : ISystem
    {
        private float timer;
        private int totalCubes;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CubesGenerator>();
            timer = 0.0f;
            totalCubes = 0;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var generator = SystemAPI.GetSingleton<CubesGenerator>();
            if (totalCubes >= generator.generationTotalNum)
            {
                state.Enabled = false;
                return;
            }
            if (timer >= generator.tickTime)
            {
                var cubes = CollectionHelper.CreateNativeArray<Entity>(generator.generationNumPerTicktime, Allocator.Temp);
                state.EntityManager.Instantiate(generator.cubeProtoType, cubes);
                foreach (var cube in cubes)
                {
                    state.EntityManager.AddComponentData<RotateSpeed>(cube, new RotateSpeed
                    {
                        rotateSpeed = math.radians(generator.rotateSpeed)
                    });
                    state.EntityManager.AddComponentData<MovementSpeed>(cube, new MovementSpeed
                    {
                        movementSpeed = generator.moveSpeed
                    });
                    var randomSingleton = SystemAPI.GetSingletonRW<RandomSingleton>();
                    var randPos = randomSingleton.ValueRW.random.NextFloat3(-generator.targetAreaSize * 0.5f,
                        generator.targetAreaSize * 0.5f);
                    state.EntityManager.AddComponentData<RandomTarget>(cube, new RandomTarget()
                    {
                        targetPos = generator.targetAreaPos + new float3(randPos.x, 0, randPos.z)
                    });
                    randomSingleton = SystemAPI.GetSingletonRW<RandomSingleton>();
                    randPos = randomSingleton.ValueRW.random.NextFloat3(-generator.generatorAreaSize * 0.5f,
                        generator.generatorAreaSize * 0.5f);
                    var position = generator.generatorAreaPos + new float3(randPos.x, 0, randPos.z);
                    var transform = SystemAPI.GetAspectRW<TransformAspect>(cube);
                    transform.WorldPosition = position;
                }
                cubes.Dispose();
                totalCubes += generator.generationNumPerTicktime;
                timer -= generator.tickTime;
            }
            timer += Time.deltaTime;
        }
    }
}
