using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Unity.Jobs;

[UpdateAfter(typeof(RotateByDegreesSystem))]
[UpdateAfter(typeof(TransformSystemGroup))]
public class ApplyVelocitySystem : SystemBase
{


    BuildPhysicsWorld m_BuildPhysicsWorld;
    EndFramePhysicsSystem m_EndFramePhysicsSystem;
    EntityCommandBufferSystem m_EndSimulationSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        m_EndFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
        m_BuildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_EndSimulationSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }


    protected override void OnUpdate()
    {

        Dependency = JobHandle.CombineDependencies(m_EndFramePhysicsSystem.FinalJobHandle, Dependency);
        // Physics system for collision checking
        var physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

        var deltaTime = Time.DeltaTime;

        Entities
            .WithEntityQueryOptions(EntityQueryOptions.FilterWriteGroup)
            .ForEach(
                (ref PhysicsVelocity vel,
                ref OnGround onGround,
                ref MovementAcceleration movementAcceleration,
                in Heading heading,
                in YVelocity yVel,
                in LocalToWorld ltw) =>
                {

                    var magnitude = math.length(heading.Value);

                    if (magnitude > 0)
                    {
                        // Increment elapsed time
                        if (movementAcceleration.ElapsedTime < movementAcceleration.AccelerationTime)
                        {
                            movementAcceleration.ElapsedTime += deltaTime;
                        }

                        // claculate speed based on elapsedTime
                        var easeInCubicSpeed = movementAcceleration.ElapsedTime / movementAcceleration.AccelerationTime;
                        easeInCubicSpeed *= easeInCubicSpeed * easeInCubicSpeed * movementAcceleration.MaxSpeed;

                        // apply velocity based on speed
                        vel.Linear.x = ltw.Forward.x * easeInCubicSpeed;
                        vel.Linear.z = ltw.Forward.z * easeInCubicSpeed;
                    }
                    else
                    {
                        vel.Linear.x = math.lerp(vel.Linear.x, 0, 0.2f);
                        vel.Linear.z = math.lerp(vel.Linear.z, 0, 0.2f);

                        // Reset timer
                        movementAcceleration.ElapsedTime = 0;
                    }




                    var groundNormal = ColliderCast(ltw.Position, ltw.Position + new float3(0, -0.75f, 0));
                    var angle = Vector3.Angle(ltw.Up, groundNormal);
                    var cross = math.cross(ltw.Right, groundNormal);

                    var grounded = math.length(groundNormal) > 0;

                    if (grounded)
                    {
                        if (yVel.Value != 0) // We jumped
                        {
                            vel.Linear.y = yVel.Value;
                        }
                        else // We did not jump
                        {
                            if (cross.y < 0) // On a ramp looking down
                            {
                                if (magnitude == 0) // Standing
                                {
                                    vel.Linear.y = math.lerp(vel.Linear.y, magnitude * cross.y * 6.5f, 0.2f); //TODO Check if this is a good value
                                }
                                else // Moving
                                {
                                    vel.Linear.y = magnitude * cross.y * 6.5f; //TODO Check if this is a good value
                                }
                            }
                            else // On ground or ramp looking up
                            {
                                vel.Linear.y = magnitude * cross.y; //TODO Check if this is a good value
                            }

                        }
                    }

                    onGround.Value = grounded;


                }).ScheduleParallel();




        m_EndSimulationSystem.AddJobHandleForProducer(Dependency);






        unsafe float3 ColliderCast(float3 rayFrom, float3 rayTo)
        {

            CollisionFilter filter = new CollisionFilter
            {
                BelongsTo = 0b_1u, // Layer 0
                CollidesWith = 0b_100u, // Layer 2
                GroupIndex = 0
            };

            SphereGeometry sphere = new SphereGeometry
            {
                Center = float3.zero,
                Radius = 0.5f
            };

            BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.SphereCollider.Create(sphere, filter);

            ColliderCastInput input = new ColliderCastInput
            {
                Collider = (Unity.Physics.Collider*) collider.GetUnsafePtr(),
                Orientation = quaternion.identity,
                Start = rayFrom,
                End = rayTo
            };

            ColliderCastHit hit = new ColliderCastHit();

            bool haveHit = collisionWorld.CastCollider(input, out hit);

            if (haveHit)
            {
                return hit.SurfaceNormal;
            }

            return float3.zero;
        }
    }
}