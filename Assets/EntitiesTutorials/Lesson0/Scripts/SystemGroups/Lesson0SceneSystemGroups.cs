using Unity.Entities;

namespace DOTS.DOD.LESSON0
{
    public class Lesson0SystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(Lesson0SystemGroup))]
    public class CubeRotateSystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "RotateCubeAuthoring";
    }
}
