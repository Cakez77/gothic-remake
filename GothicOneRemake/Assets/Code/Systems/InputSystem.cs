using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

// [UpdateBefore(typeof(Update))]
[UpdateAfter(typeof(TransformSystemGroup))]
public class InputSystem : SystemBase {

    protected override void OnCreate() {

        // Create an Entity to hold data about the player Input
        EntityManager.CreateEntity(typeof(PlayerInput));
    }

    protected override void OnUpdate() {

        var MouseInput = new float2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        var KeyInput = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        SetSingleton(new PlayerInput{
            MouseMovement = MouseInput,
            KeyMovement = KeyInput
        });

    }
}