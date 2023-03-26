using DOTS.DOD;

namespace DOTS.DOD.LESSON10
{
    public partial class EntityRespawnSystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "EntityRespawnByCleanupComponent";
    }
    
    public partial class GameObjectRespawnSystemGroup : AuthoringSceneSystemGroup
    {
        protected override string AuthoringSceneName => "GameObjectRespawnByScript";
    }
}

