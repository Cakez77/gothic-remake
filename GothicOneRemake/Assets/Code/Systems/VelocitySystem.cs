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


            //####################################################################
            // Cast a Sphere and see if it collides with anything 
            //####################################################################

            Debug.DrawRay(translation.Value, direction, Color.green, 0.1f);

            // TODO: Put the whole SphereCast into the movement System or create a new system 
            // to handle collisions and calculate the correct velocity
            var colPos = SphereCast(translation.Value, math.normalizesafe(translation.Value + direction, 0) * Time.DeltaTime, 0.5f);

            if (math.length(colPos) > 0) {
                // TODO: Only here for debug reasons
                direction -= colPos;
                //Debug.Log("Rewrite direction: " + colPos);
                //direction = colPos;
            }

            velocity.Value = math.normalizesafe(direction, 0);

        }).WithoutBurst().Run();
    }


    public unsafe float3 SphereCast(float3 RayFrom, float3 RayTo, float radius) {

        var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

        var filter = new CollisionFilter() {
            BelongsTo = 1u << 1, // Belongs to Layer 1
            CollidesWith = 1u << 2, // Collides with Layer 2
            GroupIndex = 0
        };

        CapsuleGeometry sphereGeometry = new CapsuleGeometry() {
            Radius = radius
        };

        BlobAssetReference<Unity.Physics.Collider> sphereCollider = Unity.Physics.CapsuleCollider.Create(sphereGeometry, filter);

        ColliderCastInput input = new ColliderCastInput() {
            Collider = (Unity.Physics.Collider*) sphereCollider.GetUnsafePtr(),
            Orientation = quaternion.identity,
            Start = RayFrom,
            End = RayTo
        };

        ColliderCastHit hit = new ColliderCastHit();

        bool haveHit = collisionWorld.CastCollider(input, out hit);
        if (haveHit) {
            var penetrationPoint = RayTo - hit.SurfaceNormal * 0.5f;
            var penetration = hit.Position - penetrationPoint;

            var moveOut = hit.SurfaceNormal * math.dot(penetration, hit.SurfaceNormal);

            return moveOut;
        }

        // Return null if we dont hit anything
        return float3.zero;
    }
}