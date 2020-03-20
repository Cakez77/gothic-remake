using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;


/*
This class updates the players rotation and position based on
the player input.
*/
// [UpdateAfter(typeof(PlayerCollisionCheck))]
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

            var normal = SphereCast(translation.Value, translation.Value + moveToPos, radius);


            if (math.length(normal) > 0) {
                var pushOut = math.dot(moveToPos, normal) * normal;
                moveToPos -= pushOut * 0.99f;
            }

            translation.Value += moveToPos;

        }).WithoutBurst().Run();
    }

    /*
        * Function to cast a Sphere collider at a direction to check for collision.
        * Right now the collider will only check for collisions with static objects.
        * Those static object are set to be on layer 2.
        */
    public unsafe float3 SphereCast(float3 RayFrom, float3 RayTo, float radius) {

        Debug.DrawRay(RayFrom, RayTo - RayFrom, Color.green);

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

        ColliderCastHit hit = new ColliderCastHit();

        bool haveHit = collisionWorld.CastCollider(input, out hit);
        if (haveHit) {
            return hit.SurfaceNormal;
        }

        // Return null if we dont hit anything
        return float3.zero;
    }
}