using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Mathematics;
using UnityEngine;

public class PlayerJumpSystem : SystemBase
{

   

    protected override void OnUpdate()
    {
        // TODO: Refactor this into a component data for the player
        bool canJump;
        float jumpHeight;

        // Entities.WithAll<PlayerTag>.
    }
}