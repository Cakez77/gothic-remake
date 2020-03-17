using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public class HandleCollisionsSystem : SystemBase {
    protected override void OnUpdate() {

        var colPointBuffers = GetBufferFromEntity<BufferCollisionDetails>();
        // TODO: Try to read this only once
        var playerInput = GetSingleton<PlayerInput>();

        //TODO: Turn this into a job later
        //TODO: Changed to WithNone
        Entities.WithNone<PlayerTag>().ForEach((Entity playerEntity, ref Velocity velocity, ref Translation translation, in LocalToWorld localToWorld) => {
            // Get the collisionPoint
            // TODO: Handle all collision Points later
            var playerColPointBuffer = colPointBuffers[playerEntity];

            // If a buffer is present on the player
            if (playerColPointBuffer.Length > 0) {

                var colPoint = playerColPointBuffer[0].CollisionPoint;
                var colNormal = playerColPointBuffer[0].CollisionNormal;

                // ##################################################################
                // Push out the player 
                // ##################################################################

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

                // push the translation in the opposite direction of the penetration
                // but keep the player within the collider to make things smother
                var moveOut = colNormal * math.dot(penetration, colNormal);


                // ##################################################################
                // Handle the movement during collision
                // ##################################################################

                // Check whether the velocity will move the player towards or away 
                // from the collision
                var moveTowards = math.dot(velocity.Value, colNormal) < 0;

                if (moveTowards) {

                    // Calculate the parallel vector of the collision 
                    var parallel = math.cross(colNormal, localToWorld.Up);

                    // Project the forward and right direction onto the parallel vector to calculate the velocity
                    var fwdProjected = math.dot(localToWorld.Forward, parallel);
                    var rightProjected = math.dot(localToWorld.Right, parallel);

                    // Calculate the actual direction, meaning a vector for Forward and Right
                    // TODO: Think about just removing the penetration out of the velocity
                    var fwdDirection = parallel * fwdProjected;
                    var rightDirection = parallel * rightProjected;

                    // Set the velocity and taking into account the jump height
                    var input = playerInput.InputVector;
                    var direction = fwdDirection * input.x
                                    + localToWorld.Up * input.y
                                    + -rightDirection * input.z;

                    // Set the new velocity
                    velocity.Value = direction;
                }

                velocity.Value -= moveOut;

                // Dispose of the collision
                playerColPointBuffer.RemoveAt(0);
            }
        }).WithoutBurst().Run();
    }
}