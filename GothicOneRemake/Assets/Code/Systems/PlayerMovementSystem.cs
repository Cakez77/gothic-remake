// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;

// /*
// This class updates the players rotation and position based on
// the player input.
// */
// public class PlayerMovementSystem : SystemBase {
    
//     protected override void OnUpdate() {

//         // Get the input from mouse and keyboard, stored in the Entity PlayerInput
//         var playerInput = GetSingleton<PlayerInput>();

//         // This holds the accumulated rotation along the X and Y-Axis 
//         var pitchYaw = GetSingleton<PitchYaw>();

//         // Find the player Entity
//         Entities.WithAll<PlayerTag>().ForEach((ref Translation translation, ref Rotation rotation) =>{

//             // Rotate the player along the Y-Axis 
//             rotation.Value = quaternion.RotateY(pitchYaw.Value.y);

//             // Move the player by ...
//             // TODO: Read up on movement
//         }).Schedule();
//     }
// }