using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;


public class PlayerVelocitySystem : SystemBase {
    protected override void OnUpdate() {

        var playerInput = GetSingleton<PlayerInput>();
        var cameraTarget = GetSingletonEntity<CameraTargetTag>();
        var ltwComponents = GetComponentDataFromEntity<LocalToWorld>(true);

        Entities.WithAll<PlayerTag>().ForEach((ref PhysicsVelocity velocity, in JumpHeight jumpHeight, in BaseSpeed baseSpeed) => {

            // Make player movement relative to the camera
            var cameraLTW = ltwComponents[cameraTarget];
            
            // Calculate the direction in which force will be 
            // applied to the body
            var nDirection = getNormDirection(playerInput.InputVector, cameraLTW);

            // Add the jump height to the direction
            nDirection.y *= jumpHeight.Value;

            // Lerp the movement
            velocity.Linear.x = math.lerp(velocity.Linear.x, nDirection.x * baseSpeed.Value, 0.1f);
            velocity.Linear.z = math.lerp(velocity.Linear.z, nDirection.z * baseSpeed.Value, 0.1f);

            // TODO: Test jumping 
            velocity.Linear.y += nDirection.y;




















            float3 getNormDirection(float3 input, LocalToWorld ltw) {
                var up = new float3(0, 1, 0);
                return math.normalizesafe(input.x * zeroOutY(ltw.Forward) + input.y * up + input.z * zeroOutY(ltw.Right), 0);
            }

            float3 zeroOutY(float3 vector) {
                return new float3(vector.x, 0, vector.z);
            }

        }).WithoutBurst().Run();
    }
}