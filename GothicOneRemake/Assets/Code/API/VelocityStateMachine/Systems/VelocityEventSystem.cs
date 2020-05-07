using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;

namespace VelocityStateMachine
{
    public class VelocityEventSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(
                (ref VelocityEvent velocityEvent,
                in VelocityState velocityState,
                in OnGround onGround,
                in Heading heading,
                in JumpForce jumpForce) =>
                {
                    bool G = onGround.Value;
                    bool H = math.length(heading.Value) > 0;
                    bool J = math.length(jumpForce.Value) > 0;
                    bool F = false; // TODO Add support for falling

                    velocityEvent.Value = EventTable(G, F, H, J);

                    
                });

            VelocityEvents EventTable(bool G, bool F, bool H, bool J)
            {
                if(!G && !F || G && J)
                {
                    return VelocityEvents.Jump;
                }

                if(!G && F)
                {
                    return VelocityEvents.Fall;
                }

                if(G && !J && H)
                {
                    return VelocityEvents.Run;
                }

                if(G && !J && !H)
                {
                    return VelocityEvents.Stand;
                }

                return VelocityEvents.Stand;
            }
        }
    }
}