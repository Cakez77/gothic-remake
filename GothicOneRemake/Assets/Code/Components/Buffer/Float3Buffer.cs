using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Float3Buffer : IBufferElementData {
    public float3 Value;
}