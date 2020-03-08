using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.InputSystem;
using UnityEngine;

// [UpdateBefore(typeof(Update))]
public class InputSystem : SystemBase {

    protected override void OnCreate() {
        // Create an Entity to hold data about the player Input
        // var playerInput = EntityManager.CreateEntity(typeof(PlayerInput));
        // EntityManager.SetName(playerInput, "PlayerInput");
    }

    protected override void OnUpdate() {

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        var x = (keyboard.aKey.isPressed ? -1 : 0) + (keyboard.dKey.isPressed ? 1 : 0);
        var y = (keyboard.sKey.isPressed ? -1 : 0) + (keyboard.wKey.isPressed ? 1 : 0);

        var space = (keyboard.spaceKey.isPressed ? 1 : 0);

        // TODO: Find a InputSystem replacement for this
        var MouseInput = new float2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        // TODO: Read up on the new InputSystem and maybe use that.
        var KeyInput = new float2(x, y);

        Entities.ForEach((ref PlayerInput playerInput) => {
            playerInput.Space = space;
            playerInput.MouseMovement = MouseInput;
            playerInput.KeyMovement = KeyInput;
        }).ScheduleParallel();
        

        // SetSingleton(new PlayerInput{
        //     Space = space,
        //     MouseMovement = MouseInput,
        //     KeyMovement = KeyInput
        // });

    }
}