using Unity.Entities;
using UnityEngine;

namespace DOTS.DOD
{
    public class WaveCubeAuthoring : MonoBehaviour
    {
        public GameObject cubePrefab = null;
        [Range(10, 100)] public int xHalfCount = 40;
        [Range(10, 100)] public int zHalfCount = 40;
    }

    class WaveCubeBaker : Baker<WaveCubeAuthoring>
    {
        public override void Bake(WaveCubeAuthoring authoring)
        {
            AddComponent(new WaveCubeGenerator
            {
                cubeProtoType = GetEntity(authoring.cubePrefab),
                halfCountX = authoring.xHalfCount,
                halfCountZ = authoring.zHalfCount
            });
        }
    }
}
