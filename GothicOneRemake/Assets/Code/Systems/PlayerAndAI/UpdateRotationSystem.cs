using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(InputProcessingSystem))]
public class UpdateRotationSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem endSimulationECBSystem;

    protected override void OnCreate() {
        base.OnCreate();

        endSimulationECBSystem = GetECBSystem();
    }
    protected override void OnUpdate() {

        var buffer = endSimulationECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, ref Heading heading, ref PhysicsVelocity vel) => {
            var dir = heading.Value;
            var magnitude = GetDirectionalMagnitude(dir);

            if (magnitude > 0) {

                var forward = math.forward(rotation.Value);
                var angle = CalculateAngle(forward, dir);

                if (angle > 150) {
                    vel.Linear.x = 0;
                    vel.Linear.z = 0;
                    vel.Linear.y = 0;
                    AddWaitForRotationComponent();
                } else {

                    rotation.Value = RotateSmooth(rotation.Value, dir);
                }
            }


            //===========================  Simple helper functions  =============================
            
            /**
             * This function calculates the magnitued on the X and Z Axis
             */
            float GetDirectionalMagnitude(float2 direction) {
                return math.length(direction);
            }

            float CalculateAngle(float3 a, float2 b) {
                return Vector3.Angle(a, new float3(b.x, 0f, b.y));
            }

            float2 Zero() {
                return new float2(0, 0);
            }

            void AddWaitForRotationComponent() {
                buffer.AddComponent(entityInQueryIndex, entity, typeof(WaitForRotationTag));
            }

            quaternion RotateSmooth(quaternion rot, float2 direction) {
                var vector3 = new float3(direction.x, 0f, direction.y);
                var targetRotation = quaternion.LookRotation(vector3, math.up());
                return math.slerp(rot, targetRotation, 0.1f);
            }
        }).ScheduleParallel();

        CompleteDependency();
    }


    EndSimulationEntityCommandBufferSystem GetECBSystem() {
        return World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
}