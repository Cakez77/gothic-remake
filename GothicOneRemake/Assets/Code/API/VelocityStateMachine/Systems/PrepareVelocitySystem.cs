using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Entities;

namespace VelocityStateMachine
{
    public class PrepareVelocitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(
                (ref PhysicsVelocity physicsVelocity,
                ref TakeoffHeight takeoff,
                in LocalToWorld ltw,
                in GroundNormal normal) =>
                {
                    bool grounded = math.length(normal.Value) > 0;

                    if (grounded)
                    {
                        float slope = GetSlope(ltw.Right, normal.Value);
                        physicsVelocity.Linear.y = slope;
                        takeoff.Value = ltw.Position.y;
                    }

                }).Schedule();
        }

        private float GetSlope(float3 right, float3 normal)
        {
            return math.cross(right, normal).y;
        }
    }
}