using DOTS.DOD;

namespace DOTS.DOD.LESSON10
{
    public class EntityRespawnSystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "EntityRespawnByCleanupComponent";
    }
    
    public class GameObjectRespawnSystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "GameObjectRespawnByScript";
    }
}

