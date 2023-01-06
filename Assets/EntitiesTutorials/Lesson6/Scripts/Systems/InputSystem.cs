using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.DOD.LESSON6
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(Lesson6SystemGroups))]
    [UpdateAfter(typeof(MoveCubesWithWayPointsSystem))]
    public partial struct InputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WayPoint>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DynamicBuffer<WayPoint> path = SystemAPI.GetSingletonBuffer<WayPoint>();
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float3 newWayPoint = new float3(worldPos.x, worldPos.y, 0);
                if (path.Length > 0)
                {
                    float mindist = float.MaxValue;
                    int index = path.Length;
                    for (int i = 0; i < path.Length; i++)
                    {
                        float dist = math.distance(path[i].point, newWayPoint);
                        if (dist < mindist)
                        {
                            mindist = dist;
                            index = i;
                        }
                    }
                    path.Insert(index, new WayPoint(){ point = newWayPoint });
                }
            }
        }
    }
}
