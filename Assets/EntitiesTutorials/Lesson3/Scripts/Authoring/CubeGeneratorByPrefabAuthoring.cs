using Unity.Entities;
using UnityEngine;

namespace DOTS.DOD.LESSON3
{
    struct CubeGeneratorByPrefab : IComponentData
    {
        public Entity cubeEntityProtoType;
        public int cubeCount;
    }
    
    class CubeGeneratorByPrefabAuthoring : Singleton<CubeGeneratorByPrefabAuthoring>
    {
        public GameObject cubePrefab = null;
        [Range(1, 10)] public int CubeCount = 6;
        
        class CubeBaker : Baker<CubeGeneratorByPrefabAuthoring>
        {
            public override void Bake(CubeGeneratorByPrefabAuthoring authoring)
            {
                AddComponent(new CubeGeneratorByPrefab
                {
                    cubeEntityProtoType = GetEntity(authoring.cubePrefab),
                    cubeCount = authoring.CubeCount
                });
            }
        }
    }
}
