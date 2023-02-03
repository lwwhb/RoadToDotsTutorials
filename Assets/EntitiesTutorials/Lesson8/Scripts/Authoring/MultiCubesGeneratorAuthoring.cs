using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.DOD.LESSON8
{
    struct MultiCubesGenerator : IComponentData
    {
        public Entity redCubeProtoType;
        public Entity greenCubeProtoType;
        public Entity blueCubeProtoType;
        public int generationTotalNum;
        public int generationNumPerTicktime;
        public float tickTime;
        public float3 redCubeGeneratorPos;
        public float3 greenCubeGeneratorPos;
        public float3 blueCubeGeneratorPos;
        public float3 cubeTargetPos;
    }

    public class MultiCubesGeneratorAuthoring : MonoBehaviour
    {
        public GameObject redCubePrefab = null;
        public GameObject greenCubePrefab = null;
        public GameObject blueCubePrefab = null;
        [Range(10, 10000)] public int generationTotalNum = 500;
        [Range(1, 60)] public int generationNumPerTicktime = 5;
        [Range(0.1f, 1.0f)] public float tickTime = 0.2f;
        public float3 redCubeGeneratorPos;
        public float3 greenCubeGeneratorPos;
        public float3 blueCubeGeneratorPos;
        public float3 cubeTargetPos;

        public class Baker : Baker<MultiCubesGeneratorAuthoring>
        {
            public override void Bake(MultiCubesGeneratorAuthoring authoring)
            {
                var data = new MultiCubesGenerator
                {
                    redCubeProtoType = GetEntity(authoring.redCubePrefab),
                    greenCubeProtoType = GetEntity(authoring.greenCubePrefab),
                    blueCubeProtoType = GetEntity(authoring.blueCubePrefab),
                    generationTotalNum = authoring.generationTotalNum,
                    generationNumPerTicktime = authoring.generationNumPerTicktime,
                    tickTime = authoring.tickTime,
                    redCubeGeneratorPos = authoring.redCubeGeneratorPos,
                    greenCubeGeneratorPos = authoring.greenCubeGeneratorPos,
                    blueCubeGeneratorPos = authoring.blueCubeGeneratorPos,
                    cubeTargetPos = authoring.cubeTargetPos
                };
                AddComponent(data);
            }
        }
    }
}
