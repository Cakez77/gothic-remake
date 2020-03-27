using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// [UpdateBefore(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(TransformSystemGroup))]
public class CameraFollowTargetSystem : SystemBase
{
    // private float2 pitchYaw;

    // OnUpdate is called one per frame and is Part of the SimulationSystemGroup
    protected override void OnUpdate()
    {

        var playerInput = GetSingleton<PlayerInput>();
        var mouseSensitivity = GetSingleton<MouseSensitivity>();
        var pitchYaw = GetSingleton<PitchYaw>();
        var playerEntity = GetSingletonEntity<PlayerTag>();

        var rotationComponents = GetComponentDataFromEntity<Rotation>(false);
        var translationComponents = GetComponentDataFromEntity<Translation>(true);

        var cameraTransform = UnityEngine.Camera.main.transform;

        // Get the player position and direction, and change his rotation
        Entities.WithAll<CameraTargetTag>().ForEach((ref Rotation rotation, ref Translation translation, in LocalToWorld localToWorld) =>
        {
            //// Rotate the player along the Y-Axis
            //var playerRotation = rotationComponents[playerEntity];
            //playerRotation.Value = quaternion.RotateY(pitchYaw.Value.x);
            //rotationComponents[playerEntity] = playerRotation;

            // Rotate the camera around the target
            // TODO: Enable first person camera upon pressing a key or zooming with the mouse
            cameraTransform.rotation = rotation.Value;

            // Accumulate rotation
            pitchYaw.Value += playerInput.MouseMovement * mouseSensitivity.Value;
            SetSingleton<PitchYaw>(pitchYaw);

            // Rotate the camera target along the X and Y-Axis
            rotation.Value = quaternion.Euler(new float3(-pitchYaw.Value.y, pitchYaw.Value.x, 0f));

            // Position the camera behind the player
            cameraTransform.position = localToWorld.Position + localToWorld.Forward * -5.5f;

            // Position the camera target where the player is
            var playerTranslation = translationComponents[playerEntity];
            translation.Value = playerTranslation.Value + new float3(0, 1, 0);

        }).WithoutBurst().Run();

    }
}