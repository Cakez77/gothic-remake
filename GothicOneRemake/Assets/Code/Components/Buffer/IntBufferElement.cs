using Unity.Entities;

public struct IntBufferElement : IBufferElementData {
    // Actual value each buffer element will store.
    public int Value;
}