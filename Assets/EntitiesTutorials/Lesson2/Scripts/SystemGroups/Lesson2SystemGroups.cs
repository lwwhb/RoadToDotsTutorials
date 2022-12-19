using Unity.Entities;

namespace DOTS.DOD.LESSON2
{
    public class Lesson2SystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(Lesson2SystemGroup))]
    public class CubeRotateWithIJobEntitySystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "RotateCubeWithIJobEntity";
    }
    
    [UpdateInGroup(typeof(Lesson2SystemGroup))]
    public class CubeRotateWithIJobChunkSystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "RotateCubeWithIJobChunk";
    }
}