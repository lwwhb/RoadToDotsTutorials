using DOTS.DOD;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

namespace DOTS.DOD
{
    public struct RandomSingleton : IComponentData
    {
        public Random random;
    }

    public class RandomSingletonAuthoring : Singleton<RandomSingletonAuthoring>
    {
        public uint seed = 1;
        public class Baker : Baker<RandomSingletonAuthoring>
        {
            public override void Bake(RandomSingletonAuthoring authoring)
            {
                var data = new RandomSingleton
                {
                    random = new Random(RandomSingletonAuthoring.Instance.seed)
                };
                AddComponent(data);
            }
        }
    }
}
