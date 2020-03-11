using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


/*
This class updates the players rotation and position based on
the player input.
*/
// [UpdateAfter(typeof(PlayerCollisionCheck))]
[UpdateAfter(typeof(TransformSystemGroup))]
public class PlayerMovementSystem : SystemBase
{

    protected override void OnUpdate()
    {

        // Get the input from mouse and keyboard, stored in the Entity PlayerInput
        var playerInput = GetSingleton<PlayerInput>();
        var movementSpeed = 5f;
        // Get the buffer with collisions affecting the player
        var buffers = GetBufferFromEntity<BufferCollisionDetails>();

        // Find the player Entity
        Entities.WithAll<PlayerTag>().ForEach((Entity playerEntity, ref Translation translation, in Rotation rotation, in LocalToWorld localToWorld, in Velocity velocity) =>
        {

            // Calculate the target position for the palyer to move to using the suppied velocity and the 
            // LocalToWorld component

            // Check if the player has anything in his collision buffer
            var playerBuffer = buffers[playerEntity];
            float3 targetPosition;

            if (playerBuffer.Length > 0)
            {
                // Calculate the direction to walk to based on the normal of the collision.
                // TODO: Add multiple collisions, cross of all normals maybe
                var direction = math.cross(playerBuffer[0].CollisionNormal, localToWorld.Up) * localToWorld.Forward * localToWorld.Right;

                // Calculate the velocity
                var mulXY = velocity.Value.x * velocity.Value.z;
                var addXY = velocity.Value.x + velocity.Value.z;
                var vel = mulXY != 0 ? mulXY : addXY;

                // *Debug Info: Draw a ray pointing into the direction 
                Debug.DrawRay(translation.Value, direction, Color.green, 0.2f);

                // Set the target position based on velocity
                targetPosition = vel * direction;

                // Remove the buffer element from the player
                playerBuffer.RemoveAt(0);
            }
            else
            {
                // calculate direction based on rotation
                var direction = localToWorld.Forward;

                // *Debug Info: Draw a ray pointing into the direction
                Debug.DrawRay(translation.Value, direction, Color.red, 0.2f);

                // Set the target position based on velocity
                targetPosition = new float3(0, velocity.Value.y, 0) + (localToWorld.Forward * velocity.Value.z + localToWorld.Right * velocity.Value.x);
            }

            translation.Value += targetPosition * Time.DeltaTime * movementSpeed;

        }).WithoutBurst().Run();
    }
}