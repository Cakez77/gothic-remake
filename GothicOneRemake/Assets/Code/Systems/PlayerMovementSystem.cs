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
public class PlayerMovementSystem : SystemBase {

    protected override void OnUpdate() {

        // Get the input from mouse and keyboard, stored in the Entity PlayerInput
        var playerInput = GetSingleton<PlayerInput>();
        var movementSpeed = 5f;
        // Get the buffer with collisions affecting the player
        var buffer = GetBufferFromEntity<Float3Buffer>();

        // Find the player Entity
        Entities.WithAll<PlayerTag>().ForEach((Entity playerEntity, ref Translation translation, in LocalToWorld localToWorld, in Velocity velocity) => {

            // Calculate the target position for the palyer to move to using the suppied velocity and the 
            // LocalToWorld component
            
            // Check if the player has anything in his collision buffer
            var playerBuffer = buffer[playerEntity];
            var float3PlayerBuffer = playerBuffer.Reinterpret<float3>();
            float3 targetPosition;
            if(float3PlayerBuffer.Length > 0)
            {
                var direction = math.normalizesafe(localToWorld.Forward,0);
                targetPosition = new float3(0, velocity.Value.y, 0) + (math.cross(float3PlayerBuffer[0], localToWorld.Up) * velocity.Value.z * direction + localToWorld.Right * velocity.Value.x);
                float3PlayerBuffer.RemoveAt(0);
            }else{
                targetPosition = new float3(0, velocity.Value.y, 0) + (localToWorld.Forward * velocity.Value.z + localToWorld.Right * velocity.Value.x);
            }
            
            // Debug.Log("The velocity applied here: " +velocity.Value);
            // Debug.Log("The new targetPosition: " + targetPosition);

            translation.Value += targetPosition * Time.DeltaTime * movementSpeed;
            // TODO: Read up on movement
        }).WithoutBurst().Run();
    }
}