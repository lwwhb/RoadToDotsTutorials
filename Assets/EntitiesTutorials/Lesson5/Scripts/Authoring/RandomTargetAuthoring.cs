using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

struct RandomTarget : IComponentData
{
    public float3 targetPos;
}

public class RandomTargetAuthoring : MonoBehaviour
{
    public class Baker : Baker<RandomTargetAuthoring>
    {
        public override void Bake(RandomTargetAuthoring authoring)
        {
            var data = new RandomTarget
            {  
                targetPos = float3.zero
            };
            AddComponent(data);
        }
    }
}
