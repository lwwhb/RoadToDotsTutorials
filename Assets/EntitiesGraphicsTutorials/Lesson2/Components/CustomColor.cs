using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace DOTS.DOD.GRAPHICS.LESSON2
{
    [MaterialProperty("_BaseColor")]
    struct CustomColor : IComponentData
    {
        public float4 customColor;
    }
}

