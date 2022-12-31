using DOTS.DOD.LESSON3;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace DOTS.DOD.LESSON5
{
    [BurstCompile]
    partial struct GenerateCubesWithParallelWriterJob : IJobFor
    {
        [ReadOnly] public Entity cubeProtoType;
        public NativeArray<Entity> cubes;
        public EntityCommandBuffer.ParallelWriter ecbParallel;
        [NativeDisableUnsafePtrRestriction]public RefRW<RandomSingleton> random;
        public void Execute(int index)
        {
            cubes[index] = ecbParallel.Instantiate(index, cubeProtoType);
            ecbParallel.AddComponent<RotateAndMoveSpeed>(index, cubes[index], new RotateAndMoveSpeed
            {
                rotateSpeed = math.radians(60.0f),
                moveSpeed = 5.0f
            });
            float2 targetPos2D = random.ValueRW.random.NextFloat2(new float2(-15, -15), new float2(15, 15));
            ecbParallel.AddComponent<RandomTarget>(index, cubes[index], new RandomTarget()
            {
                targetPos = new float3(targetPos2D.x, 0, targetPos2D.y)
            });
        }
    }
}
