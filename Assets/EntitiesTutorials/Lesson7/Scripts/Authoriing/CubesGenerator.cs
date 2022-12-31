using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

struct CubesGenerator : IComponentData
{
    public Entity cubeProtoType;
    public int generationTotalNum;
    public int generationNumPerTicktime;
    public float tickTime;
    public float3 generatorAreaSize;
    public float3 targetAreaSize;
    public float rotateSpeed;
    public float moveSpeed;
}

[RequireComponent(typeof(BoxCollider))]
public class CubesGeneratorAuthoring : MonoBehaviour
{
    public GameObject cubePrefab = null;
    public GameObject targetArea = null;
    [Range(10, 10000)] public int generationTotalNum = 500;
    [Range(1, 60)] public int generationNumPerTicktime = 5;
    [Range(0.1f, 1.0f)] public float tickTime = 0.2f;
    [HideInInspector]
    public float3 generatorAreaSize;
    [HideInInspector]
    public float3 targetAreaSize;
    public float rotateSpeed = 180.0f;
    public float moveSpeed = 5.0f;
    public class Baker : Baker<CubesGeneratorAuthoring>
    {
        public override void Bake(CubesGeneratorAuthoring authoring)
        {
            var data = new CubesGenerator
            {  
                cubeProtoType = GetEntity(authoring.cubePrefab),
                generationTotalNum = authoring.generationTotalNum,
                generationNumPerTicktime = authoring.generationNumPerTicktime,
                tickTime = authoring.tickTime,
                generatorAreaSize = authoring.generatorAreaSize,
                targetAreaSize = authoring.targetAreaSize,
                rotateSpeed = authoring.rotateSpeed,
                moveSpeed = authoring.moveSpeed
                
            };
            AddComponent(data);
        }
    }

    private void Start()
    {
        generatorAreaSize = transform.position + GetComponent<BoxCollider>().size;
        targetAreaSize = targetArea.transform.position + targetArea.GetComponent<BoxCollider>().size;
    }
}
