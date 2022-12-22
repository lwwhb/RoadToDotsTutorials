using Unity.Entities;
using Unity.Transforms;

namespace DOTS.DOD.LESSON4
{
    readonly partial struct WaveCubeAspect : IAspect
    {
        readonly RefRW<LocalTransform> localTransform;
    }
}
