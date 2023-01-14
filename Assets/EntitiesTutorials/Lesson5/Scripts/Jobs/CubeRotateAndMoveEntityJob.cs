using DOTS.DOD.LESSON3;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.DOD.LESSON5
{
    [BurstCompile]
    partial struct CubeRotateAndMoveEntityJob : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecbParallel;
        void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity ,ref LocalTransform transform, in RandomTarget target, in RotateAndMoveSpeed speed)
        {
            var distance = math.distance(transform.Position, target.targetPos);
            if (distance < 0.02f)
            {
                ecbParallel.DestroyEntity(chunkIndex, entity);
            }
            else
            {
                float3 dir =  math.normalize(target.targetPos - transform.Position);
                transform.Position += dir * speed.moveSpeed * deltaTime;
                transform = transform.RotateY(speed.rotateSpeed * deltaTime);
            }
        }
    }
}
