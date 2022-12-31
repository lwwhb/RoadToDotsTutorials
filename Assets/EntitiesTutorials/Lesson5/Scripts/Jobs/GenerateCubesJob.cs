using DOTS.DOD.LESSON3;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.DOD.LESSON5
{
    [BurstCompile]
    struct GenerateCubesJob : IJobFor
    {
        [ReadOnly] public Entity cubeProtoType;
        public NativeArray<Entity> cubes;
        public EntityCommandBuffer ecb;
        [NativeDisableUnsafePtrRestriction]public RefRW<RandomSingleton> random;

        public void Execute(int index)
        {
            cubes[index] = ecb.Instantiate(cubeProtoType);
            ecb.AddComponent<RotateAndMoveSpeed>(cubes[index], new RotateAndMoveSpeed
            {
                rotateSpeed = math.radians(60.0f),
                moveSpeed = 5.0f
            });
            float2 targetPos2D = random.ValueRW.random.NextFloat2(new float2(-15, -15), new float2(15, 15));
            ecb.AddComponent<RandomTarget>(cubes[index], new RandomTarget()
            {
                targetPos = new float3(targetPos2D.x, 0, targetPos2D.y)
            });
                
        }
    }
}
