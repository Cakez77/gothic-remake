using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Physics;
using Unity.Jobs;
using VelocityStateMachine;
using Unity.Mathematics;
using Unity.Physics.Extensions;

//namespace VelocityStateMachine
//{
[UpdateAfter(typeof(TransformSystemGroup))]
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
                    float time = velocityState.Time;

                    time += deltaTime / velocityState.Duration;
                    velocityState.Time = time;

                    if (time > 1) // At "1" the state reached "maximum"
                    {
                        time = 1;
                    }

                    VelocityParams velocityParams = new VelocityParams
                    {
                        linearVelocity = physicsVelocity.Linear,
                        forward = ltw.Forward,
                        right = ltw.Right,
                        normal = normal.Value,
                        time = time,
                        movementSpeed = movementSpeed.Value,
                        jumpForce = jumpForce.Value
                    };

                    physicsVelocity.Linear = *velocityState.VelocityFunction.Invoke(&velocityParams);

                }).Schedule();
    }
}
//}