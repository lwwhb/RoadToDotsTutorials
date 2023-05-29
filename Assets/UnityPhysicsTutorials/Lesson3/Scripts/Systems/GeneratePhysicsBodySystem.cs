using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace DOTS.PHYSICS.LESSON3
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    public partial struct GeneratePhysicsBodySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MultiPhysicsBodyGenerator>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var generator = SystemAPI.GetSingleton<MultiPhysicsBodyGenerator>();
            var redCubes = CollectionHelper.CreateNativeArray<Entity>((int)(generator.redGridNums.x*generator.redGridNums.y*generator.redGridNums.z), Allocator.TempJob);
            state.EntityManager.Instantiate(generator.redCubeEntityProtoTypes, redCubes);
            var greenCubes = CollectionHelper.CreateNativeArray<Entity>((int)(generator.greenGridNums.x*generator.greenGridNums.y*generator.greenGridNums.z), Allocator.TempJob);
            state.EntityManager.Instantiate(generator.greenCubeEntityProtoTypes, greenCubes);
            var blueCubes = CollectionHelper.CreateNativeArray<Entity>((int)(generator.blueGridNums.x*generator.blueGridNums.y*generator.blueGridNums.z), Allocator.TempJob);
            state.EntityManager.Instantiate(generator.blueCubeEntityProtoTypes, blueCubes);
            int count = 0;
            foreach (var cube in redCubes)
            {
                int y = count / (int)(generator.redGridNums.x*generator.redGridNums.z)+ 50;
                int temp = count % (int)(generator.redGridNums.x*generator.redGridNums.z);
                int z = temp / (int)(generator.redGridNums.x) - (int)generator.redGridNums.z;
                int x = temp % (int)(generator.redGridNums.x) - (int)generator.redGridNums.z;
                var position = new float3(x, y, z)*generator.space;
                var transform = SystemAPI.GetComponentRW<LocalTransform>(cube);
                transform.ValueRW.Position = position;
                count++;
            }
            redCubes.Dispose();
            
            count = 0;
            foreach (var cube in greenCubes)
            {
                int y = count / (int)(generator.redGridNums.x*generator.redGridNums.z)+ 50 + (int)generator.redGridNums.y;
                int temp = count % (int)(generator.redGridNums.x*generator.redGridNums.z);
                int z = temp / (int)(generator.redGridNums.x) - (int)generator.redGridNums.z;
                int x = temp % (int)(generator.redGridNums.x) - (int)generator.redGridNums.z;
                var position = new float3(x, y, z)*generator.space;
                var transform = SystemAPI.GetComponentRW<LocalTransform>(cube);
                transform.ValueRW.Position = position;
                count++;
            }
            greenCubes.Dispose();
            
            count = 0;
            foreach (var cube in blueCubes)
            {
                int y = count / (int)(generator.redGridNums.x*generator.redGridNums.z)+ 50 + (int)generator.redGridNums.y + (int)generator.greenGridNums.y;
                int temp = count % (int)(generator.redGridNums.x*generator.redGridNums.z);
                int z = temp / (int)(generator.redGridNums.x) - (int)generator.redGridNums.z;
                int x = temp % (int)(generator.redGridNums.x) - (int)generator.redGridNums.z;
                var position = new float3(x, y, z)*generator.space;
                var transform = SystemAPI.GetComponentRW<LocalTransform>(cube);
                transform.ValueRW.Position = position;
                count++;
            }
            blueCubes.Dispose();
            
            state.Enabled = false;
        }
    }
}
