using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;


[WriteGroup(typeof(PhysicsVelocity))]
public struct Smooth180Rotation : IComponentData
{
    public float DegreesRotated;
    public float ElapsedTime;
    public float Duration;
    public float3 OriginalVelocity;
}