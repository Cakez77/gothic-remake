using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TMPForward : IComponentData {
    public float3 Value;
}