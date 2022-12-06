using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace DOTS.DOD.LESSON0
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(CubeRotateSystemGroup))]
    [BurstCompile]
    public partial struct CubeRotateSystem : ISystem
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
            foreach (var (transform, speed) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>>())
            {
                transform.ValueRW = transform.ValueRO.RotateY(
                    speed.ValueRO.rotateSpeed * deltaTime);
            }
        }
    }
}
