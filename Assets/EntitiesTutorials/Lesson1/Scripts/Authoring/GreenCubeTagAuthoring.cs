using Unity.Entities;
using UnityEngine;

namespace DOTS.DOD.LESSON1
{
    struct GreenCubeTag : IComponentData
    {
    }
    public class GreenCubeTagAuthoring : MonoBehaviour
    {
        public class Baker : Baker<GreenCubeTagAuthoring>
        {
            public override void Bake(GreenCubeTagAuthoring authoring)
            {
                var greenTag = new GreenCubeTag();
                AddComponent(greenTag);
            }
        }
    }
}
