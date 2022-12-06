using Unity.Entities;
namespace DOTS.DOD.LESSON1
{
    [DisableAutoCreation]
    public class Lesson1SystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(Lesson1SystemGroup))]
    public class CreateEntitiesByPrefabSystemGroup : SceneSystemGroup
    {
        protected override string SceneName => "CreateEntitiesByPrefab";
    }
}