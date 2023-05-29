using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.PHYSICS.LESSON3
{
    struct TornadoComponent : IComponentData
    {
        public float AngularSpeed;
        public float3 MoveDirection;
    }

    public class TornadoComponentAuthoring : MonoBehaviour
    {
        public float AngularSpeed = 5.0f;
        public float3 MoveDirection = new float3(0, 10, 0);
        public class Baker : Baker<TornadoComponentAuthoring>
        {
            public override void Bake(TornadoComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var data = new TornadoComponent
                {
                    AngularSpeed = authoring.AngularSpeed,
                    MoveDirection = authoring.MoveDirection
                };
                AddComponent(entity,data);
            }
        }
    }
}
