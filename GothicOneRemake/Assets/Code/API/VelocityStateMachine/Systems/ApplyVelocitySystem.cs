using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Physics;
using Unity.Jobs;
using VelocityStateMachine;

//namespace VelocityStateMachine
//{
    public class ApplyVelocitySystem : SystemBase
    {
        protected override void OnUpdate()
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
                    var linearVelocity = physicsVelocity.Linear;
                    float time = velocityState.Time;

                    time += deltaTime/velocityState.Duration;
                    velocityState.Time = time;

                    if(time > 1) // At "1" the state reached "maximum"
                    {
                        time = 1;
                    }
                    linearVelocity = velocityState.VelocityFunction.Invoke(linearVelocity, 
                        ltw.Forward, ltw.Right, normal.Value, time, movementSpeed.Value, jumpForce.Value);

                    physicsVelocity.Linear = linearVelocity;

                }).Schedule();
        }
    }
//}