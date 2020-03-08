using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;

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
        var query = new EntityQueryDesc{
            All = new ComponentType[]{typeof(PhysicsCollider)}
        };
        m_query = GetEntityQuery(query);
    }

    private struct CollisionJob : ICollisionEventsJob
    {
        // A Rigidbody colided with something
        public void Execute(CollisionEvent collisionEvent)
        {
            Debug.Log("Collision! Entity A: " + collisionEvent.Entities.EntityA + " and Entity B: " + collisionEvent.Entities.EntityB);
        }
    }

    protected override void OnUpdate()
    {
        var physicsWorld = buildPhysicsWorld.PhysicsWorld;
        var collisionJob = new CollisionJob()
        {
        }.Schedule<CollisionJob>(m_query, JobHandle);

        collisionJob.Complete();
        return inputDeps;
    }
}