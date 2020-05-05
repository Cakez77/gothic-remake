using Unity.Entities;
using Unity.Mathematics;

public struct PitchYaw : IComponentData {
    public float2 Value;
}

public struct RotationSpeed : IComponentData {
    public float Value;
}

public struct PlayerDistance : IComponentData {
    public float Value;
}

public struct CameraFOV : IComponentData
{
    public float Value;
}

public struct TakeoffHeight : IComponentData
{
    public float Value;
}