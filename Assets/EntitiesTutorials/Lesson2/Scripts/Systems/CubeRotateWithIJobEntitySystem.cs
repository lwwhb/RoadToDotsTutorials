using DOTS.DOD.LESSON0;
using DOTS.DOD.LESSON2;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.DOD.LESSON3
{
    partial struct RotateCubeWithJobEntity : IJobEntity
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
            var job = new RotateCubeWithJobEntity { deltaTime = SystemAPI.Time.DeltaTime };
            job.ScheduleParallel();
            //job.Schedule();
            //job.Run();
        }
    }
}
