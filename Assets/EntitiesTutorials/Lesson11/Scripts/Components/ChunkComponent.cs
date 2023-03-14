using Unity.Entities;

namespace DOTS.DOD.LESSON11
{
    struct ChunkComponentA : IComponentData
    {
        public int numA;
    }
    struct ChunkComponentB : IComponentData
    {
        public int numB;
    }
    struct ChunkComponentAB : IComponentData
    {
        public int numA;
        public int numB;
    }
}
