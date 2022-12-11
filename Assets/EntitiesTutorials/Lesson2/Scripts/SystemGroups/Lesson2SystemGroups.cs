using Unity.Entities;

namespace DOTS.DOD.LESSON2
{
    public class Lesson2SystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(Lesson2SystemGroup))]
    public class CubeRotateWithIJobEntitySystemGroup : SceneSystemGroup
    {
        protected override string SceneName => "RotateCubeWithIJobEntity";
    }
    
    [UpdateInGroup(typeof(Lesson2SystemGroup))]
    public class CubeRotateWithIJobChunkSystemGroup : SceneSystemGroup
    {
        protected override string SceneName => "RotateCubeWithIJobChunk";
    }
}