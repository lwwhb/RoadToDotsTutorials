using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_WorkflowMode")]
    struct WorkflowModeFloatOverride : IComponentData
    {
        public float Value;
    }
}
