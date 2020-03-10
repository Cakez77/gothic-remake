using Unity.Entities;

[InternalBufferCapacity(10)]
[GenerateAuthoringComponent]
public struct IntBufferElement : IBufferElementData {
    // Actual value each buffer element will store.
    public int Value;
}