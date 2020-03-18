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
        var dTime = Time.DeltaTime;
        // Get the buffer with collisions affecting the player
        var buffers = GetBufferFromEntity<BufferCollisionDetails>();

        //Update the position of all entities with a translation and a velocity
        Entities.ForEach((ref Translation translation, in Velocity velocity) =>
        {
            translation.Value += velocity.Value * movementSpeed * dTime;
        }).WithoutBurst().Run();
    }
}