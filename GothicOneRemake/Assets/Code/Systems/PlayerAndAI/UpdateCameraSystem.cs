using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class UpdateCameraSystem : SystemBase
{

    protected override void OnUpdate()
    {

        var mainCamera = UnityEngine.Camera.main;
        var rotationSmoothnes = GetSingleton<RotationSmothnes>();

        // Get the player position and direction, and change his rotation
        Entities.WithAll<CameraTargetTag>().ForEach(
            (ref Rotation r,
            in CameraFOV fov,
            in PitchYaw py,
            in LocalToWorld ltw,
            in PlayerDistance pd,
            in TakeoffHeight th) =>
            {
                var rotation = r.Value;
                var fieldOfViewValue = fov.Value;
                var pitchYaw = py.Value;
                var position = ltw.Position;
                var forward = math.forward(rotation);
                float distanceToPlayer = pd.Value;
                float takeoffHeight = th.Value;
                bool falling = position.y < takeoffHeight;

                if (!falling)
                {
                    position.y = takeoffHeight;
                }

                var targetPosition = positionCamera();
                var targetRotation = rotateByPitchAndYaw();

                // TODO: Will be changed/removed once the camera is fully integrated in ESC
                // THIS LINE IS THE PROBLEM

                mainCamera.transform.position = targetPosition;
                mainCamera.transform.rotation = rotation;
                mainCamera.fieldOfView = math.lerp(mainCamera.fieldOfView, fieldOfViewValue, 0.1f);


                // ======================= write back ========================
                r.Value = targetRotation;




                //===========================  Helper Functions    =============================
                Vector3 positionCamera()
                {
                    return position + forward * -distanceToPlayer;
                }

                quaternion rotateByPitchAndYaw()
                {
                    var targetRot = new float3(pitchYaw.x, pitchYaw.y, 0f);
                    return math.slerp(rotation, quaternion.Euler(targetRot), rotationSmoothnes.Value);
                }

            }).WithoutBurst().Run();

    }
}