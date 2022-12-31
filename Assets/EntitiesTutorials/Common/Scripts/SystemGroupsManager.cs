using Unity.Entities;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DOTS.DOD
{
    public abstract class AuthoringSceneSystemGroup : ComponentSystemGroup
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
                    var subScene = Object.FindObjectOfType<SubScene>();
                    if (subScene != null)
                    {
                        Enabled = AuthoringSceneName == subScene.gameObject.scene.name;
                    }
                    else
                    {
                        Enabled = false;
                    }
                    initialized = true;
                }
            }
            base.OnUpdate();
        }

        protected abstract string AuthoringSceneName { get; }
    }
}



