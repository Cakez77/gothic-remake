using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Unity.Transforms;

[DisableAutoCreation]
[UpdateAfter(typeof(TransformSystemGroup))]
public class AlternateMovementSystem : SystemBase {
    private bool waitForRotation = false;
    protected override void OnUpdate() {
        var playerInput = GetSingleton<PlayerInput>();
        var cameraTransform = UnityEngine.Camera.main.transform;
        var movementSpeed = 5f;
        var dTime = Time.DeltaTime;

        Entities.WithAll<PlayerTag>().ForEach((ref PhysicsVelocity velocity, ref Rotation rotation, in LocalToWorld localToWorld) => {

            var input = playerInput.InputVector;

            // Only change the rotation if input is being sent
            var magnitude = math.length(input);

            // TODO: This should be in it's own system? 
            // Only do stuff if there is input
            if (magnitude > 0) {

                var cameraForward = zeroOutY(cameraTransform.forward);
                var cameraRight = zeroOutY(cameraTransform.right);

                var playerForward = math.forward(rotation.Value);

                // Calculate the rotation direction based on input
                // in radians
                var result = math.atan2(input.z, input.x);

 
                Debug.DrawRay(localToWorld.Position, cameraRight, Color.blue);

                // Direction and relative angle in which the character will move
                var direction = getNormDirection(cameraForward, cameraRight, input);
                Debug.DrawRay(localToWorld.Position, direction, Color.red);
                var angle = Vector3.Angle(playerForward, direction);
                Debug.Log("Angle: " + angle);
                

                // Direction in which to rotate
                //var targetRotation = quaternion.RotateY(result+ math.radians(cameraTransform.eulerAngles.y));
                var targetRotation = quaternion.LookRotation(direction, localToWorld.Up);

                // Set something that 
                if (angle > 150) {
                    waitForRotation = true;
                    velocity.Linear = float3.zero;
                }

                if(!waitForRotation) {
                    rotation.Value = math.slerp(rotation.Value, targetRotation, 0.1f);

                    velocity.Linear = math.lerp(velocity.Linear, direction * magnitude * movementSpeed, 0.1f);
                } else {
                    var currentRotation = rotation.Value;
                    rotation.Value = Quaternion.RotateTowards(rotation.Value, targetRotation, 40);
                    if (rotation.Value.value.y == targetRotation.value.y) {
                        waitForRotation = false;
                    }  
                }
            }

        }).WithoutBurst().Run();
    }

    // TODO: maybe place this into a central library, might be used more often
    private float3 zeroOutY(float3 vector) {
        return new float3(vector.x, 0, vector.z);
    }

    //TODO: change the input from float3 to float2
    private float3 getNormDirection(float3 forward, float3 right, float3 input) {
        return math.normalizesafe(input.x * forward + input.z * right, 0);
    }
}