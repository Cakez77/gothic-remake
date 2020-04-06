using Unity.Entities;

[GenerateAuthoringComponent]
public struct MouseSensitivity : IComponentData {
    public float Value;
}

public struct RotationSmothnes : IComponentData {
    public float Value;
}