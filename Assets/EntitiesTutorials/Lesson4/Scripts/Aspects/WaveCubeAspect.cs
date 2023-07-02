using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.DOD.LESSON4
{
    readonly partial struct WaveCubeAspect : IAspect
    {
        public readonly RefRW<LocalTransform> localTransform;
        public readonly RefRO<WaveCubeTag> waveCubeTag;
        public void WaveCubes(float elapsedTime)
        {
            var distance = math.distance(localTransform.ValueRW.Position, float3.zero);
            localTransform.ValueRW.Position += new float3(0, 1, 0) * math.sin(elapsedTime * 3f + distance * 0.2f);
        }
    }
}
