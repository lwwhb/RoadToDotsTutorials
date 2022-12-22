using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.DOD.LESSON8
{
    public class CreateEntitiesByArchetype : MonoBehaviour
    {
        public Mesh cubeMesh = null;
        public Material cubeMaterial = null;
        [Range(1, 10)] public int CubeCount = 6;
        void Start()
        {
            /*EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(RenderMesh),
                typeof(LocalToWorld),
                typeof(RenderBounds)
            );

            // create 100 entities
            NativeArray<Entity> entityArray = new NativeArray<Entity>(CubeCount, Allocator.Temp);
            entityManager.CreateEntity(entityArchetype, entityArray);

            for (int i = 0; i < entityArray.Length; i++)
            {
                Entity entity = entityArray[i];

                entityManager.SetSharedComponentManaged<RenderMesh>(entity, new RenderMesh
                {
                    mesh = cubeMesh,
                    material = cubeMaterial
                }); 
            }
            entityArray.Dispose();*/
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
