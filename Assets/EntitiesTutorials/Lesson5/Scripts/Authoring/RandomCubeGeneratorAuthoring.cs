using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.DOD.LESSON5
{
    struct RandomCubeGenerator : IComponentData
    {
        public Entity cubeProtoType;
        public int generationTotalNum;
        public int generationNumPerTicktime;
        public float tickTime;
        public bool useScheduleParallel;
    }

    public class RandomCubeGeneratorAuthoring : MonoBehaviour
    {
        public GameObject redCubePrefab = null;
        public GameObject blueCubePrefab = null;
        [Range(10, 10000)] public int generationTotalNum = 500;
        [Range(1, 60)] public int generationNumPerTicktime = 5;
        [Range(0.1f, 1.0f)] public float tickTime = 0.2f;
        public bool useScheduleParallel = false;

        public class Baker : Baker<RandomCubeGeneratorAuthoring>
        {
            public override void Bake(RandomCubeGeneratorAuthoring authoring)
            {
                var data = new RandomCubeGenerator
                {
                    cubeProtoType = authoring.useScheduleParallel ? GetEntity(authoring.redCubePrefab) : GetEntity(authoring.blueCubePrefab),
                    generationTotalNum = authoring.generationTotalNum,
                    generationNumPerTicktime = authoring.generationNumPerTicktime,
                    tickTime = authoring.tickTime,
                    useScheduleParallel = authoring.useScheduleParallel
                };
                AddComponent(data);
            }
        }
    }
}
