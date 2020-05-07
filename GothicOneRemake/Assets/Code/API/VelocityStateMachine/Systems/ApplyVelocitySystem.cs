using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Unity.Jobs;

namespace VelocityStateMachine
{
    public class ApplyVelocitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entities.ForEach(
                (ref PhysicsVelocity physicsVelocity,
                in VelocityState velocityState,
                in LocalToWorld ltw,
                in JumpForce jumpForce,
                in MovementSpeed movementSpeed) =>
                {
                    var linearVelocity = physicsVelocity.Linear;
                    var forward = ltw.Forward;
                    float time = velocityState.Time;
                    float duration = velocityState.Duration;
                    float speed = movementSpeed.Value;

                    time += deltaTime/duration;

                    if(time > 1) // At "1" the state reached "maximum"
                    {
                        time = 1;
                    }

                    physicsVelocity.Linear = velocityState.VelocityFunction.Invoke(linearVelocity, 
                        forward, time, movementSpeed.Value, jumpForce.Value);

                }).Schedule();






















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






           

            float EaseInSpeedQuad(float speed, float time)
            {
                return speed * time * time;
            }
        }
    }
}