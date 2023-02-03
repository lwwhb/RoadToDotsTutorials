using Unity.Entities;

namespace DOTS.DOD.LESSON8
{
    public struct CubeSharedComponentData : ISharedComponentData
    {
        public float rotateSpeed;
        public float moveSpeed;
    }
    public struct SharingGroup : ISharedComponentData
    {
        //0 red, 1 green, 2 blue
        public int group;
    }
    
    public struct CubeComponentData : IComponentData
    {
        public float rotateSpeed;
        public float moveSpeed;
    }
}

