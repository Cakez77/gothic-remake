using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace VelocityStateMachine
{
    public class GroundCheckingSystem : SystemBase
    {
        private CollisionWorld _collisionWorld;
        private BuildPhysicsWorld _buildPhysicsWorld;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _buildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
            _collisionWorld = _buildPhysicsWorld.PhysicsWorld.CollisionWorld;
        }

        protected override void OnUpdate()
        {

            Entities.ForEach(
                (ref GroundNormal groundNormal,
                ref OnGround onGround,
                ref TakeoffHeight takeoff,
                in LocalToWorld ltw) =>
                {
                    var position = ltw.Position;
                    var normal = ColliderCast(position, position + new float3(0f, -0.6f, 0f));
                    bool grounded = math.length(normal) > 0;

                    if (grounded)
                    {
                        takeoff.Value = ltw.Position.y;
                    }

                    groundNormal.Value = normal;
                    onGround.Value = grounded;

                }).WithoutBurst().Run();
        }


        private unsafe float3 ColliderCast(float3 rayFrom, float3 rayTo)
            {

                CollisionFilter filter = new CollisionFilter
                {
                    BelongsTo = 0b_1u, // Layer 0
                    CollidesWith = 0b_100u, // Layer 2
                    GroupIndex = 0
                };

                SphereGeometry sphere = new SphereGeometry
                {
                    Center = float3.zero,
                    Radius = 0.5f
                };

                BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.SphereCollider.Create(sphere, filter);

                ColliderCastInput input = new ColliderCastInput
                {
                    Collider = (Unity.Physics.Collider*) collider.GetUnsafePtr(),
                    Orientation = quaternion.identity,
                    Start = rayFrom,
                    End = rayTo
                };

                ColliderCastHit hit = new ColliderCastHit();

                bool haveHit = _collisionWorld.CastCollider(input, out hit);

                if (haveHit)
                {
                    return hit.SurfaceNormal;
                }

                return float3.zero;
            }
    }
}