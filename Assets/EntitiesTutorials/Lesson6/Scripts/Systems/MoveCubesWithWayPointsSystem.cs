using DOTS.DOD.LESSON0;
using DOTS.DOD.LESSON3;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.DOD.LESSON6
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(Lesson6SystemGroups))]
    public partial struct MoveCubesWithWayPointsSystem : ISystem
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

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            DynamicBuffer<WayPoint> path = SystemAPI.GetSingletonBuffer<WayPoint>();
            float deltaTime = SystemAPI.Time.DeltaTime;
            if (!path.IsEmpty)
            {
                foreach (var (transform, nextIndex, speed) in
                         SystemAPI.Query<RefRW<LocalTransform>, RefRW<NextPathIndex>, RefRO<RotateAndMoveSpeed>>())
                {
                    float3 direction = path[(int)nextIndex.ValueRO.nextIndex].point - transform.ValueRO.Position;
                    transform.ValueRW.Position =
                        transform.ValueRO.Position + math.normalize(direction) * speed.ValueRO.moveSpeed*deltaTime;
                    transform.ValueRW = transform.ValueRO.RotateY(speed.ValueRO.rotateSpeed * deltaTime);
                    if (math.distance(path[(int)nextIndex.ValueRO.nextIndex].point, transform.ValueRO.Position) <=
                        0.02f)
                    {
                        nextIndex.ValueRW.nextIndex = (uint)((nextIndex.ValueRO.nextIndex + 1) % path.Length);
                    }
                }
            }
        }
    }
}
