using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.DOD.LESSON1
{
    struct BlueCubeTag : IComponentData
    {
    }
    public class BlueCubeTagAuthoring : MonoBehaviour
    {
        public class Baker : Baker<BlueCubeTagAuthoring>
        {
            public override void Bake(BlueCubeTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var blueCube = new BlueCubeTag();
                AddComponent(entity, blueCube);
            }
        }
    }
}