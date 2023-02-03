using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.DOD.LESSON7
{
    [BurstCompile]
    partial struct CubesMarchingEntityJob : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecbParallel;
        void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, MarchingCubesAspect aspect)
        {
            
            if (aspect.IsNeedDestroy())
            {
                ecbParallel.DestroyEntity(chunkIndex, entity);
            }
            else
            {
                aspect.Move(deltaTime);
            }
        }
    }
}
