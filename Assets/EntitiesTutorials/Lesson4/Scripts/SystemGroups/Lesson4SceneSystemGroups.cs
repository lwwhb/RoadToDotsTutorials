using Unity.Entities;
namespace DOTS.DOD.LESSON4
{
    public class Lesson4SystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(Lesson4SystemGroup))]
    public class WaveCubesWithDotsSystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "WaveCubesWithDots";
    }
}