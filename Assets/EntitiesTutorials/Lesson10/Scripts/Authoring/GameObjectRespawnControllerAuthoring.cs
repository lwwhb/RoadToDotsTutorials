using Unity.Entities;
using UnityEngine;

namespace DOTS.DOD.LESSON10
{
    class GameObjectRespawnController : IComponentData
    {
        public float timer;
        public GameObject[] prefab;
    }

    public class GameObjectRespawnControllerAuthoring : MonoBehaviour
    {
        public GameObject[] spawners = null;
        [Range(1, 5)] public float timer = 3.0f;

        public class Baker : Baker<GameObjectRespawnControllerAuthoring>
        {
            public override void Bake(GameObjectRespawnControllerAuthoring authoring)
            {
                var data = new GameObjectRespawnController
                {
                    timer = authoring.timer,
                    prefab = authoring.spawners
                };
                AddComponentObject(data);
            }
        }
    }
}