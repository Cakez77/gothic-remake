using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct JumpHeight : IComponentData {
    public float Value;
}

/**
 * This component stores the information where entity is 
 * moving every frame.
 */
public struct Heading : IComponentData {
    public float2 Value;
}

public struct YVelocity : IComponentData {
    public float Value;
}

/**
 * This component stores the angle between the collision
 * object and the player when a collision occures. This is used
 * to determine if the player is grounded, climbing a wall or 
 * in the air(jumping/falling).
 */
public struct ColAngle : IComponentData {
    public float Value;
}

public struct OnGround : IComponentData {
    public bool Value;
}

[WriteGroup(typeof(PhysicsVelocity))]
public struct WaitForRotationTag : IComponentData {}