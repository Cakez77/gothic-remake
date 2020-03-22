using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Unity.Transforms;

/**
This system calculates the velocity for every entity in the game. Included 
in the velocity is gravity and player input.
*/
// TODO: Look into TranslationSystemGroup and if you need to update after it
[UpdateBefore(typeof(PlayerMovementSystem))]
public class VelocitySystem : SystemBase {

    protected override void OnUpdate() {
        // TODO: How about two queries merged into one big query that has all Entities with the
        // TODO: Velocity and the PlayerInput component??
        var playerInput = GetSingleton<PlayerInput>();
        var gravity = GetSingleton<Gravity>();
        var jumpHeight = 5f;
        // var grounded = GetSingleton<Grounded>();

        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
        RaycastInput raycastInput;


        Entities.ForEach((ref Velocity velocity, in LocalToWorld localToWorld) => {

            // Set the velocity and taking into account the jump height
            var input = playerInput.InputVector;
            var direction = localToWorld.Forward * input.x
                            + localToWorld.Up * (jumpHeight * input.y + gravity.Value)
                            + -localToWorld.Right * input.z;

            velocity.Value = math.normalizesafe(direction, 0);

        }).WithoutBurst().Run();
    }
}