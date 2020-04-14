using Unity.Entities;

[GenerateAuthoringComponent]
public struct BaseSpeed : IComponentData {
    public float Value;
    public BaseSpeed Default => new BaseSpeed{Value = 5f };
}



