using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace DOTS.DOD.LESSON0
{
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
                     SystemAPI.Query<RefRW<LocalToWorldTransform>, RefRO<RotateSpeed>>())
            {
                transform.ValueRW.Value = transform.ValueRO.Value.RotateY(
                    speed.ValueRO.rotateSpeed * deltaTime);
            }
        }
    }
}
