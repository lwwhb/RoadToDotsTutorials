using Unity.Burst;
using Unity.Entities;
using Unity.Scenes;

namespace DOTS.DOD.LESSON10
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(EntityRespawnSystemGroup))]
    public partial struct EntityRespawnSystem : ISystem, ISystemStartStop
    {
        private int index;
        private float timer;
        private Entity controllerEntity;
        private Entity instanceEntity;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RespawnController>();
        }
        

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!controllerEntity.Equals(default))
            {
                if (state.EntityManager.HasComponent<PrefabLoadResult>(controllerEntity))
                {
                    if(!instanceEntity.Equals(default))
                        state.EntityManager.DestroyEntity(instanceEntity);
                    var data = state.EntityManager.GetComponentData<PrefabLoadResult>(controllerEntity);
                    instanceEntity = state.EntityManager.Instantiate(data.PrefabRoot);
                    state.EntityManager.DestroyEntity(controllerEntity);
                    timer = 0;
                }
                
                var controller = SystemAPI.GetSingleton<RespawnController>();
                timer += SystemAPI.Time.DeltaTime;
                if (timer >= controller.timer)
                {
                    var prefabs = SystemAPI.GetSingletonBuffer<PrefabBufferElement>(true);
                    state.EntityManager.AddComponentData<RequestEntityPrefabLoaded>(controllerEntity, new RequestEntityPrefabLoaded
                    {
                        Prefab = prefabs[index % prefabs.Length].prefab
                    });
                    index++;
                    timer = 0;
                }
            }
        }
        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            index = 0;
            timer = 0;
            controllerEntity = default;
            instanceEntity = default;
            var prefabs = SystemAPI.GetSingletonBuffer<PrefabBufferElement>(true);
            controllerEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData<RequestEntityPrefabLoaded>(controllerEntity, new RequestEntityPrefabLoaded
            {
                Prefab = prefabs[index % prefabs.Length].prefab
            });
            state.EntityManager.AddComponent<RespawnCleanupComponent>(controllerEntity);
            index++;
        }
        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            if (!instanceEntity.Equals(default))
            {
                state.EntityManager.DestroyEntity(instanceEntity);
                instanceEntity = default;
            }

            if (!controllerEntity.Equals(default))
            {
                state.EntityManager.DestroyEntity(controllerEntity);
                index = 0;
                timer = 0;
                controllerEntity = default;
            }
        }
    }
}
