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
                ref OnGround og,
                ref MovementAcceleration ma,
                in Heading h,
                in JumpForce jf,
                in LocalToWorld ltw) =>
                {
                    // local copies 
                    var linearVelocity = vel.Linear;
                    var onGround = og.Value;
                    var movementAcceleration = ma.Value;
                    var heading = h.Value;
                    var localToWorld = ltw.Value;
                    var 

                    // environment information 
                    var groundNormal = ColliderCast(ltw.Position, ltw.Position + new float3(0, -0.75f, 0));
                    float angle = Vector3.Angle(ltw.Up, groundNormal);
                    var cross = math.cross(ltw.Right, groundNormal);

                    // player states
                    bool moving = math.length(heading.Value) > 0;
                    bool grounded = math.length(groundNormal) > 0;
                    bool inAir = !grounded;
                    bool jumped = jumpForce.Value != 0;























                    if (moving)
                    {
                        // Increment elapsed time
                        if (movementAcceleration.ElapsedTime < movementAcceleration.AccelerationTime)
                        {
                            movementAcceleration.ElapsedTime += deltaTime;
                        }

                        // Reset reset timer
                        movementAcceleration.ResetTimer = 0;

                        // claculate speed based on elapsedTime
                        var easeInCubicSpeed = movementAcceleration.ElapsedTime / movementAcceleration.AccelerationTime;
                        easeInCubicSpeed *= easeInCubicSpeed * easeInCubicSpeed * movementAcceleration.MaxSpeed;

                        // apply velocity based on speed
                        vel.Linear.x = ltw.Forward.x * easeInCubicSpeed;
                        vel.Linear.z = ltw.Forward.z * easeInCubicSpeed;
                    }
                    else // No Input, start counting
                    {
                        movementAcceleration.ResetTimer += deltaTime;

                        var currentSpeed = math.length(new float3(vel.Linear.x, 0, vel.Linear.z));

                        // apply velocity based on speed
                        vel.Linear.x = ltw.Forward.x * currentSpeed;
                        vel.Linear.z = ltw.Forward.z * currentSpeed;
                    }


                    if (movementAcceleration.ResetTimer > 0.5f && moving == 0)
                    {
                        vel.Linear.x = math.lerp(vel.Linear.x, 0, 0.3f);
                        vel.Linear.z = math.lerp(vel.Linear.z, 0, 0.3f);

                        // Reset timer
                        movementAcceleration.ElapsedTime = 0;
                    }




                    if (grounded)
                    {
                        if (jumpForce.Value != 0) // We jumped
                        {
                            vel.Linear.y = jumpForce.Value;
                        }
                        else // We did not jump
                        {
                            if (cross.y < 0) // On a ramp looking down
                            {
                                if (moving == 0) // Standing
                                {
                                    vel.Linear.y = math.lerp(vel.Linear.y, moving * cross.y * 6.5f, 0.2f); //TODO Check if this is a good value
                                }
                                else // Moving
                                {
                                    vel.Linear.y = moving * cross.y * 6.5f; //TODO Check if this is a good value
                                }
                            }
                            else // On ground or ramp looking up
                            {
                                vel.Linear.y = moving * cross.y; //TODO Check if this is a good value


                            }

                        }
                    }

                    onGround.Value = grounded;


                }).WithoutBurst().Run();




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