using Unity.Entities;

namespace DOTS.DOD.LESSON1
{
    public class Lesson1SystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(Lesson1SystemGroup))]
    public class RotateCubesFilterSystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "RotateCubesFilter";
    }
    
}