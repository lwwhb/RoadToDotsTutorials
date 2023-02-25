using DOTS.DOD.LESSON9;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

namespace DOTS.DOD.LESSON10
{
    struct RespawnController : IComponentData
    {
        public float timer;
    }

    struct PrefabBufferElement : IBufferElementData
    {
        public EntityPrefabReference prefab;
    }

    public class RespawnControllerAuthoring : MonoBehaviour
    {
        public GameObject[] spawners = null;
        [Range(1, 5)]public float timer = 3.0f;
        public class Baker : Baker<RespawnControllerAuthoring>
        {
            public override void Bake(RespawnControllerAuthoring authoring)
            {
                var data = new RespawnController
                {
                    timer = authoring.timer
                };
                AddComponent(data);
                var buffer = AddBuffer<PrefabBufferElement>();
                for (int i = 0; i < authoring.spawners.Length; i++)
                {
                    var elem = new PrefabBufferElement
                    {
                        prefab = new EntityPrefabReference(authoring.spawners[i])
                    };
                    buffer.Add(elem);
                }
            }
        }
    }
}
