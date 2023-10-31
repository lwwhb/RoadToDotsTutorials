using Unity.Collections;
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
        public float maxSpeed;
        public float accel;
    }

    public struct Direction : IComponentData
    {
        public float direction; // angle Z
    }

    public struct AntBlobData
    {
        public float randomSteering;            //随机转向
        public float pheromoneSteerStrength;    //信息素转向强度
        public float pheromoneSteerDistance;    //信息素转向距离
    }
    
    public struct Ant : IComponentData
    {
        public float wallSteering;      //墙壁的转向
        public float pheroSteering;     //信息素的转向
        public float resourceSteering;  //食物的转向
        public bool hasResource;        //是否已经携带食物
    }
    
    public struct AntProperties : IComponentData
    {
        public BlobAssetReference<AntBlobData> blobData;        //蚂蚁公用数据
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
                var blobReference = CreateAntBlobData();
                AddBlobAsset(ref blobReference, out var hash);
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<AntProperties>(entity, new AntProperties{ blobData = blobReference });
                AddComponent<Ant>(entity, authoring.ant);
                AddComponent<Position>(entity, authoring.position);
                AddComponent<Speed>(entity, authoring.speed);
                AddComponent<Direction>(entity, authoring.direction);
                AddSharedComponent<ColonyID>(entity, new ColonyID { id = -1 });
                AddComponent(entity, new URPMaterialPropertyBaseColor{ Value = new float4(0,0,1, 1) });
            }
        }

        static BlobAssetReference<AntBlobData> CreateAntBlobData()
        {
            var builder = new BlobBuilder(Allocator.Temp);
            
            ref AntBlobData antBlobData = ref builder.ConstructRoot<AntBlobData>();
            antBlobData.randomSteering = 8.0f;
            antBlobData.pheromoneSteerStrength = 0.86f;
            antBlobData.pheromoneSteerDistance = 3.0f;
            
            var result = builder.CreateBlobAssetReference<AntBlobData>(Allocator.Persistent);
            builder.Dispose();
            return result;
        }
    }
}
