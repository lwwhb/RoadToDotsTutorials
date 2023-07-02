using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    public struct Position : IComponentData
    {
        public float2 position;
    }

    public struct Speed : IComponentData
    {
        public float speed;
    }

    public struct Direction : IComponentData
    {
        public float direction; // angle Z
    }

    public struct Ant : IComponentData
    {
        public float wallSteering;      //墙壁的转向
        public float pheroSteering;     //信息素的转向
        public float resourceSteering;  //食物的转向
        public bool hasResource;        //是否已经携带食物
    }

    public class AntAuthoring : MonoBehaviour
    {
        public Ant ant;
        public Position position;
        public Speed speed;
        public Direction direction;

        public class Baker : Baker<AntAuthoring>
        {
            public override void Bake(AntAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent<Ant>(entity, authoring.ant);
                AddComponent<Position>(entity, authoring.position);
                AddComponent<Speed>(entity, authoring.speed);
                AddComponent<Direction>(entity, authoring.direction);
                AddSharedComponent<ColonyID>(entity, new ColonyID { id = -1 });
                AddComponent(entity, new URPMaterialPropertyBaseColor{ Value = new float4(0,0,1, 1) });
            }
        }
    }
}
