using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Collections;


public class UpdateHeadingSystem : SystemBase {
    protected override void OnUpdate() {

        var cameraTarget = GetSingletonEntity<CameraTargetTag>();
        var ltwComponents = GetComponentDataFromEntity<LocalToWorld>(true);

        Entities.WithAll<PlayerTag>().ForEach(
            (ref PhysicsVelocity velocity,
             ref ColAngle collisionAngle,
             in Heading heading,
             in JumpHeight jumpHeight,
             in BaseSpeed baseSpeed) => {

                 // Copy needed values;
                 var m_linearVelocity = velocity.Linear;
                 var m_colAngle = collisionAngle.Value;
                 var m_heading = heading.Value;
                 var m_jumpForce = jumpHeight.Value;
                 var m_speed = baseSpeed.Value;

                 // Jumping and moving is only possible on
                 // slopes that are flatter than 40°
                 if (isInRange(m_colAngle, 0, 40)) {

                     move();
                     jump();
                 }


                 // Write back important values;
                 velocity.Linear = m_linearVelocity;
                 collisionAngle.Value = m_colAngle;

















                 //--------------------------------------------------------------------------------------
                 //                  Helper functions
                 //--------------------------------------------------------------------------------------

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
                 void move() {
                     m_linearVelocity.x = math.lerp(m_linearVelocity.x, m_heading.x * m_speed, 0.1f);
                     m_linearVelocity.z = math.lerp(m_linearVelocity.z, m_heading.z * m_speed, 0.1f);
                 }

                 /**
                  * Applies velocity along the Y Axis in case the Player is grounded.
                  * The player is grounded if his angle of collision is within a range 
                  * of 0 and 40 degrees. Steeper slopes will cause the player to slide down
                  * or in the case of 90 degrees, fall down.
                  */
                 void jump() {
                     // Apply force on the Y-Axis relative
                     m_linearVelocity.y = m_heading.y * m_jumpForce;

                     // Set the collisionAngle to -1 to indicate that we are in the air(jumping/falling)
                     m_colAngle = -1;
                 }

             }).WithoutBurst().Run();
    }
}