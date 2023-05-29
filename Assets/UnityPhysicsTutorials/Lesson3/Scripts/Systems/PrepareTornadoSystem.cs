using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DOTS.PHYSICS.LESSON3
{
    [BurstCompile]
    struct SetTornadoFlagJob : IContactsJob
    {
        [ReadOnly]
        public ComponentLookup<TornadoComponent> mTornadoComponent;

        public void Execute(ref ModifiableContactHeader manifold, ref ModifiableContactPoint contact)
        {
            if (mTornadoComponent.HasComponent(manifold.EntityA) || mTornadoComponent.HasComponent(manifold.EntityB))
            {
                manifold.JacobianFlags |= JacobianFlags.EnableSurfaceVelocity;
                manifold.JacobianFlags |= JacobianFlags.EnableCollisionEvents;
            }
        }
    }
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSimulationGroup))]
    [UpdateAfter(typeof(PhysicsCreateContactsGroup))]
    [UpdateBefore(typeof(PhysicsCreateJacobiansGroup))]
    public partial struct PrepareTornadoSystem : ISystem
    {
        private ComponentLookup<TornadoComponent> mTornadoComponentData;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate(state.GetEntityQuery(ComponentType.ReadOnly<TornadoComponent>()));
            mTornadoComponentData = state.GetComponentLookup<TornadoComponent>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            mTornadoComponentData.Update(ref state);
            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            if (simulation.Type == SimulationType.NoPhysics)
                return;
            var world = SystemAPI.GetSingletonRW<PhysicsWorldSingleton>().ValueRW.PhysicsWorld;
            state.Dependency = new SetTornadoFlagJob
            {
                mTornadoComponent = mTornadoComponentData
            }.Schedule(simulation, ref world, state.Dependency);
        }
    }
}
