using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using Unity.Physics;

[UpdateAfter(typeof(UpdateRotationSystem))]
public class RotateByDegreesSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem endSimulationECBSystem;

    protected override void OnCreate() {
        base.OnCreate();

        endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
        var buffer = endSimulationECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.WithAll<WaitForRotationTag>().ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, ref PhysicsVelocity vel, in Heading heading) => {
            // Rotate by 180° 
            rotation.Value = RotateByDegrees(rotation.Value, heading.Value, 182);
            vel.Linear.x = 0;
            vel.Linear.z = 0;

            // Remove the tag
            buffer.RemoveComponent(entityInQueryIndex, entity, typeof(WaitForRotationTag));


        }).Schedule();






        quaternion RotateByDegrees(quaternion rot, float2 dir, float deg) {
            var direction = new float3(dir.x, 0f, dir.y);
            var targetRot = quaternion.LookRotationSafe(direction, new float3(0f, 1f, 0f));
            return Quaternion.RotateTowards(rot, targetRot, deg);
        }
    }
}