using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AntPhermonesSystemGroup))]
    [CreateAfter(typeof(AntAISystem))]
    [UpdateAfter(typeof(AntAISystem))]
    public partial struct RenderingSystem : ISystem, ISystemStartStop
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LevelSettings>();
            state.RequireForUpdate<Pheromone>();
            
        }
        
        public void OnUpdate(ref SystemState state)
        {
            // 绘制蚂蚁颜色
            var antRenderingJob = new AntRenderingJob();
            state.Dependency = antRenderingJob.Schedule(state.Dependency);
            
            
            // 绘制信息素
            var gameObject = GameObject.Find("PheromoneRenderer");
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            var material = meshRenderer.material;
            var texture2D = material.mainTexture as Texture2D;

            var pheromones = SystemAPI.GetSingletonBuffer<Pheromone>();
            texture2D.SetPixelData(pheromones.AsNativeArray(), 0, 0);
            texture2D.Apply();
        }

        public void OnStartRunning(ref SystemState state)
        {
            var colony = SystemAPI.GetSingleton<LevelSettings>();
            int mapSize = (int)colony.mapSize;
            var gameObject = GameObject.Find("PheromoneRenderer");
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            var material = meshRenderer.material;
            var texture2D = new Texture2D(mapSize,mapSize, TextureFormat.RFloat, false);
            material.mainTexture = texture2D;
        }

        public void OnStopRunning(ref SystemState state)
        {
            
        }
    }
}