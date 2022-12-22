using Unity.Entities;
using UnityEngine;

struct CubesGenerator : IComponentData
{

}

public class CubesGeneratorAuthoring : MonoBehaviour
{
    public class Baker : Baker<CubesGeneratorAuthoring>
    {
        public override void Bake(CubesGeneratorAuthoring authoring)
        {
            var data = new CubesGenerator
            {  
            
            };
            AddComponent(data);
        }
    }
}
