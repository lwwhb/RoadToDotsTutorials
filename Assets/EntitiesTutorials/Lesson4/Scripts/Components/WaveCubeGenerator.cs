using Unity.Entities;

namespace DOTS.DOD.LESSON4
{
    struct WaveCubeGenerator : IComponentData
    {
        public Entity cubeProtoType;
        public int halfCountX;
        public int halfCountZ;
    }
}
