using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Collections;


public class PlayerVelocitySystem : SystemBase {
    protected override void OnUpdate() {

        var playerInput = GetSingleton<PlayerInput>();
        var cameraTarget = GetSingletonEntity<CameraTargetTag>();
        var ltwComponents = GetComponentDataFromEntity<LocalToWorld>(true);

        Entities.WithAll<PlayerTag>().ForEach(
            (ref PhysicsVelocity velocity,
             ref CollisionAngle collisionAngle,
             in JumpHeight jumpHeight,
             in BaseSpeed baseSpeed) => {

                 // Copy needed values;
                 var m_linearVelocity = velocity.Linear;
                 var m_colAngle = collisionAngle.Value;
                 var m_input = playerInput.InputVector;
                 var m_cameraLTW = ltwComponents[cameraTarget];
                 var m_speed = baseSpeed.Value;
                 var m_jumpForce = jumpHeight.Value;

                 // Calculate the direction in which force will be 
                 // applied to the body. This value is use by the helper functions below.
                 var moveDirection = getVelDirection();

                 // Sliding
                 if (m_colAngle < 40) {

                     // Lerp the movement
                     move();

                     // set jump velocity
                     jump();
                 }


                 // Write back important values;
                 velocity.Linear = m_linearVelocity;
                 collisionAngle.Value = m_colAngle;

















                 //--------------------------------------------------------------------------------------
                 //                  Helper functions
                 //--------------------------------------------------------------------------------------

                 /**
                  * Function that calculates the direction in which velocity should be applied to the 
                  * entity. The direction depends on the player input and the LocalToWorld component 
                  * of the camera following the player, as well as a jumpHeight value.
                  */
                 float3 getVelDirection() {

                     // Helper variable because there is no float3.up
                     var up = new float3(0, 1, 0);

                     // Calculate the X and Z direction
                     var direction = m_input.x * zeroOutY(m_cameraLTW.Forward)
                                     + m_input.y * up
                                     + m_input.z * zeroOutY(m_cameraLTW.Right);

                     // Normalize the direction
                     direction = math.normalizesafe(direction, 0);

                     // Add the jump height to the direction
                     direction.y *= m_jumpForce;

                     return direction;
                 }

                 /**
                  * Function that sets the value 0 at the Y position within a 
                  * float3 and returns the new vector.
                  * 
                  */
                 float3 zeroOutY(float3 vector) {
                     return new float3(vector.x, 0, vector.z);
                 }

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
                     m_linearVelocity.x = math.lerp(m_linearVelocity.x, moveDirection.x * m_speed, 0.1f);
                     m_linearVelocity.z = math.lerp(m_linearVelocity.z, moveDirection.z * m_speed, 0.1f);
                 }

                 /**
                  * Applies velocity along the Y Axis in case the Player is grounded.
                  * The player is grounded if his angle of collision is within a range 
                  * of 0 and 40 degrees. Steeper slopes will cause the player to slide down
                  * or in the case of 90 degrees, fall down.
                  */
                 void jump() {
                     // If the collision angle is between 0 and 40 degrees we should be grounded
                     if (isInRange(m_colAngle, 0, 40)) {
                         m_linearVelocity.y = moveDirection.y;

                         // Set the collisionAngle to -1 to indicate that we are in the air(falling)
                         m_colAngle = -1;
                     }
                 }

             }).WithoutBurst().Run();
    }
}