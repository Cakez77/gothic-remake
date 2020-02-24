using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PlayerInput : IComponentData {
    public float2 MouseMovement;
    public float2 KeyMovement;
}