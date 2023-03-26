using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.DOD.LESSON7
{
    struct CubesGenerator : IComponentData
    {
        public Entity cubeProtoType;
        public int generationTotalNum;
        public int generationNumPerTicktime;
        public float tickTime;
        public float3 generatorAreaPos;
        public float3 generatorAreaSize;
        public float3 targetAreaPos;
        public float3 targetAreaSize;
        public float rotateSpeed;
        public float moveSpeed;
    }
    
    public class CubesGeneratorAuthoring : MonoBehaviour
    {
        public GameObject cubePrefab = null;
        [Range(10, 10000)] public int generationTotalNum = 500;
        [Range(1, 60)] public int generationNumPerTicktime = 5;
        [Range(0.1f, 1.0f)] public float tickTime = 0.2f;
        public float3 generatorAreaSize;
        public float3 targetAreaPos;
        public float3 targetAreaSize;
        public float rotateSpeed = 180.0f;
        public float moveSpeed = 5.0f;

        public class Baker : Baker<CubesGeneratorAuthoring>
        {
            public override void Bake(CubesGeneratorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var data = new CubesGenerator
                {
                    cubeProtoType = GetEntity(authoring.cubePrefab, TransformUsageFlags.Dynamic),
                    generationTotalNum = authoring.generationTotalNum,
                    generationNumPerTicktime = authoring.generationNumPerTicktime,
                    tickTime = authoring.tickTime,
                    generatorAreaPos = authoring.transform.position,
                    generatorAreaSize = authoring.generatorAreaSize,
                    targetAreaPos = authoring.targetAreaPos,
                    targetAreaSize = authoring.targetAreaSize,
                    rotateSpeed = authoring.rotateSpeed,
                    moveSpeed = authoring.moveSpeed

                };
                AddComponent(entity, data);
            }
        }
    }
}
