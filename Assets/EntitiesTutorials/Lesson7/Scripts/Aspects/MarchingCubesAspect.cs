using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.DOD.LESSON7
{
    readonly partial struct MarchingCubesAspect : IAspect
    {
        readonly RefRW<LocalTransform> localTransform;
        readonly RefRO<MovementSpeed> moveSpeed;
        readonly RefRO<RandomTarget> targetPos;

        public bool IsNeedDestroy()
        {
            var distance = math.distance(localTransform.ValueRO.Position, targetPos.ValueRO.targetPos);
            if (distance < 0.02f)
                return true;
            else
                return false;
        }
        public void Move(float deltaTime)
        {
            float3 dir =  math.normalize(targetPos.ValueRO.targetPos - localTransform.ValueRO.Position);
            localTransform.ValueRW.Position += dir * moveSpeed.ValueRO.movementSpeed * deltaTime;
        }
    }
}
