using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using Unity.Physics.Systems;

namespace DOTS.PHYSICS.LESSON3
{
    [BurstCompile]
    public partial struct ApplyImpulseJob : IJobEntity
    {
        public float DeltaTime;
        public float3 ImpulseDir;

        public void Execute(RigidBodyAspect rigidBodyAspect)
        {
            float3 impulse = -ImpulseDir;
            impulse *= DeltaTime;
            rigidBodyAspect.ApplyAngularImpulseWorldSpace(impulse);
        }
    }
    
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    public partial struct ApplyImpulseSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //state.Enabled = true;
            state.RequireForUpdate<TornadoComponent>();
            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            if (simulation.Type == SimulationType.NoPhysics)
                return;
            TornadoComponent tornado = SystemAPI.GetSingleton<TornadoComponent>();
            state.Dependency = new ApplyImpulseJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                ImpulseDir = tornado.MoveDirection
            }.Schedule(state.Dependency);
        }
    }
}
