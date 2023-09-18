using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    // Obstacle Bucket
    [ChunkSerializable]
    public struct Bucket: IBufferElementData
    {
        public UnsafeList<float2> obstacles;
    }
    public struct LevelSettings : IComponentData
    {
        public float mapSize;   //地图大小
        public int colonyNum;   //蚂蚁群落个数
        public float sizeScale; //缩放比例
        
        public float maxObstaclesFillRatio;   //最大障碍物填充率
        public float obstacleSize;      //障碍物大小
        public int bucketResolution;    //障碍物桶分辨率

        public Entity homePrefab;     //蚂蚁窝预制体
        public Entity obstaclePrefab;   //障碍物预制体
        public Entity resourcePrefab;   //食物点预制体
    }

    public class LevelSettingsAuthoring : MonoBehaviour
    {
        public float mapSize = 128;   //地图大小
        [Range(1, 16)]public int colonyNum = 1;   //群落个数
        [HideInInspector]public float sizeScale = 1; //缩放比例
        [HideInInspector]public float maxObstaclesFillRatio = 0.8f;    //最大障碍物填充率
        [HideInInspector]public float obstacleSize = 2.0f;      //障碍物大小
        [HideInInspector]public int bucketResolution = 64;      //障碍物桶分辨率
        public GameObject homePrefab;
        public GameObject obstaclePrefab;
        public GameObject resourcePrefab;
        public class Baker : Baker<LevelSettingsAuthoring>
        {
            public override void Bake(LevelSettingsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var settings = new LevelSettings
                {
                    mapSize = authoring.mapSize,
                    colonyNum = authoring.colonyNum,
                    sizeScale = authoring.sizeScale,
                    maxObstaclesFillRatio = authoring.maxObstaclesFillRatio,
                    obstacleSize = authoring.obstacleSize,
                    bucketResolution = authoring.bucketResolution,
                    homePrefab = GetEntity(authoring.homePrefab, TransformUsageFlags.Renderable),
                    obstaclePrefab = GetEntity(authoring.obstaclePrefab, TransformUsageFlags.Renderable),
                    resourcePrefab = GetEntity(authoring.resourcePrefab, TransformUsageFlags.Renderable)
                };
                AddComponent<LevelSettings>(entity, settings);
                AddBuffer<Bucket>(entity);
            }
        }
    }
}
