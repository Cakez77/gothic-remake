using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;


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
                 in BaseSpeed speed,
                 in JumpHeight jumpHeight) => {

                     var currentVel = velocity.Linear;
                     var m_colAngle = colAngle.Value;
                     var dir = heading.Value;
                     var height = jumpHeight.Value;



                     if (m_colAngle < 40) {
                         Move(speed.Value);
                     }

                     if (isInRange(m_colAngle, 0, 40)) {
                         Jump();
                     }


                     velocity.Linear = ApplyVelocity(velocity.Linear);
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
                         currentVel = math.lerp(currentVel, dir * moveSpeed, 0.3f);
                     }

                     /**
                      * Applies velocity along the Y Axis in case the Player is grounded.
                      * The player is grounded if his angle of collision is within a range 
                      * of 0 and 40 degrees. Setting -1 as colAngle ensures that this function
                      * won't be called, even it height > 0
                      */
                     void Jump() {
                         if (height > 0) {

                             m_colAngle = -1;

                             currentVel.y = height;
                         }
                     }

                     float3 ApplyVelocity(float3 vel) {
                         var finalVel = vel;

                         finalVel.x = currentVel.x;
                         finalVel.z = currentVel.z;

                         // Bewege ich mich auf einer Rampe oder bin ich gesprungen?
                         if (dir.y != 0 || currentVel.y == height) {
                             finalVel.y = currentVel.y;
                         }

                         return finalVel;
                     }

                 }).Schedule();
    }
}