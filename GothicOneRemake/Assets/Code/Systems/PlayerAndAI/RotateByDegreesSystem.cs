using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;


public class RotateByDegreesSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem endSimulationECBSystem;

    protected override void OnCreate() {
        base.OnCreate();

        endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
        var buffer = endSimulationECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.WithAll<WaitForRotationTag>().ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, in Heading heading) => {
            // Rotate by 180° 
            rotation.Value = RotateByDegrees(rotation.Value, heading.Value, 182);

            // Remove the tag
            buffer.RemoveComponent(entityInQueryIndex, entity, typeof(WaitForRotationTag));


            quaternion RotateByDegrees(quaternion rot, float3 dir, float deg) {
                var targetRot = quaternion.LookRotationSafe(dir, new float3(0f, 1f, 0f));
                return Quaternion.RotateTowards(rot, targetRot, deg);
            }
        }).ScheduleParallel();

        Dependency.Complete();
    }
}