using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
public class UpdateRotationSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem endSimulationECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        endSimulationECBSystem = GetECBSystem();
    }

    protected override void OnUpdate()
    {
        var buffer = endSimulationECBSystem.CreateCommandBuffer().ToConcurrent();


        Entities
            .WithEntityQueryOptions(EntityQueryOptions.FilterWriteGroup)
            .ForEach(
                (Entity entity,
                int entityInQueryIndex,
                ref Rotation rotation,
                ref Heading heading,
                ref PhysicsVelocity vel) =>
                {
                    var dir = heading.Value;
                    var magnitude = GetDirectionalMagnitude(dir);

                    if (magnitude > 0)
                    {

                        var forward = math.forward(rotation.Value);
                        var angle = CalculateAngle(forward, dir);

                        if (angle > 150)
                        {
                            AddSmooth180Rotation(vel.Linear);
                        }
                        else
                        {
                            rotation.Value = RotateSmooth(rotation.Value, dir);
                        }
                    }


                    //===========================  Simple helper functions  =============================

                    /**
                     * This function calculates the magnitued on the X and Z Axis
                     */
                    float GetDirectionalMagnitude(float2 direction)
                    {
                        return math.length(direction);
                    }

                    float CalculateAngle(float3 a, float2 b)
                    {
                        return Vector3.Angle(a, new float3(b.x, 0f, b.y));
                    }

                    void AddSmooth180Rotation(float3 velocity)
                    {
                        buffer.AddComponent<Smooth180Rotation>(entityInQueryIndex, entity);
                        buffer.SetComponent<Smooth180Rotation>(entityInQueryIndex, entity, new Smooth180Rotation { OriginalVelocity = velocity, Duration = 0.15f });
                    }

                    quaternion RotateSmooth(quaternion rot, float2 direction)
                    {
                        var vector3 = new float3(direction.x, 0f, direction.y);
                        var targetRotation = quaternion.LookRotation(vector3, math.up());
                        return math.slerp(rot, targetRotation, 0.1f);
                    }
                }).ScheduleParallel();

        CompleteDependency();
    }


    EndSimulationEntityCommandBufferSystem GetECBSystem()
    {
        return World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
}