using Unity.Entities;
namespace DOTS.DOD.LESSON5
{
    public class Lesson5SystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(Lesson5SystemGroup))]
    public class WaveCubesWithDotsSystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "WaveCubesWithDots";
    }
}