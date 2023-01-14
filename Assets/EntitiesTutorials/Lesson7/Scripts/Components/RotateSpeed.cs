using Unity.Entities;
using UnityEngine;

namespace DOTS.DOD.LESSON7
{
    struct RotateSpeed : IComponentData, IEnableableComponent
    {
        public float rotateSpeed;
    }
}

