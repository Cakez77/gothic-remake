using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;


[DisableAutoCreation]
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
        public BufferFromEntity<BufferCollisionDetails> colBuffers;
        public PhysicsWorld physicsWorld;

        // A Rigidbody colided with something
        public void Execute(CollisionEvent collisionEvent)
        {
            // Collision occurred!!
            // TODO: Check which entity has a buffer, what if both have a buffer
            // Add the normal of the collision to the buffer of the body colliding
            // TODO: ADD a check for physics body maybe
            var playerBuffer = colBuffers[collisionEvent.Entities.EntityA];

            // Add a buffer element containing the details about the collision to the 
            // entity with the buffer
            playerBuffer.Add(new BufferCollisionDetails{
                CollisionNormal = collisionEvent.Normal,
                CollisionPoint = collisionEvent.CalculateDetails(ref physicsWorld).AverageContactPointPosition
            });
        }
    }

    protected override void OnUpdate()
    {

        var colBuffers = GetBufferFromEntity<BufferCollisionDetails>();
        var physicsWorld = buildPhysicsWorld.PhysicsWorld;
        var collisionJob = new CollisionJob()
        {
            colBuffers = colBuffers,
            physicsWorld = physicsWorld
        }.Schedule(stepPhysicsWorld.Simulation, ref physicsWorld, Dependency);

        collisionJob.Complete();
    }
}