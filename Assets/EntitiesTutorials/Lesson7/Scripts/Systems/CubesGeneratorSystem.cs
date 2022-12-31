using Unity.Burst;
using Unity.Entities;

namespace DOTS.DOD.LESSON7
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(CubesMarchSystemGroup))]
    public partial struct CubesGeneratorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

        }
    }
}
