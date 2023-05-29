using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace DOTS.PHYSICS.LESSON3
{
    [BurstCompile]
    public partial struct CountNumCollisionEvents : ICollisionEventsJob
    {
        public NativeReference<int> NumCollisionEvents;
        public void Execute(CollisionEvent collisionEvent)
        {
            NumCollisionEvents.Value++;
        }
    }
    
    [BurstCompile]
    public partial struct CountNumTriggerEvents : ITriggerEventsJob
    {
        public NativeReference<int> NumTriggerEvents;
        public void Execute(TriggerEvent collisionEvent)
        {
            NumTriggerEvents.Value++;
        }
    }
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial struct GetPhysicsEventsSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            NativeReference<int> numCollisionEvents = new NativeReference<int>(0, Allocator.TempJob);
            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            if (simulation.Type == SimulationType.NoPhysics)
                return;
            state.Dependency = new CountNumCollisionEvents
            {
                NumCollisionEvents = numCollisionEvents
            }.Schedule(simulation, state.Dependency);
            state.Dependency.Complete();
            Debug.Log("Current Frame Collision Events Number: " + numCollisionEvents.Value);
            
            NativeReference<int> numTriggerEvents = new NativeReference<int>(0, Allocator.TempJob);

            state.Dependency = new CountNumTriggerEvents
            {
                NumTriggerEvents = numTriggerEvents
            }.Schedule(simulation, state.Dependency);
            state.Dependency.Complete();
            Debug.Log("Current Frame Trigger Events Number: " + numTriggerEvents.Value);
        }
    }
}
