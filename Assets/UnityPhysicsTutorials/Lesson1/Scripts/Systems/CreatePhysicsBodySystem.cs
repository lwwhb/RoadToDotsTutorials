using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.PHYSICS.LESSON0
{
    [BurstCompile]
    public struct SpawnJob : IJobParallelFor
    {
        public Entity prototype;
        public NativeArray<Entity> cubes;
        public int halfCountX;
        public int halfCountZ;
        public float space;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute(int index)
        {
            cubes[index] = Ecb.Instantiate(index, prototype);
            Ecb.SetComponent(index, cubes[index], new LocalToWorld { Value = ComputeTransform(index) });
        }

        public float4x4 ComputeTransform(int index)
        {
            int y = index / (halfCountX * halfCountZ * 4) + 50;
            int temp = index % (halfCountX * halfCountZ * 4);
            int z = temp / (halfCountX * 2) - halfCountZ;
            int x = temp % (halfCountX * 2) - halfCountX;
            
            float4x4 M = float4x4.TRS(
                new float3(x, y, z)*space,
                quaternion.identity,
                new float3(1));
            return M;
        }
    }
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    public partial struct CreatePhysicsBodySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsBodyGenerator>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var generator = SystemAPI.GetSingleton<PhysicsBodyGenerator>();
            var cubes = CollectionHelper.CreateNativeArray<Entity>((int)(generator.gridNum.x*generator.gridNum.y*generator.gridNum.z), Allocator.TempJob);
            state.EntityManager.Instantiate(generator.cubeEntityProtoType, cubes);
            int count = 0;
            foreach (var cube in cubes)
            {
                int y = count / (int)(generator.gridNum.x*generator.gridNum.z)+ 50;
                int temp = count % (int)(generator.gridNum.x*generator.gridNum.z);
                int z = temp / (int)(generator.gridNum.x) - (int)generator.gridNum.z;
                int x = temp % (int)(generator.gridNum.x) - (int)generator.gridNum.z;
                var position = new float3(x, y, z)*generator.space;
                var transform = SystemAPI.GetComponentRW<LocalTransform>(cube);
                transform.ValueRW.Position = position;
                count++;
            }
            cubes.Dispose();
            state.Enabled = false;
        }
    }
}
