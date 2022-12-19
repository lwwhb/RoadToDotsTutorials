using Unity.Entities;
namespace DOTS.DOD.LESSON3
{
    public class Lesson3SystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(Lesson3SystemGroup))]
    public class CreateEntitiesByPrefabSystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "CreateEntitiesByPrefab";
    }
}