using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class HandleCollisionPointsSystem : SystemBase
{
    protected override void OnUpdate() {

        var colPointBuffers = GetBufferFromEntity<BufferCollisionDetails>();

        //TODO: Turn this into a job later
        Entities.WithAll<PlayerTag>().ForEach((Entity playerEntity, ref Translation translation) => {
            // Get the collisionPoint
            // TODO: Handle all collision Points later
            var playerColPointBuffer = colPointBuffers[playerEntity];

            // If a buffer is present on the player
            if(playerColPointBuffer.Length > 0) {
                var colPoint = playerColPointBuffer[0].CollisionPoint;

                // Calculate the vector from the position to the collision point
                var posToCol = translation.Value - colPoint;

                // Measure the length of the vector 
                // TODO: Might not be needed
                var length = math.length(posToCol);

                // normalize the length to get the general direction
                var nLength = math.normalizesafe(posToCol, 0);

                // stretch the normalized vector to match the radious of the capsule
                var stretched = nLength * 0.5f;

                // Calculate the penetration of the collision
                var penetration = posToCol - stretched;

                //push the translation in the opposite direction of the penetration
                //translation.value -= penetation;
                // TODO: Just to visualize if the calculation is correct
                Debug.DrawRay(colPoint, penetration, Color.yellow, 0.2f);

                // Clean up the buffer
            }

        }).WithoutBurst().Run();
    }
}