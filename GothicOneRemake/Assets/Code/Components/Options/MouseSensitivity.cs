using Unity.Entities;

[GenerateAuthoringComponent]
public struct MouseSensitivity : IComponentData {
    public float Value;
}