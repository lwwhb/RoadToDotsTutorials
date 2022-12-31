using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.DOD.LESSON3
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateAfter(typeof(CubeGenerateByPrefabSystem))]
    [UpdateInGroup(typeof(CreateEntitiesByPrefabSystemGroup))]
    partial struct CubeRotateAndMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            state.RequireForUpdate<RotateAndMoveSpeed>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            double elapsedTime = SystemAPI.Time.ElapsedTime;
            
            // 1.Use Component Ref
            /*foreach (var (transform, speed) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateAndMoveSpeed>>())
            {
                transform.ValueRW = transform.ValueRO.RotateY(speed.ValueRO.rotateSpeed * deltaTime);
                transform.ValueRW.Position.y = (float)math.sin(elapsedTime * speed.ValueRO.moveSpeed);
            }*/
            
            // 2.Use RotateAndMoveAspect
            foreach (var aspect in SystemAPI.Query<RotateAndMoveAspect>())
            {
                //aspect.Move(elapsedTime);
                //aspect.Rotate(deltaTime);
                aspect.RotateAndMove(elapsedTime, deltaTime);
            }
        }
    }
}
