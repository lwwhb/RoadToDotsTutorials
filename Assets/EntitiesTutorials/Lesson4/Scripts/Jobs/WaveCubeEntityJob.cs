using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.DOD.LESSON4
{
    [BurstCompile]
    partial struct WaveCubeEntityJob : IJobEntity
    {
        [ReadOnly] public float elapsedTime;
        void Execute(ref LocalTransform transform)
        {
            var distance = math.distance(transform.Position, float3.zero);
            transform.Position += new float3(0, 1, 0) * math.sin(elapsedTime * 3f + distance * 0.2f);
        }
    }
}
