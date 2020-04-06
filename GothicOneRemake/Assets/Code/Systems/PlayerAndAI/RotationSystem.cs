using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;


public class RotationSystem : SystemBase {
    private bool waitForRotation = false;
    protected override void OnUpdate() {
        Entities.ForEach(
            (ref Rotation rotation, ref PhysicsVelocity velocity, in LocalToWorld localToWorld, in Heading heading) => {
                var magnitude = math.length(heading.Value);
        
                if (magnitude > 0) {
                    // More important values
                    var m_forward = math.forward(rotation.Value);
                    var m_up = math.up();

                    // Calculate the rotation angle
                    var m_rotationAngle = calculateAngle(heading.Value);
                    var m_targetRotation = quaternion.LookRotationSafe(heading.Value, m_up);
                    Debug.DrawRay(localToWorld.Position, heading.Value, Color.red);

                    // if the angle is too big a different rotation method has to be used
                    if (m_rotationAngle > 150) {
                        // TODO: Make this better
                        // Add a component to the entity via a command buffer
                        // Try write groups to achieve the same thing

                        waitForRotation = true;
                    }

                    if (!waitForRotation) {
                        // Just rotate, by lerping
                        rotation.Value = math.slerp(rotation.Value, m_targetRotation, 0.1f);
                        velocity.Linear.x = heading.Value.x;
                        velocity.Linear.y += heading.Value.y;
                        velocity.Linear.z = heading.Value.z;
                    } else {

                        // Rotate by degrees
                        rotation.Value = Quaternion.RotateTowards(rotation.Value, m_targetRotation, 40);
                        if (rotation.Value.value.y == m_targetRotation.value.y) {
                            waitForRotation = false;
                        }
                    }

                    float calculateAngle(float3 _heading) {
                        var plainVelocity = new float3(_heading.x, 0, _heading.z);
                        var angle = Vector3.Angle(m_forward, plainVelocity);
                        return angle;
                    }

                }
                float3 zeroOutY(float3 vector) {
                    return new float3(vector.x, 0f, vector.z);
                }


            }).WithoutBurst().Run();
    }
}