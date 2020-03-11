using Unity.Entities;
using Unity.Mathematics;

public struct BufferCollisionDetails : IBufferElementData {
    public float3 CollisionPoint;
    public float3 CollisionNormal;
}