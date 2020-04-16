using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using Unity.Physics;

[UpdateAfter(typeof(UpdateRotationSystem))]
public class RotateByDegreesSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem endSimulationECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var buffer = endSimulationECBSystem.CreateCommandBuffer().ToConcurrent();
        var deltaTime = Time.DeltaTime;

        Entities
            .ForEach(
            (Entity entity,
            int entityInQueryIndex,
            ref Rotation rotation,
            ref PhysicsVelocity vel,
            ref Smooth180Rotation smooth180,
            in Heading heading) =>
            {
                if (smooth180.ElapsedTime < smooth180.Duration)
                {
                    smooth180.ElapsedTime += deltaTime;
                }
                else
                {
                    // Remove the component
                    buffer.RemoveComponent(entityInQueryIndex, entity, typeof(Smooth180Rotation));
                }

                // Tweening function
                var tweenRotationSmooth = smooth180.ElapsedTime / smooth180.Duration;

                // Tween the velocity
                vel.Linear = smooth180.OriginalVelocity * SmoothVelocity(tweenRotationSmooth);
                //vel.Linear.z = smooth180.OriginalVelocity.z * SmoothVelocity(tweenRotationSmooth);

                var degressToRotate = tweenRotationSmooth * 180 - smooth180.DegreesRotated;
                rotation.Value = RotateByDegrees(rotation.Value, heading.Value, degressToRotate);
                smooth180.DegreesRotated += degressToRotate;
            }).ScheduleParallel();


        Dependency.Complete();




        quaternion RotateByDegrees(quaternion rot, float2 dir, float deg)
        {
            var direction = new float3(dir.x, 0f, dir.y);
            var targetRot = quaternion.LookRotationSafe(direction, new float3(0f, 1f, 0f));
            return Quaternion.RotateTowards(rot, targetRot, deg);
        }

        float SmoothVelocity(float t)
        {
            if (t < 0.1)
            {
                return (1f - t * 10 * t * 10 * t * 10);
            }
            else if (t >= 0.1 && t < 0.8)
            {
                return 0f;
            }
            else
            {
                return -t * t * t;
            }
        }
    }
}