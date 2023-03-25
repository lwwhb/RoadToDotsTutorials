using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Transforms;

namespace DOTS.DOD.GRAPHICS.LESSON2
{
    [BurstCompile]
    partial struct WaveCubeEntityJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public float elapsedTime;
        void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref LocalToWorld transform, ref CustomColor color)
        {
            var distance = math.distance(transform.Position, float3.zero);
            float s = elapsedTime * 3f + distance * 0.2f;
            float3 newPos = transform.Position + new float3(0, 1, 0) * math.sin(s);
            transform.Value = float4x4.Translate(newPos);
            color.customColor = new float4((math.sin(s)+1)/2, (math.cos(s*1.1f)+1)/2, (math.sin(s*2.2f)+1)*(math.cos(s*2.2f)+1)/4, 1);
        }
    }
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(GraphicsLesson2SystemGroup))]
    public partial struct ChangeWaveCubesColorSystem : ISystem
    {
        static readonly ProfilerMarker profilerMarker = new ProfilerMarker("ChangeWaveCubesColorSystem");
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CustomColor>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            using (profilerMarker.Auto())
            {
                EntityCommandBuffer ecbJob = new EntityCommandBuffer(Allocator.TempJob);
                var job = new WaveCubeEntityJob()
                {
                    ecb = ecbJob.AsParallelWriter(),
                    elapsedTime = (float)SystemAPI.Time.ElapsedTime
                };
                state.Dependency = job.ScheduleParallel(state.Dependency);
                state.Dependency.Complete();
                
                ecbJob.Playback(state.EntityManager);
                ecbJob.Dispose();
            }
        }
    }
}
