using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.PHYSICS.LESSON3
{
    struct MultiPhysicsBodyGenerator : IComponentData
    {
        public Entity redCubeEntityProtoTypes;
        public Entity greenCubeEntityProtoTypes;
        public Entity blueCubeEntityProtoTypes;
        public float3 redGridNums;
        public float3 greenGridNums;
        public float3 blueGridNums;
        public float space;
    }

    public class MultiPhysicsBodyGeneratorAuthoring : MonoBehaviour
    {
        public GameObject[] cubePrefab = new GameObject[3];
        public Vector3[] gridNum = new Vector3[3];
        public float space = 1.1f;
        public class Baker : Baker<MultiPhysicsBodyGeneratorAuthoring>
        {
            public override void Bake(MultiPhysicsBodyGeneratorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var data = new MultiPhysicsBodyGenerator();
                data.redCubeEntityProtoTypes = GetEntity(authoring.cubePrefab[0], TransformUsageFlags.None);
                data.greenCubeEntityProtoTypes = GetEntity(authoring.cubePrefab[1], TransformUsageFlags.None);
                data.blueCubeEntityProtoTypes = GetEntity(authoring.cubePrefab[2], TransformUsageFlags.None);
                data.redGridNums = authoring.gridNum[0];
                data.greenGridNums = authoring.gridNum[1];
                data.blueGridNums= authoring.gridNum[2];
                data.space = authoring.space;
                AddComponent(entity, data);
            }
        }
    }
}
