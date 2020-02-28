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
public class VelocitySystem : SystemBase
{

    protected override void OnUpdate()
    {
        var playerInput = GetSingleton<PlayerInput>();
        var gravity = GetSingleton<Gravity>();
        var grounded = GetSingleton<Grounded>();

        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
        RaycastInput raycastInput;


        Entities.WithAll<PlayerTag>().ForEach((ref Translation translation, ref Velocity velocity) =>
            {
                var fromPosition = translation.Value;
                var relativePosition = new float3(0, -1.1f, 0);
                var currentGravity = gravity.Value;

                raycastInput = new RaycastInput
                {
                    Start = fromPosition,
                    End = fromPosition + relativePosition,
                    Filter = new CollisionFilter
                    {
                        BelongsTo = 10u,
                        CollidesWith = 100u,
                        GroupIndex = 0,
                    }
                };

                Unity.Physics.RaycastHit raycastHit = new Unity.Physics.RaycastHit();

                if (collisionWorld.CastRay(raycastInput, out raycastHit))
                {
                    //We should be grounded so

                    //Hit something, set the position of the player to snap to the ground
                    //by setting the postion of the player to the hit position + playerHeight
                    // TODO: Add playerHeight and make all of this better
                    if (translation.Value.y <= raycastHit.Position.y + 1)
                    {
                        translation.Value.y = raycastHit.Position.y + 1;
                    }

                    currentGravity = 0;
                    Debug.DrawRay(fromPosition, relativePosition, Color.green, 0.2f);
                    Debug.Log("HitPosition: " + raycastHit.Position);

                }
                else
                {
                    Debug.DrawRay(fromPosition, relativePosition, Color.red, 0.2f);
                }

                // Set the velocity and taking into account the jump height
                velocity.Value = new float3(playerInput.KeyMovement.x, playerInput.Space * 4f + currentGravity, playerInput.KeyMovement.y);


            }).WithoutBurst().Run();
    }
}