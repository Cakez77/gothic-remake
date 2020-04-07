using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
public class UpdateHeadingSystem : SystemBase {
    protected override void OnUpdate() {

        var cameraTarget = GetSingletonEntity<CameraTargetTag>();
        var ltwComponents = GetComponentDataFromEntity<LocalToWorld>(true);

        Entities
            .WithEntityQueryOptions(EntityQueryOptions.FilterWriteGroup)
            .ForEach(
                (ref PhysicsVelocity velocity,
                 ref ColAngle colAngle,
                 in Heading heading,
                 in LocalToWorld ltw,
                 in BaseSpeed speed,
                 in JumpHeight jumpHeight) => {

                     var m_heading = heading.Value;
                     var m_velocity = velocity.Linear;
                     var m_colAngle = colAngle.Value;



                     if (m_colAngle < 40) {
                         Move(speed.Value);
                     }

                     if (isInRange(m_colAngle, 0, 40)) {
                         Jump(jumpHeight.Value);
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
                     void Move(float moveSpeed) {
                         m_velocity.x = math.lerp(m_velocity.x, m_heading.x * moveSpeed, 0.3f);
                         m_velocity.z = math.lerp(m_velocity.z, m_heading.z * moveSpeed, 0.3f);
                     }

                     /**
                      * Applies velocity along the Y Axis in case the Player is grounded.
                      * The player is grounded if his angle of collision is within a range 
                      * of 0 and 40 degrees. Steeper slopes will cause the player to slide down
                      * or in the case of 90 degrees, fall down.
                      */
                     void Jump(float height) {
                         m_velocity.y += math.lerp(m_heading.y, m_heading.y * height, 0.5f);

                         // Set the collisionAngle to -1 to indicate that we are in the air(jumping/falling)
                         m_colAngle = -1;
                     }

                 }).WithoutBurst().Run();
    }
}