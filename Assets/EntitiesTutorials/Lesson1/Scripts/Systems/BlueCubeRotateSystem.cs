using DOTS.DOD.LESSON0;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace DOTS.DOD.LESSON1
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(RotateCubesFilterSystemGroup))]
    [BurstCompile]
    public partial struct BlueCubeRotateSystem : ISystem
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
            float deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (transform, speed, tag) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>,RefRO<BlueCubeTag>>())
            {
                transform.ValueRW = transform.ValueRO.RotateY(
                    speed.ValueRO.rotateSpeed * deltaTime);
            }
        }
    }
}
