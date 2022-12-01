using Unity.Entities;

namespace DOTS.DOD
{
    struct WaveCubeGenerator : IComponentData
    {
        public Entity cubeProtoType;
        public int halfCountX;
        public int halfCountZ;
    }
}
