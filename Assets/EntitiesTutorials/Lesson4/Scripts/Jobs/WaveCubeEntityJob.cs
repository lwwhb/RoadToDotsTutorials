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
        void Execute(WaveCubeAspect aspect)
        {
            aspect.WaveCubes(elapsedTime);
        }
    }
}
