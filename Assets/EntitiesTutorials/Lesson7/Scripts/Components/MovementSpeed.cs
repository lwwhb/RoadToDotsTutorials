using Unity.Entities;
using UnityEngine;

namespace DOTS.DOD.LESSON7
{
    struct MovementSpeed : IComponentData, IEnableableComponent
    {
        public float movementSpeed;
    }
}

