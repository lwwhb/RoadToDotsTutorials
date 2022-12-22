using Unity.Entities;
namespace DOTS.DOD.LESSON8
{
    public class Lesson8SystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(Lesson8SystemGroup))]
    public class CreateEntitiesByScriptsSystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "CreateEntitiesByScripts";
    }
    
}
