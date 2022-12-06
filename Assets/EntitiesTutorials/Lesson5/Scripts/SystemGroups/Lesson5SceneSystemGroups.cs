using Unity.Entities;
namespace DOTS.DOD.LESSON5
{
    [DisableAutoCreation]
    public class Lesson5SystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(Lesson5SystemGroup))]
    public class WaveCubesWithDotsSystemGroup : SceneSystemGroup
    {
        protected override string SceneName => "WaveCubesWithDots";
    }
}