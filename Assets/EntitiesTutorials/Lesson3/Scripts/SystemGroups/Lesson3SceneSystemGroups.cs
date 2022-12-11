using Unity.Entities;
namespace DOTS.DOD.LESSON3
{
    [DisableAutoCreation]
    public class Lesson3SystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(Lesson3SystemGroup))]
    public class CreateEntitiesByPrefabSystemGroup : SceneSystemGroup
    {
        protected override string SceneName => "CreateEntitiesByPrefab";
    }
}