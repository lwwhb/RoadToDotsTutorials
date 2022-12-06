using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.DOD.LESSON0
{
    partial struct RotateCubeJob : IJobEntity
    {
        public float deltaTime;
        void Execute(ref LocalTransform transform, in RotateSpeed speed)
        {
            transform = transform.RotateY(speed.rotateSpeed * deltaTime);
        }
    }
    
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(CubeRotateWithIJobEntitySystemGroup))]
    public partial struct CubeRotateWithIJobEntitySystem : ISystem
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
            var job = new RotateCubeJob { deltaTime = SystemAPI.Time.DeltaTime };
            job.ScheduleParallel();
        }
    }
}
