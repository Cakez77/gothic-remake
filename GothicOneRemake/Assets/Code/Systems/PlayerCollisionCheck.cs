using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;


[UpdateBefore(typeof(PlayerMovementSystem))]
[UpdateAfter(typeof(VelocitySystem))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class PlayerCollisionCheck : SystemBase
{

    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    private EntityQuery m_query;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        var query = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(PhysicsCollider) }
        };
        m_query = GetEntityQuery(query);
    }

    private struct CollisionJob : ICollisionEventsJob
    {
        public BufferFromEntity<Float3Buffer> lookUp;

        // A Rigidbody colided with something
        public void Execute(CollisionEvent collisionEvent)
        {

            // Addiere den Index der Entity mit der der Player Kollidiert ist zum int buffer des players hinzu.
            var buffer = lookUp[collisionEvent.Entities.EntityA];
            var float3Buffer = buffer.Reinterpret<float3>();

            // The normal of the collision Event
            var normal = math.normalizesafe(collisionEvent.Normal, 0);

            // Debugging the normal vector
            // Debug.Log("The normal vector: " + normal);

            float3Buffer.Add(collisionEvent.Normal);
            // Debug.Log("Collision! Entity A: " + collisionEvent.Entities.EntityA + " and Entity B: " + collisionEvent.Entities.EntityB);
        }
    }

    protected override void OnUpdate()
    {

        var lookUp = GetBufferFromEntity<Float3Buffer>();
        var physicsWorld = buildPhysicsWorld.PhysicsWorld;
        var collisionJob = new CollisionJob()
        {
            lookUp = lookUp,
        }.Schedule(stepPhysicsWorld.Simulation, ref physicsWorld, Dependency);

        collisionJob.Complete();
    }
}