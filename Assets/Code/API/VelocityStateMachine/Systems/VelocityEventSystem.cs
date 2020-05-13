using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

namespace VelocityStateMachine
{
    public class VelocityEventSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(
                (ref VelocityEvent velocityEvent,
                in TakeoffHeight takeoff,
                in LocalToWorld ltw,
                in OnGround onGround,
                in Heading heading,
                in JumpForce jumpForce) =>
                {
                    bool G = onGround.Value;
                    bool H = math.length(heading.Value) > 0;
                    bool J = math.length(jumpForce.Value) > 0;
                    bool F = ltw.Position.y < takeoff.Value;

                    velocityEvent.Value = EventTable(G, F, H, J);

                    
                }).Schedule();

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