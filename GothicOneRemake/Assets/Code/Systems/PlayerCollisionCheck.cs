// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Physics;
// using Unity.Physics.Systems;
// using UnityEngine;
// using Unity.Mathematics;
// using Unity.Burst;
// using Unity.Transforms;

// [UpdateBefore(typeof(PlayerMovementSystem))]
// [UpdateAfter(typeof(VelocitySystem))]
// [UpdateInGroup(typeof(SimulationSystemGroup))]
// public class PlayerCollisionCheck : JobComponentSystem
// {

//     private BuildPhysicsWorld buildPhysicsWorld;
//     private StepPhysicsWorld stepPhysicsWorld;

//     protected override void OnCreate()
//     {
//         buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
//         stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
//     }

//     private struct CollisionJob : ICollisionEventsJob
//     {
//         // A Rigidbody colided with something
//         // TODO: Is a collision always ground?
//         public PhysicsWorld world;
//         public ComponentDataFromEntity<LocalToWorld> LocalToWorld;
//         public ComponentDataFromEntity<Velocity> velocity;


//         public void Execute(CollisionEvent collisionEvent)
//         {

//             // var normal = collisionEvent.Normal;
//             // Entity entityA = collisionEvent.Entities.EntityA;

//             // var playerLTW = LocalToWorld[entityA];
//             // playerLTW.Forward = normal;
//             // Entity entityB = collisionEvent.Entities.EntityB;

//             // var details = collisionEvent.CalculateDetails(ref world);

//             // // The average contact point where the two entities collided,
//             // // this can be used to position the 
//             // var avererageContactPoint = details.AverageContactPointPosition;

//             // // TODO: This can be wrong aswell, just a guess which one of the entities is the player.
//             // var playerPos = translations[entityA];

//             // float2 pushVector = new float2(avererageContactPoint.x - playerPos.Value.x, avererageContactPoint.z - playerPos.Value.z);
//             // var currentVelocity = velocity[entityA];
//             // Debug.Log("The current velocity: " + currentVelocity.Value);


//             // var penetrationVector = math.normalizesafe(pushVector, 0) * 0.5f - pushVector;

//             // currentVelocity.Value.x -= penetrationVector.x;
//             // currentVelocity.Value.z -= penetrationVector.y;

//             // Debug.Log("The new velocity: " + currentVelocity.Value);

//             // velocity[entityA] = currentVelocity;



//             // pushVector = math.normalizesafe(-pushVector) * 0.5f;


//             // // var normalizedVector = math.normalizesafe(avererageContactPoint - playerPos.Value, 0) * 0.5f;
//             // // Debug.Log("The length of the streched vector: " + pushVector);
//             // playerPos.Value.x = avererageContactPoint.x + pushVector.x;
//             // // playerPos.Value = avererageContactPoint;
//             // playerPos.Value.z = avererageContactPoint.z + pushVector.y;

//             // translations[entityA] = playerPos;


//         }
//     }

//     protected override JobHandle OnUpdate(JobHandle inputDeps)
//     {
//         var translations = GetComponentDataFromEntity<Translation>(false);
//         var physicsWorld = buildPhysicsWorld.PhysicsWorld;
//         var velocity = GetComponentDataFromEntity<Velocity>(false);
//         var collisionJob = new CollisionJob()
//         {
//             world = physicsWorld,
//             // translations = translations,
//             velocity = velocity,

//         }.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

//         collisionJob.Complete();
//         return inputDeps;
//     }
// }