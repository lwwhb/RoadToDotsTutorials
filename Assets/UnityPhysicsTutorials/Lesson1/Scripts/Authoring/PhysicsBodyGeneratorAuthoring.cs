using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.PHYSICS.LESSON0
{
    struct PhysicsBodyGenerator : IComponentData
    {
        public Entity cubeEntityProtoType;
        public float3 gridNum;
        public float space;
    }

    public class PhysicsBodyGeneratorAuthoring : MonoBehaviour
    {
        public GameObject cubePrefab = null;
        public Vector3 gridNum;
        public float space = 1.1f;

        public class Baker : Baker<PhysicsBodyGeneratorAuthoring>
        {
            public override void Bake(PhysicsBodyGeneratorAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new PhysicsBodyGenerator
                {
                    cubeEntityProtoType = GetEntity(authoring.cubePrefab, TransformUsageFlags.None),
                    gridNum = authoring.gridNum,
                    space = authoring.space
                });
            }
        }
    }
}
