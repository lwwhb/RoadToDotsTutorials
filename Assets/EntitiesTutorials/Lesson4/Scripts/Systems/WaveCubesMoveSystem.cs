using Unity.Burst;
using Unity.Entities;
using Unity.Profiling;

namespace DOTS.DOD.LESSON4
{
    [BurstCompile]
    [UpdateInGroup(typeof(WaveCubesWithDotsSystemGroup))]
    [UpdateAfter(typeof(WaveCubesGenerateSystem))]
    public partial struct WaveCubesMoveSystem : ISystem
    {
        static readonly ProfilerMarker profilerMarker = new ProfilerMarker("WaveCubeEntityJobs");
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
            using (profilerMarker.Auto())
            {
                double elapsedTime = SystemAPI.Time.DeltaTime;
                var job = new WaveCubeEntityJob() { elapsedTime = (float)SystemAPI.Time.ElapsedTime };
                job.ScheduleParallel();
            }
        }
    }
}