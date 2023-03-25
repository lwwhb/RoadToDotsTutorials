using System.Collections.Generic;
using Unity.Entities;
using Unity.Profiling;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace DOTS.DOD.GRAPHICS.LESSON2
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(GraphicsLesson2SystemGroup))]
    [UpdateAfter(typeof(ChangeWaveCubesColorSystem))]
    public partial class ChangeWaveCubesMeshAndMaterialSystem : SystemBase
    {
        static readonly ProfilerMarker profilerMarker = new ProfilerMarker("ChangeWaveCubesMeshAndMaterialSystem");
        private Dictionary<Mesh, BatchMeshID> m_MeshMapping;
        private Dictionary<Material, BatchMaterialID> m_MaterialMapping;
        protected override void OnStartRunning()
        {
            RequireForUpdate<CustomMeshAndMaterial>();
            var entitiesGraphicsSystem = World.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();
            m_MeshMapping = new Dictionary<Mesh, BatchMeshID>();
            m_MaterialMapping = new Dictionary<Material, BatchMaterialID>();

            Entities
                .WithoutBurst()
                .ForEach((in CustomMeshAndMaterial changer) =>
                {
                    if (!m_MeshMapping.ContainsKey(changer.sphere))
                        m_MeshMapping[changer.sphere] = entitiesGraphicsSystem.RegisterMesh(changer.sphere);
                    if (!m_MeshMapping.ContainsKey(changer.capsule))
                        m_MeshMapping[changer.capsule] = entitiesGraphicsSystem.RegisterMesh(changer.capsule);
                    if (!m_MeshMapping.ContainsKey(changer.cylinder))
                        m_MeshMapping[changer.cylinder] = entitiesGraphicsSystem.RegisterMesh(changer.cylinder);
                    
                    if (!m_MaterialMapping.ContainsKey(changer.red))
                        m_MaterialMapping[changer.red] = entitiesGraphicsSystem.RegisterMaterial(changer.red);
                    if (!m_MaterialMapping.ContainsKey(changer.green))
                        m_MaterialMapping[changer.green] = entitiesGraphicsSystem.RegisterMaterial(changer.green);
                    if (!m_MaterialMapping.ContainsKey(changer.blue))
                        m_MaterialMapping[changer.blue] = entitiesGraphicsSystem.RegisterMaterial(changer.blue);
                    
                }).Run();
        }

        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((CustomMeshAndMaterial changer, ref MaterialMeshInfo info, in LocalToWorld trans) =>
                {
                    if (trans.Position.y < -5)
                    {
                        info.MeshID = m_MeshMapping[changer.cylinder];
                        info.MaterialID = m_MaterialMapping[changer.blue];
                    }
                    else if(trans.Position.y > 5)
                    {
                        info.MeshID = m_MeshMapping[changer.sphere];
                        info.MaterialID = m_MaterialMapping[changer.red];
                    }
                    else
                    {
                        info.MeshID = m_MeshMapping[changer.capsule];
                        info.MaterialID = m_MaterialMapping[changer.green];
                    }
                }).Run();
        }
    }
}
