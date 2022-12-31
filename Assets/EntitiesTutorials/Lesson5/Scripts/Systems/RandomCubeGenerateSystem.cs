using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace DOTS.DOD.LESSON5
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(RandomGenerateCubesSystemGroup))]
    public partial struct RandomCubeGenerateSystem : ISystem
    {
        private float timer;
        private int totalCubes;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RandomCubeGenerator>();
            timer = 0.0f;
            totalCubes = 0;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var generator = SystemAPI.GetSingleton<RandomCubeGenerator>();
            if (totalCubes >= generator.generationTotalNum)
            {
                state.Enabled = false;
                return;
            }
            if (timer >= generator.tickTime)
            {
                CreateGenerateCubesEntityJob(ref state, generator);
                timer -= generator.tickTime;
            }
            timer += Time.deltaTime;
        }

        private void CreateGenerateCubesEntityJob(ref SystemState state, RandomCubeGenerator generator)
        {
            RefRW<RandomSingleton> random = SystemAPI.GetSingletonRW<RandomSingleton>();
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            var cubes = CollectionHelper.CreateNativeArray<Entity>(generator.generationNumPerTicktime, Allocator.TempJob);
            if(generator.useScheduleParallel)
            {
                EntityCommandBuffer.ParallelWriter ecbParallel = ecb.AsParallelWriter();
                var job = new GenerateCubesWithParallelWriterJob()
                {
                    cubeProtoType = generator.cubeProtoType,
                    cubes = cubes,
                    ecbParallel = ecbParallel,
                    random = random
                };
                state.Dependency = job.ScheduleParallel(cubes.Length, 1, state.Dependency);
            }
            else
            { 
                var job = new GenerateCubesJob
                {
                    cubeProtoType = generator.cubeProtoType,
                    cubes = cubes,
                    ecb = ecb,
                    random = random
                };
                state.Dependency = job.Schedule(cubes.Length, state.Dependency);
                
            }
            state.Dependency.Complete();
            ecb.Playback(state.EntityManager);
            totalCubes += generator.generationNumPerTicktime;
            cubes.Dispose();
            ecb.Dispose();
        }
    }
}
