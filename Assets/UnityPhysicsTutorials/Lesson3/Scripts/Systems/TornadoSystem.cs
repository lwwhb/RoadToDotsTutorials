using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DOTS.PHYSICS.LESSON3
{
    [BurstCompile]
    struct SetTornadoSpeedJob : IJacobiansJob
    {
        [ReadOnly] public ComponentLookup<TornadoComponent> mTornadoComponent;
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<RigidBody> rigidBodies;
        public void Execute(ref ModifiableJacobianHeader jacHeader, ref ModifiableTriggerJacobian tiggerJacobian) {}

        public void Execute(ref ModifiableJacobianHeader jacHeader, ref ModifiableContactJacobian contactJacobian)
        {
            if (!jacHeader.HasSurfaceVelocity) 
                return;

            float3 linearVelocity = float3.zero;
            float3 angularVelocity = float3.zero;
            
            for (int i = 0; i < 2; i++)
            {
                var entity = (i == 0) ? jacHeader.EntityA : jacHeader.EntityB;
                if (!mTornadoComponent.HasComponent(entity)) 
                    continue;

                var index = (i == 0) ? jacHeader.BodyIndexA : jacHeader.BodyIndexB;
                var rotation = rigidBodies[index].WorldFromBody.rot;
                var tornado = mTornadoComponent[entity];

                // 假设围绕接触点法线旋转
                var av = contactJacobian.Normal * tornado.AngularSpeed;

                // 计算线速度
                var otherIndex = (i == 0) ? jacHeader.BodyIndexB : jacHeader.BodyIndexA;
                var offset = rigidBodies[otherIndex].WorldFromBody.pos - rigidBodies[index].WorldFromBody.pos;
                var lv = math.cross(av, offset);

                angularVelocity += av;
                linearVelocity += lv-offset*3;
            }

            // 添加额外速度
            jacHeader.SurfaceVelocity = new SurfaceVelocity
            {
                LinearVelocity = jacHeader.SurfaceVelocity.LinearVelocity + linearVelocity,
                AngularVelocity = jacHeader.SurfaceVelocity.AngularVelocity + angularVelocity,
            };
        }
    }
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSimulationGroup))]
    [UpdateBefore(typeof(PhysicsSolveAndIntegrateGroup))]
    [UpdateAfter(typeof(PhysicsCreateJacobiansGroup))]
    public partial struct TornadoSystem : ISystem
    {
        private ComponentLookup<TornadoComponent> mTornadoComponentData;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //state.Enabled = false;
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

            state.Dependency = new SetTornadoSpeedJob
            {
                mTornadoComponent = mTornadoComponentData,
                rigidBodies = world.Bodies
            }.Schedule(simulation, ref world, state.Dependency);
        }
    }
}
