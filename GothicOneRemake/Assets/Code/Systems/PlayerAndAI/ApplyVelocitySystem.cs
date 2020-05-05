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
                    // local copies (makes debugging easier)
                    var linearVelocity = vel.Linear;
                    bool grounded = og.Value;
                    float elapsedTime = ma.ElapsedTime;
                    float accelerationDuration = ma.AccelerationDuration;
                    float maxSpeed = ma.MaxSpeed;
                    float resetTimer = ma.ResetTimer;
                    float jumpForce = jf.Value;
                    var heading = h.Value;
                    var forward = ltw.Forward;
                    var position = ltw.Position;
                    var up = ltw.Up;
                    var right = ltw.Right;

                    // environment information 
                    var groundNormal = ColliderCast(position, position + new float3(0, -0.6f, 0));
                    float angle = Vector3.Angle(up, groundNormal);
                    var parallellGroundVector = math.cross(right, groundNormal);

                    // player states
                    bool haveHeading = math.length(heading) > 0;
                    bool moving = haveHeading || elapsedTime > 0;
                    bool lookingUp = parallellGroundVector.y >= 0;
                    bool lookingDown = parallellGroundVector.y < 0;
                    grounded = math.length(groundNormal) > 0;
                    bool inAir = !grounded;
                    bool jumped = grounded && jumpForce != 0;
                    bool notMoving = resetTimer > 0.15f;
                    bool notMovingOnGround = notMoving && !jumped && grounded;
                    bool shouldAddTime = elapsedTime < accelerationDuration && haveHeading;

                    bool inMotionWithNoInput = !haveHeading && moving;
                    bool movingUpOnRamp = moving && !jumped && grounded && lookingUp;
                    bool movingDownOnRamp = moving && !jumped && grounded && lookingDown;


                    // =============================== Logic =======================================

                    if (shouldAddTime)
                    {
                        addTime();
                    }
                    float speed = EaseInSpeedQuad(maxSpeed, elapsedTime / accelerationDuration);


                    if (movingUpOnRamp)
                    {
                        accelerateY(1f);
                    }
                    if (movingDownOnRamp)
                    {
                        accelerateY(6.5f);
                    }
                    if (jumped)
                    {
                        linearVelocity.y = jumpForce;
                    }
                    if (haveHeading)
                    {
                        resetCountdown();
                        accelerate();
                    }
                    if (inMotionWithNoInput)
                    {
                        increaseResetTimer();
                        accelerate();
                    }
                    if (notMoving)
                    {
                        decelerate();
                        resetTime();
                    }
                    if (notMovingOnGround)
                    {
                        decelerateY();
                    }

                    // ======================== camera =======================

                    // Notify camera to update position
                    if (grounded)
                    {
                        SetSingleton(new TakeoffHeight { Value = position.y + 0.7f });
                    }
                    
                    // ======================= write back ========================
                    vel.Linear = linearVelocity;
                    og.Value = grounded;
                    ma.ElapsedTime = elapsedTime;
                    ma.ResetTimer = resetTimer;












                    //================= helper functions ===================

                    void decelerate()
                    {
                        linearVelocity.x = math.lerp(linearVelocity.x, 0, 0.1f);
                        linearVelocity.z = math.lerp(linearVelocity.z, 0, 0.1f);
                    }

                    void resetTime()
                    {
                        elapsedTime = 0;
                    }

                    void increaseResetTimer()
                    {
                        resetTimer += deltaTime;
                    }

                    void accelerate()
                    {
                        linearVelocity.x = forward.x * speed;
                        linearVelocity.z = forward.z * speed;
                    }

                    void resetCountdown()
                    {
                        resetTimer = 0;
                    }

                    void addTime()
                    {
                        elapsedTime += deltaTime;
                    }

                    void accelerateY(float intensity)
                    {
                        linearVelocity.y = elapsedTime / accelerationDuration * parallellGroundVector.y * intensity;
                    }

                    void decelerateY()
                    {
                        linearVelocity.y = math.lerp(linearVelocity.y, 0, 0.1f);
                    }

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

        float EaseInSpeedQuad(float speed, float time)
        {
            return speed * time * time;
        }
    }
}