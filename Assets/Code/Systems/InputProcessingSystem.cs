using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine;
using VelocityStateMachine;

public class InputProcessingSystem : SystemBase
{
    private Keyboard keyboard;
    private Mouse mouse;

    protected override void OnCreate()
    {
        keyboard = Keyboard.current;
        mouse = Mouse.current;
    }

    protected override void OnUpdate()
    {

        //===========================   Gather input    =============================

        var xDir = (keyboard.wKey.isPressed ? 1 : 0) + (keyboard.sKey.isPressed ? -1 : 0);
        var zDir = (keyboard.aKey.isPressed ? -1 : 0) + (keyboard.dKey.isPressed ? 1 : 0);

        var spaceDown = keyboard.spaceKey.isPressed ? 1 : 0;

        var mouseDir = new float2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));

        var scrollDir = -mouse.scroll.ReadValue().y / 200;


        // TODO: Will be removed once the camera is imported in ECS
        var cameraT = UnityEngine.Camera.main.transform;
        var cameraFwd = cameraT.forward;
        var cameraR = cameraT.right;


        // Heading indicates in which direction the player should move
        Entities
            .WithAll<PlayerTag>()
            .ForEach(
                (ref Heading heading,
                ref JumpForce jumpForce,
                in JumpHeight jumpHeight) =>
                {

                    var relativeHeading = HeadingRelativeToCamera();
                    relativeHeading = Normalize();

                    jumpForce.Value = spaceDown * jumpHeight.Value;
                    heading.Value = relativeHeading;


                    //===========================  Simple helper functions    =============================    
                    float2 HeadingRelativeToCamera()
                    {
                        var dir = (xDir * cameraFwd + zDir * cameraR);
                        return new float2(dir.x, dir.z);
                    }

                    float2 Normalize()
                    {
                        return math.normalizesafe(relativeHeading, 0);
                    }

                }).WithoutBurst().Run();


        // Updating components the UpdateCameraSystem needs to read 
        var mouseSensitivity = GetSingleton<MouseSensitivity>();
        Entities.ForEach((ref PitchYaw pitchYaw, ref PlayerDistance playerDistance, ref CameraFOV cameraFOV) =>
        {
            bool input = xDir != 0 || zDir != 0;

            cameraFOV.Value = input ? 69 : 70;

            pitchYaw.Value += mouseDir * mouseSensitivity.Value;
            pitchYaw.Value.x = math.clamp(pitchYaw.Value.x, 5.5f, 7.5f);

            playerDistance.Value += scrollDir;
            playerDistance.Value = math.clamp(playerDistance.Value, 5.5f, 15.5f);

        }).ScheduleParallel();
    }
}