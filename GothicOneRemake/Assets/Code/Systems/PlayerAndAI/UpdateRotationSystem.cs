using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;


public class UpdateRotationSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem endSimulationECBSystem;

    protected override void OnCreate() {
        base.OnCreate();

        endSimulationECBSystem = GetECBSystem();
    }
    protected override void OnUpdate() {

        var buffer = endSimulationECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, in Heading heading) => {

            if (math.length(heading.Value) > 0) {

                var forward = math.forward(rotation.Value);
                var angle = Vector3.Angle(forward, heading.Value);

                if (angle > 150) {
                    AddWaitForRotationComponent();
                } else {
                    rotation.Value = RotateSmooth(rotation.Value, heading.Value);
                }
            }


            //===========================  Simple helper functions  =============================
            quaternion RotateSmooth(quaternion rot, float3 direction) {
                var targetRotation = quaternion.LookRotationSafe(direction, new float3(0f, 1f, 0f));
                return math.slerp(rot, targetRotation, 0.1f);
            }

            void AddWaitForRotationComponent() {
                buffer.AddComponent(entityInQueryIndex, entity, typeof(WaitForRotationTag));
            }
        }).ScheduleParallel();

        CompleteDependency();
    }


    EndSimulationEntityCommandBufferSystem GetECBSystem() {
        return World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
}