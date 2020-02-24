using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct PlayerRotationInfo : IComponentData {
    public LocalToWorld LTW;
    public Translation Translation;
    public float2 PitchYaw;
    public quaternion Rotation;
    public float RotationSpeed;
}