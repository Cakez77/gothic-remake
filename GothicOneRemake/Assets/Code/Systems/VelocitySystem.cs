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
        // var grounded = GetSingleton<Grounded>();

        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
        RaycastInput raycastInput;


        Entities.ForEach((ref Velocity velocity, in LocalToWorld localToWorld, in Translation translation, in Rotation rotation) => {
            var fromPosition = translation.Value;
            var relativePosition = new float3(0, -1.1f, 0);
            var currentGravity = gravity.Value;

            //raycastInput = new RaycastInput {
            //    Start = fromPosition,
            //    End = fromPosition + relativePosition,
            //    Filter = new CollisionFilter {
            //        BelongsTo = 0b10, // Belongs to Layer 11
            //        CollidesWith = 0b100, // Collides with Layer 2
            //        GroupIndex = 0,
            //    }
            //};

            //Unity.Physics.RaycastHit raycastHit = new Unity.Physics.RaycastHit();

            //if (collisionWorld.CastRay(raycastInput, out raycastHit)) {
            //    //We should be grounded so

            //    Debug.Log("We hit something: " + buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity);

            //    //Hit something, set the position of the player to snap to the ground
            //    //by setting the postion of the player to the hit position + playerHeight
            //    // TODO: Add playerHeight and make all of this better
            //    if (translation.Value.y <= raycastHit.Position.y + 1) {
            //        // This is making me fly when not colliding with something on the ground
            //        // and no gravity is applied.
            //        // TODO: Commented out the line below for later review
            //        // translation.Value.y = raycastHit.Position.y + 1;
            //    }

            //    currentGravity = 0;
            //    // TODO: Only here for debug reasons
            //    // Debug.DrawRay(fromPosition, relativePosition, Color.green, 0.2f);
            //    // Debug.Log("HitPosition: " + raycastHit.Position);

            //} else {
            //    Debug.DrawRay(fromPosition, relativePosition, Color.red, 0.2f);
            //}

            // Set the velocity and taking into account the jump height
            var input = playerInput.InputVector;
            var direction = localToWorld.Forward * input.x
                            + localToWorld.Up * input.y
                            + -localToWorld.Right * input.z;

            velocity.Value = math.normalizesafe(direction, 0);

        }).WithoutBurst().Run();
    }
}