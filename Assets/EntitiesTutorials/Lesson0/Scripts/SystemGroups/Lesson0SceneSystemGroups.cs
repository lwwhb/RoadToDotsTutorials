using Unity.Entities;

namespace DOTS.DOD.LESSON0
{
    public class Lesson0SystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(Lesson0SystemGroup))]
    public class CubeRotateSystemGroup : SceneSystemGroup
    {
        protected override string SceneName => "RotateCubeAuthoring";
    }
    
    [UpdateInGroup(typeof(Lesson0SystemGroup))]
    public class CubeRotateWithIJobEntitySystemGroup : SceneSystemGroup
    {
        protected override string SceneName => "RotateCubeWithIJobEntityAuthoring";
    }
}
