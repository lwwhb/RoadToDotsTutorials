using DOTS.DOD;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine.SceneManagement;

namespace DOTS.DOD.GRAPHICS
{
    public abstract class SceneSystemGroup : ComponentSystemGroup
    {
        private bool initialized;
        protected override void OnCreate()
        {
            base.OnCreate();
            initialized = false;
            
        }

        protected override void OnUpdate()
        {
            if (!initialized)
            {
                if (SceneManager.GetActiveScene().isLoaded)
                {
                    
                    Enabled = SceneName == SceneManager.GetActiveScene().name;
                    initialized = true;
                }
            }
            base.OnUpdate();
        }

        protected abstract string SceneName { get; }
    }
}

