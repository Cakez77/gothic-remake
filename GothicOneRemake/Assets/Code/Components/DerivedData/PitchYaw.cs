using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PitchYaw : IComponentData {
    public float2 Value;
}