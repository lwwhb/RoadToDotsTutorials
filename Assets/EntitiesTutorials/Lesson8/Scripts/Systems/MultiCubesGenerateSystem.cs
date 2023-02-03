using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace DOTS.DOD.LESSON8
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(MultiCubesMarchSystemGroup))]
    public partial struct MultiCubesGenerateSystem : ISystem
    {
        private float timer;
        private int totalCubes;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MultiCubesGenerator>();
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
            var generator = SystemAPI.GetSingleton<MultiCubesGenerator>();
            if (totalCubes >= generator.generationTotalNum)
            {
                state.Enabled = false;
                return;
            }
            if (timer >= generator.tickTime)
            {
                Entity redCube = state.EntityManager.Instantiate(generator.redCubeProtoType);
                Entity greenCube = state.EntityManager.Instantiate(generator.greenCubeProtoType);
                Entity blueCube = state.EntityManager.Instantiate(generator.blueCubeProtoType);
                
                state.EntityManager.AddSharedComponent<CubeSharedComponentData>(redCube, new CubeSharedComponentData
                //state.EntityManager.AddComponentData<CubeComponentData>(redCube, new CubeComponentData
                {
                    rotateSpeed = math.radians(180.0f),
                    moveSpeed = 5.0f
                    //moveSpeed = Random.CreateFromIndex((uint)totalCubes).NextFloat(5.0f, 20.0f)
                });
                state.EntityManager.AddSharedComponent<SharingGroup>(redCube, new SharingGroup
                {
                    group = 0
                });
                state.EntityManager.AddSharedComponent<CubeSharedComponentData>(greenCube, new CubeSharedComponentData
                //state.EntityManager.AddComponentData<CubeComponentData>(greenCube, new CubeComponentData
                {
                    rotateSpeed = math.radians(180.0f),
                     moveSpeed = 5.0f
                    //moveSpeed = Random.CreateFromIndex((uint)totalCubes).NextFloat(5.0f, 10.0f)
                });
                state.EntityManager.AddSharedComponent<SharingGroup>(greenCube, new SharingGroup
                {
                    group = 1
                });
                state.EntityManager.AddSharedComponent<CubeSharedComponentData>(blueCube, new CubeSharedComponentData
                //state.EntityManager.AddComponentData<CubeComponentData>(blueCube, new CubeComponentData
                {
                    rotateSpeed = math.radians(180.0f),
                    moveSpeed = 5.0f
                    //moveSpeed = Random.CreateFromIndex((uint)totalCubes).NextFloat(10.0f, 20.0f)
                });
                state.EntityManager.AddSharedComponent<SharingGroup>(blueCube, new SharingGroup
                {
                    group = 2
                });
                
                var redCubeTransform = SystemAPI.GetAspectRW<TransformAspect>(redCube);
                redCubeTransform.WorldPosition = generator.redCubeGeneratorPos;
                
                var greenCubeTransform = SystemAPI.GetAspectRW<TransformAspect>(greenCube);
                greenCubeTransform.WorldPosition = generator.greenCubeGeneratorPos;
                
                var blueCubeTransform = SystemAPI.GetAspectRW<TransformAspect>(blueCube);
                blueCubeTransform.WorldPosition = generator.blueCubeGeneratorPos;

                totalCubes += 3;
                timer -= generator.tickTime;
            }
            timer += Time.deltaTime;
        }
    }
}
