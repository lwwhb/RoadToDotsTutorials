using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.DOD.LESSON3
{
    readonly partial struct RotateAndMoveAspect : IAspect
    {
        readonly RefRW<LocalTransform> localTransform;
        readonly RefRO<RotateAndMoveSpeed> speed;

        public void Move(double elapsedTime)
        {
            localTransform.ValueRW.Position.y = (float)math.sin(elapsedTime * speed.ValueRO.moveSpeed);
        }
        public void Rotate(float deltaTime)
        {
            localTransform.ValueRW = localTransform.ValueRO.RotateY(speed.ValueRO.rotateSpeed * deltaTime);
        }

        public void RotateAndMove(double elapsedTime, float deltaTime)
        {
            localTransform.ValueRW.Position.y = (float)math.sin(elapsedTime * speed.ValueRO.moveSpeed);
            localTransform.ValueRW = localTransform.ValueRO.RotateY(speed.ValueRO.rotateSpeed * deltaTime);
        }
    }
}
