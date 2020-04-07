using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public class UpdateHeadingSystem : SystemBase {
    protected override void OnUpdate() {

        var cameraTarget = GetSingletonEntity<CameraTargetTag>();
        var ltwComponents = GetComponentDataFromEntity<LocalToWorld>(true);

        Entities.WithAll<PlayerTag>()
            .WithEntityQueryOptions(EntityQueryOptions.FilterWriteGroup)
            .ForEach((ref PhysicsVelocity velocity, ref ColAngle colAngle, in Heading heading) => {
                var m_velocity = velocity.Linear;
                var m_colAngle = colAngle.Value;

                if (isInRange(m_colAngle, 0, 40)) {

                    moveTo(heading.Value);
                    jumpTo(heading.Value.y);
                }

                velocity.Linear = m_velocity;
                colAngle.Value = m_colAngle;



                //===========================  Simple helper functions  =============================
                // TODO: Create a central helper library
                /**
                 * Function that checks whether a value is within a certain range. 
                 * Returns true is number is within [a, b], false if not
                 */
                bool isInRange(float number, float a, float b) {
                    return number >= a && number <= b;
                }

                /**
                 * Applies velocity along the X and Z Axis. The applies velocity depends
                 * on the nDirection Vector and the speed supplied to the function.
                 */
                void moveTo(float3 _heading) {
                    m_velocity.x = math.lerp(m_velocity.x, _heading.x, 0.3f);
                    m_velocity.z = math.lerp(m_velocity.z, _heading.z, 0.3f);
                }

                /**
                 * Applies velocity along the Y Axis in case the Player is grounded.
                 * The player is grounded if his angle of collision is within a range 
                 * of 0 and 40 degrees. Steeper slopes will cause the player to slide down
                 * or in the case of 90 degrees, fall down.
                 */
                void jumpTo(float _jumpHeight) {
                    m_velocity.y += _jumpHeight;

                    // Set the collisionAngle to -1 to indicate that we are in the air(jumping/falling)
                    m_colAngle = -1;
                }

            }).WithoutBurst().Run();
    }
}