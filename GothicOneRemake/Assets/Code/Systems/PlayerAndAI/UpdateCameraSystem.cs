using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;



public class UpdateCameraSystem : SystemBase {

    protected override void OnUpdate() {

        var cameraT = UnityEngine.Camera.main.transform;
        var rotationSmoothnes = GetSingleton<RotationSmothnes>();

        // Get the player position and direction, and change his rotation
        Entities.WithAll<CameraTargetTag>().ForEach(
            (ref Rotation rotation,
            in PitchYaw pitchYaw,
            in LocalToWorld ltw,
            in PlayerDistance playerDistance) => {

                    // TODO: Will be changed/removed once the camera is fully integrated in ESC
                    // THIS LINE IS THE PROBLEM
                    var targetPosition = ltw.Position + math.forward(rotation.Value) * -playerDistance.Value;
                    cameraT.position = targetPosition;
                    cameraT.rotation = rotation.Value;

                var currentRotation = rotation.Value;
                var targetRotation = GetTargetRotation(pitchYaw.Value);

                rotation.Value = RotateSmooth(targetRotation);




                //===========================  Helper Functions    =============================
                quaternion RotateSmooth(float3 targetRot) {
                    return math.slerp(currentRotation, quaternion.Euler(targetRotation), rotationSmoothnes.Value);
                }

                float3 GetTargetRotation(float2 _pichYaw) {
                    return new float3(_pichYaw.x, _pichYaw.y, 0f);
                }

            }).WithoutBurst().Run();

    }
}