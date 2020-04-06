using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputProcessingSystem : SystemBase {
    private Keyboard keyboard;
    private Mouse mouse;

    protected override void OnCreate() {
        keyboard = Keyboard.current;
        mouse = Mouse.current;
    }

    protected override void OnUpdate() {

        //===========================   Gather input    =============================

        var xDir = (keyboard.wKey.isPressed ? 1 : 0) + (keyboard.sKey.isPressed ? -1 : 0);
        var zDir = (keyboard.aKey.isPressed ? -1 : 0) + (keyboard.dKey.isPressed ? 1 : 0);

        var spaceDown = keyboard.spaceKey.wasPressedThisFrame;

        var mouseDir = new float2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));

        var scrollDir = mouse.scroll.ReadValue().y / 100;

        //===========================   Accuire data for calculation    =============================

        // TODO: Will be removed once the camera is imported in ECS
        var cameraT = UnityEngine.Camera.main.transform;
        var cameraFwd = cameraT.forward;
        var cameraR = cameraT.right;


        // Update the heading of the player depending on keyboard input and camera playement
        Entities.ForEach((ref Heading heading, in BaseSpeed baseSpeed, in JumpHeight jumpHeight) => {
            // calculate raw heading value alont the X and Z Axis
            var rawHeading = xDir * cameraFwd + zDir * cameraR;
            rawHeading.y = 0f;

            // normalize the raw value, this ensures consistent movement
            // in all directions
            var nHeading = math.normalizesafe(rawHeading, 0);

            // Set the jump height
            nHeading.y = spaceDown ? jumpHeight.Value : 0;
                

            // write back the calculated value
            heading.Value = nHeading;

        }).ScheduleParallel();


        // Updating components the UpdateCameraSystem needs to read 
        var mouseSensitivity = GetSingleton<MouseSensitivity>();
        Entities.ForEach((ref PitchYaw pitchYaw, ref PlayerDistance playerDistance) => {

            pitchYaw.Value += mouseDir * mouseSensitivity.Value;
            pitchYaw.Value.x = math.clamp(pitchYaw.Value.x, 5.5f, 8f);

            playerDistance.Value += scrollDir;
            playerDistance.Value = math.clamp(playerDistance.Value, 5.5f, 15.5f);

        }).ScheduleParallel();
    }
}