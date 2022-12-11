using Unity.Entities;

namespace DOTS.DOD.LESSON1
{
    public class Lesson1SystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(Lesson1SystemGroup))]
    public class RotateCubesFilterSystemGroup : SceneSystemGroup
    {
        protected override string SceneName => "RotateCubesFilter";
    }
    
}