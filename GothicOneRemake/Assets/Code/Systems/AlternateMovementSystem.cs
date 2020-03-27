using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Unity.Transforms;


public class AlternateMovementSystem : SystemBase {
    protected override void OnUpdate() {
        var playerInput = GetSingleton<PlayerInput>();
        var movementSpeed = 5f;

        Entities.WithAll<PlayerTag>().ForEach((ref PhysicsVelocity velocity, ref Rotation rotation, in LocalToWorld localToWorld) => {
            var input = playerInput.InputVector;
            var magnitude = math.length(input);

            var result = math.atan2(input.z, input.x);
            if(magnitude > 0) {
                rotation.Value = math.slerp(rotation.Value, quaternion.RotateY(result).value, 0.5f);
            }
            
            velocity.Linear = math.normalizesafe(localToWorld.Forward * math.length(input), 0) * movementSpeed;

        }).WithoutBurst().Run();
    }
}