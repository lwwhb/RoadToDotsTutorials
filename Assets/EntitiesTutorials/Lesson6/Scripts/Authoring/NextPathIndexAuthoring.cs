using Unity.Entities;
using UnityEngine;

namespace DOTS.DOD.LESSON6
{
    struct NextPathIndex : IComponentData
    {
        public uint nextIndex;
    }

    public class NextPathIndexAuthoring : MonoBehaviour
    {
        [HideInInspector] public uint nextIndex = 0;

        public class Baker : Baker<NextPathIndexAuthoring>
        {
            public override void Bake(NextPathIndexAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var data = new NextPathIndex
                {
                    nextIndex = authoring.nextIndex
                };
                AddComponent(entity, data);
            }
        }
    }
}
