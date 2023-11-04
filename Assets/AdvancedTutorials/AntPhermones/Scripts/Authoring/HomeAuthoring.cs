using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    public struct Home : IComponentData
    {
        public float2 position;
    }
    public class HomeAuthoring : MonoBehaviour
    {
        private class HomeAuthoringBaker : Baker<HomeAuthoring>
        {
            public override void Bake(HomeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                var home = new Home()
                {
                    position = float2.zero
                };
                AddComponent(entity, home);
            }
        }
    }
}