using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
namespace DOTS.DOD
{
    [BurstCompile]
    public partial struct WaveCubesMoveSystem : ISystem
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
            
            /*foreach (var (transform, speed) in
                     SystemAPI.Query<RefRW<LocalToWorldTransform>, RefRO<RotationSpeed>>())
            {
                transform.ValueRW.Value =
                    transform.ValueRO.Value.RotateY(speed.ValueRO.RadiansPerSecond * SystemAPI.Time.DeltaTime);
            }
            
            foreach (var transform in SystemAPI.Query<RefRW<LocalToWorldTransform>>())
            {
                transform.ValueRW.Value = transform.ValueRO.Value.RotateY(deltaTime);
            }*/
        }
    }
}