using Unity.Entities;
using UnityEngine;

namespace DOTS.DOD.LESSON9
{
    struct SpawnerGenerator : IComponentData
    {
        public Entity spawnerProtoType;
        public int halfCountX;
        public int halfCountZ;
    }

    public class SpawnerGeneratorAuthoring : MonoBehaviour
    {
        public GameObject spawnerPrefab = null;
        [Range(10, 100)] public int xHalfCount = 40;
        [Range(10, 100)] public int zHalfCount = 40;
        public class Baker : Baker<SpawnerGeneratorAuthoring>
        {
            public override void Bake(SpawnerGeneratorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var data = new SpawnerGenerator
                {
                    spawnerProtoType = GetEntity(authoring.spawnerPrefab, TransformUsageFlags.Dynamic),
                    halfCountX = authoring.xHalfCount,
                    halfCountZ = authoring.zHalfCount
                };
                AddComponent(entity, data);
            }
        }
    }
}
