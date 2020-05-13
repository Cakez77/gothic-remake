using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Jobs;
using VelocityStateMachine;

//namespace VelocityStateMachine
//{
    public class ApplyVelocitySystem : SystemBase
    {
        protected unsafe override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entities.ForEach(
                (ref PhysicsVelocity physicsVelocity,
                ref VelocityState velocityState,
                in LocalToWorld ltw,
                in GroundNormal normal,
                in JumpForce jumpForce,
                in MovementSpeed movementSpeed) =>
                {
                    float3* linearVelPtr;
                    fixed (PhysicsVelocity* vel = &physicsVelocity)
                    {
                        linearVelPtr = &vel->Linear;
                    };

                    LocalToWorld* ltwPointer;
                    fixed (LocalToWorld* ltwPtr = &ltw)
                    {
                        ltwPointer = ltwPtr;
                    }

                    float time = velocityState.Time;

                    time += deltaTime/velocityState.Duration;
                    velocityState.Time = time;

                    if(time > 1) //Past "1" the state reached "maximum"
                    {
                        time = 1;
                    }
                    physicsVelocity.Linear = velocityState.VelocityFunction.Invoke(linearVelPtr, 
                        &ltwPointer->Forward, ltw.Right, normal.Value, time, movementSpeed.Value, jumpForce.Value);


                }).Schedule();
        }
    }
//}