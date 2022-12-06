using Unity.Entities;
namespace DOTS.DOD.LESSON4
{
    [DisableAutoCreation]
    public class Lesson4SystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(Lesson4SystemGroup))]
    public class CreateEntitiesByScriptsSystemGroup : SceneSystemGroup
    {
        protected override string SceneName => "CreateEntitiesByScripts";
    }
    
}
