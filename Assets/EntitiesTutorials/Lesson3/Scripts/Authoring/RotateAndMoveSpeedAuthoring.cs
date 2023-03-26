using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.DOD.LESSON3
{
    struct RotateAndMoveSpeed : IComponentData
    {
        public float rotateSpeed;
        public float moveSpeed;
    }
    public class RotateAndMoveSpeedAuthoring : MonoBehaviour
    {
        [Range(0, 360)]public float rotateSpeed = 360.0f;
        [Range(0, 10)]public float moveSpeed = 1.0f;
        public class Baker : Baker<RotateAndMoveSpeedAuthoring>
        {
            public override void Bake(RotateAndMoveSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var data = new RotateAndMoveSpeed
                {
                    rotateSpeed = math.radians(authoring.rotateSpeed),
                    moveSpeed = authoring.moveSpeed
                };
                AddComponent(entity, data);
            }
        }
    }
}
