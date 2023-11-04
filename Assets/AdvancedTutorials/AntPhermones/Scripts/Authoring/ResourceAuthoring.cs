using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    public struct Resource : IComponentData
    {
        public float2 position;
    }
    public class ResourceAuthoring : MonoBehaviour
    {
        private class ResourceAuthoringBaker : Baker<ResourceAuthoring>
        {
            public override void Bake(ResourceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                var resource = new Resource()
                {
                    position = float2.zero
                };
                AddComponent(entity, resource);
            }
        }
    }
}