using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VelocityStateMachine;


public class UpdateCameraSystem : SystemBase
{

    protected override void OnUpdate()
    {

        var mainCamera = UnityEngine.Camera.main;
        var rotationSmoothnes = GetSingleton<RotationSmothnes>();
        var takeoff = GetSingleton<TakeoffHeight>();

        // Get the player position and direction, and change his rotation
        Entities.WithAll<CameraTargetTag>().ForEach(
            (ref Rotation rotation,
            in CameraFOV cameraFOV,
            in PitchYaw pitchYaw,
            in LocalToWorld ltw,
            in PlayerDistance distanceToPlayer) =>
            {
                var playerPosition = ltw.Position;
                var playerForward = math.forward(rotation.Value);
                bool playerFalling = playerPosition.y < takeoff.Value;


                if (!playerFalling)
                {
                    playerPosition.y = takeoff.Value;
                }

                var targetPosition = PositionCamera(playerPosition, playerForward, distanceToPlayer.Value);
                var targetRotation = Rotate(pitchYaw.Value, rotation.Value);

                // TODO: Will be changed/removed once the camera is fully integrated in ESC
                // THIS LINE IS THE PROBLEM

                mainCamera.transform.position = targetPosition;
                mainCamera.transform.rotation = rotation.Value;
                mainCamera.fieldOfView = math.lerp(mainCamera.fieldOfView, cameraFOV.Value, 0.1f);


                // ======================= write back ========================
                rotation.Value = targetRotation;




                //===========================  Helper Functions    =============================
                Vector3 PositionCamera(float3 p, float3 f, float distance)
                {
                    return p + f * -distance;
                }

                quaternion Rotate(float2 py, quaternion r)
                {
                    var targetRot = new float3(py.x, py.y, 0f);
                    return math.slerp(r, quaternion.Euler(targetRot), rotationSmoothnes.Value);
                }

            }).WithoutBurst().Run();

    }
}