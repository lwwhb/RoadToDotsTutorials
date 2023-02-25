using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace DOTS.DOD.LESSON10
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(GameObjectRespawnSystemGroup))]
    public partial class GameObjectRespawnSystem : SystemBase
    {
        private int index = 0;
        private float timer = 0;
        private GameObject obj = null;
        protected override void OnUpdate()
        {
            foreach (var grc in SystemAPI.Query<GameObjectRespawnController>())
            {
                if(obj == null)
                    obj = GameObject.Instantiate(grc.prefab[index%grc.prefab.Length]);
                timer += SystemAPI.Time.DeltaTime;
                if (timer >= grc.timer)
                {
                    GameObject.Destroy(obj);
                    obj = null;
                    index++;
                    timer = 0;
                }
            }
        }
    }
}
