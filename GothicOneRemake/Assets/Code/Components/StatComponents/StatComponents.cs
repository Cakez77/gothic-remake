using Unity.Entities;

[GenerateAuthoringComponent]
public struct BaseSpeed : IComponentData {
    public float Value;
}

[GenerateAuthoringComponent]
public struct JumpHeight : IComponentData {
    public float Value;
}

