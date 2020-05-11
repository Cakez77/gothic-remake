using UnityEngine;
using Unity.Physics;
using Unity.Entities;

namespace VelocityStateMachine
{
    public class UpdateVelocitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var run = Input.GetKey(KeyCode.T);
            Entities.WithAll<PlayerTag>().ForEach((ref PhysicsVelocity vel) =>
            {
                if (run)
                {
                    vel.Linear.x = 1f;
                }
            }).Schedule();


        }
    }

}