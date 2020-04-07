using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;

[DisableAutoCreation]
[UpdateAfter(typeof(TransformSystemGroup))]
public class UpdateRotationSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem endSimulationECBSystem;

    protected override void OnCreate() {
        base.OnCreate();

        endSimulationECBSystem = GetECBSystem();
    }
    protected override void OnUpdate() {

        var buffer = endSimulationECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, in Heading heading, in LocalToWorld ltw) => {


            var magnitude = GetDirectionalMagnitude(heading.Value);

            if (magnitude > 0) {

                var forward = math.forward(rotation.Value);
                var angle = Vector3.Angle(forward, heading.Value);

                if (angle > 150) {
                    AddWaitForRotationComponent();
                } else {
                    rotation.Value = RotateSmooth(rotation.Value, heading.Value, ltw.Up);
                }
            }


            //===========================  Simple helper functions  =============================
            quaternion RotateSmooth(quaternion rot, float3 direction, float3 up) {
                var targetRotation = quaternion.LookRotation(direction, up);
                return math.slerp(rot, targetRotation, 0.1f);
            }

            void AddWaitForRotationComponent() {
                buffer.AddComponent(entityInQueryIndex, entity, typeof(WaitForRotationTag));
            }

            /**
             * This function calculates the magnitued on the X and Z Axis
             */
            float GetDirectionalMagnitude(float3 direction) {
                return math.length(direction * new float3(1f, 0f, 1f));
            }
        }).WithoutBurst().Run();

        CompleteDependency();
    }


    EndSimulationEntityCommandBufferSystem GetECBSystem() {
        return World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
}