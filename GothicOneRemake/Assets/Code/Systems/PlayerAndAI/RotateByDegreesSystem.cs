using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

[DisableAutoCreation]
public class RotateByDegreesSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem endSimulationECBSystem;

    protected override void OnCreate() {
        base.OnCreate();

        endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
        var buffer = endSimulationECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.WithAll<WaitForRotationTag>().ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, in Heading heading) => {
            rotation.Value = Quaternion.RotateTowards(rotation.Value, quaternion.LookRotation(heading.Value, new float3(0f, 1f, 0f)), 190);

            buffer.RemoveComponent(entityInQueryIndex, entity, typeof(WaitForRotationTag));
            if (math.forward(rotation.Value).y == heading.Value.y) {
            }
        }).ScheduleParallel();
    }
}