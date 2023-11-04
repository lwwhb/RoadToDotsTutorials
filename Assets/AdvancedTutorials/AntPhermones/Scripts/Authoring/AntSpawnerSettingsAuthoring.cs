using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    public struct AntSpawnerBlobData
    {
        public float outwardStrength;           //外部强度
        public float inwardStrength;            //内部强度
    }
    public struct AntSpawnerSettings : IComponentData
    {
        public Entity antPrefab;    //蚂蚁预制体
        public int antCount;        //蚂蚁数量
        public float antScale;      //蚂蚁大小
        public float antMaxSpeed;   //蚂蚁最大速度
        public float antAccel;      //蚂蚁加速度
        
        public int ringCount;       //障碍物环数

        public BlobAssetReference<AntSpawnerBlobData> blobData;
    }

    public class AntSpawnerSettingsAuthoring : MonoBehaviour
    {
        public GameObject antPrefab = null; //蚂蚁预制体
        public int antCount = 1000;         //蚂蚁数量
        public float antScale = 1;          //蚂蚁大小
        public float antMaxSpeed = 0.2f;    //蚂蚁最大速度
        public float antAccel = 0.07f;      //蚂蚁加速度
        
        public int ringCount = 3;           //障碍物环数
        public class Baker : Baker<AntSpawnerSettingsAuthoring>
        {
            public override void Bake(AntSpawnerSettingsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                var blobData = CreateAntSpawnerBlobData();
                AddBlobAsset(ref blobData, out var hash);
                var settings = new AntSpawnerSettings
                {
                    antPrefab = GetEntity(authoring.antPrefab, TransformUsageFlags.Renderable),
                    antCount = authoring.antCount,
                    antScale = authoring.antScale,
                    antMaxSpeed = authoring.antMaxSpeed,
                    antAccel = authoring.antAccel,
                    
                    ringCount = authoring.ringCount,
                    
                    blobData = blobData
                };
                AddComponent(entity, settings);
            }
            
            BlobAssetReference<AntSpawnerBlobData> CreateAntSpawnerBlobData()
            {
                var builder = new BlobBuilder(Allocator.Temp);

                ref AntSpawnerBlobData spawnerBlobData = ref builder.ConstructRoot<AntSpawnerBlobData>();
                
                spawnerBlobData.outwardStrength = 0.5f;
                spawnerBlobData.inwardStrength = 0.5f;

                var result = builder.CreateBlobAssetReference<AntSpawnerBlobData>(Allocator.Persistent);
                builder.Dispose();
                return result;
            }
        }
    }
}
