using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
public class AlternateMovementSystem : SystemBase {
    protected override void OnUpdate() {
        var inputX = Input.GetAxis("Vertical");
        var inputY = Input.GetAxis("Horizontal");
        var movementSpeed = 5f;

        Entities.WithAll<PlayerTag>().ForEach((ref PhysicsVelocity velocity, in LocalToWorld localToWorld) => {
            velocity.Angular.y = quaternion.RotateY(math.atan2(inputX, inputY)).value.y;

            velocity.Linear = (localToWorld.Forward * inputX
                                + localToWorld.Right * inputY) * movementSpeed;
                        
        }).WithoutBurst().Run();
    }
}