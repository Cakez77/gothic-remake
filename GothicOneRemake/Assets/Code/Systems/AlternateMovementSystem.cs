using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Unity.Transforms;


public class AlternateMovementSystem : SystemBase {
    public float lerpValue = 1;
    public float accTime = 0;
    protected override void OnUpdate() {
        var playerInput = GetSingleton<PlayerInput>();
        var movementSpeed = 5f;

        Entities.WithAll<PlayerTag>().ForEach((ref PhysicsVelocity velocity, ref Rotation rotation, in LocalToWorld localToWorld) => {
            var input = playerInput.InputVector;

            var magnitude = math.length(input);

            var result = math.atan2(input.z, input.x);

            var dir = quaternion.RotateY(result);

            //-------------------- Debug ---------------------------
            var targetRotation = math.normalizesafe(input.x * localToWorld.Forward + input.z * localToWorld.Right, 0);
            var currentRotation = math.normalizesafe(localToWorld.Forward, 0);

            Debug.DrawRay(localToWorld.Position, targetRotation, Color.red);
            Debug.DrawRay(localToWorld.Position, currentRotation, Color.green);

            Debug.Log("Angle: " + Vector3.Angle(currentRotation, targetRotation));
            //-------------------------------------------------------

            if (magnitude > 0) {
                //accTime += Time.DeltaTime;
                //if(accTime > 1) {
                //    rotation.Value = math.nlerp(rotation.Value, dir, 1);
                //    accTime = 0;
                //} else {
                //    rotation.Value = math.nlerp(rotation.Value, dir, accTime);
                //}
                rotation.Value = math.nlerp(rotation.Value, dir, 0.1f);

            }




            velocity.Linear = math.normalizesafe(localToWorld.Forward * math.length(input), 0) * movementSpeed;

        }).WithoutBurst().Run();
    }
}