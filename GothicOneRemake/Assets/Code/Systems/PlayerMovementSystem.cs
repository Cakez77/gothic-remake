using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Unity.Collections;


/*
This class updates the players rotation and position based on
the player input.
*/
// [UpdateAfter(typeof(PlayerCollisionCheck))]
[DisableAutoCreation]
[UpdateAfter(typeof(TransformSystemGroup))]
public class PlayerMovementSystem : SystemBase {


    protected override void OnUpdate() {
        
        // Get the input from mouse and keyboard, stored in the Entity PlayerInput
        var playerInput = GetSingleton<PlayerInput>();
        var movementSpeed = 5f;
        var dTime = Time.DeltaTime;
        // Get the buffer with collisions affecting the player
        var buffers = GetBufferFromEntity<BufferCollisionDetails>();

        //Update the position of all entities with a translation and a normalized!!! velocity
        Entities.ForEach((ref Translation translation, in Velocity velocity) => {

            //####################################################################
            // Cast a Sphere and see if it collides with anything 
            //####################################################################
            var radius = 0.5f;

            var moveToPos = velocity.Value * movementSpeed * dTime;

            var colliderHits = SphereCast(translation.Value, translation.Value + moveToPos, radius);
            var upVector = new float3(0f, 1f, 0f);
            var yDirection = upVector * moveToPos;
            var rayDirection = math.normalizesafe(yDirection * upVector, 0);

            var groundNormal = RayCast(translation.Value, translation.Value + rayDirection);


            if (colliderHits.Length > 0) {
                var addedNormal = float3.zero;
                var pushOut = float3.zero;
                for (int i = 0; i < colliderHits.Length; i++) {
                    addedNormal += colliderHits[i].SurfaceNormal;;
                }
                Debug.DrawRay(translation.Value, addedNormal, Color.blue);

                pushOut += math.dot(moveToPos, addedNormal) * addedNormal;
                moveToPos -= pushOut;
            }

            if (math.length(groundNormal) > 0) {
                var pushOut = math.dot(moveToPos, groundNormal) * groundNormal;
                moveToPos -= pushOut;
            }

            translation.Value += moveToPos;

        }).WithoutBurst().Run();
        
    }

    /*
        * Function to cast a Sphere collider at a direction to check for collision.
        * Right now the collider will only check for collisions with static objects.
        * Those static object are set to be on layer 2.
        */
    public unsafe NativeList<ColliderCastHit> SphereCast(float3 RayFrom, float3 RayTo, float radius) {

        var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

        var filter = new CollisionFilter() {
            BelongsTo = 1u, // Belongs to Layer 0 (Player)
            CollidesWith = 1u << 2, // Collides with Layer 2 (Static Objects)
            GroupIndex = 0
        };

        SphereGeometry geometry = new SphereGeometry() {
            Center = float3.zero,
            Radius = radius
        };

        BlobAssetReference<Unity.Physics.Collider> sphereCollider = Unity.Physics.SphereCollider.Create(geometry, filter);

        ColliderCastInput input = new ColliderCastInput() {
            Collider = (Unity.Physics.Collider*) sphereCollider.GetUnsafePtr(),
            Orientation = quaternion.identity,
            Start = RayFrom,
            End = RayTo
        };

        NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(3, Allocator.Temp);

        bool haveHit = collisionWorld.CastCollider(input, ref hits);
        if (haveHit) {
            return hits;
        }

        // Return null if we dont hit anything
        return hits;
    }

    /*
    * Function to cast a Sphere collider at a direction to check for collision.
    * Right now the collider will only check for collisions with static objects.
    * Those static object are set to be on layer 2.
    */
    public float3 RayCast(float3 RayFrom, float3 RayTo) {

        Debug.DrawRay(RayFrom, RayTo - RayFrom, Color.red);

        var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

        RaycastInput raycastInput = new RaycastInput {
            Start = RayFrom,
            End = RayTo,
            Filter = new CollisionFilter {
                BelongsTo = 1u, // Belongs to Layer 0 (Player)
                CollidesWith = 1u << 2, // Collides with Layer 2 (Static Objects)
                GroupIndex = 0
            }
        };

        Unity.Physics.RaycastHit hit = new Unity.Physics.RaycastHit();

        if (collisionWorld.CastRay(raycastInput, out hit)) {

            //We should be grounded so
            return hit.SurfaceNormal;
        }

        return float3.zero;
    }
}