using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateAfter(typeof(UpdateRotationSystem))]
[UpdateAfter(typeof(RotateByDegreesSystem))]
[UpdateAfter(typeof(TransformSystemGroup))]
[DisableAutoCreation]
public class GroundDetectionSystem : SystemBase {

    protected override void OnUpdate() {
        var physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

        Entities.ForEach(
            (ref YVelocity yvel,
            ref OnGround onGround,
            ref OnSlope onSlope,
            in LocalToWorld ltw) => {

                var groundNormal = ColliderCast(ltw.Position, ltw.Position + new float3(0, -0.75f, 0));
                var angle = Vector3.Angle(ltw.Up, groundNormal);
                var cross = math.cross(ltw.Right, groundNormal);

                onGround.Value = angle > 0 ? true : false;
                onSlope.Value = angle > 0 && angle < 40 ? true : false;

                if(yvel.Value == 0) {// We did not jump
                    yvel.Value = cross.y * 3.5f; //TODO Check if this is a good value
                }

                //========================Debug=============================
                Debug.DrawRay(ltw.Position, cross, Color.red);


            }).WithoutBurst().Run();











        unsafe float3 ColliderCast(float3 rayFrom, float3 rayTo) {

            CollisionFilter filter = new CollisionFilter {
                BelongsTo = 0b_1u,
                CollidesWith = 0b_100u,
                GroupIndex = 0
            };

            SphereGeometry sphere = new SphereGeometry {
                Center = float3.zero,
                Radius = 0.5f
            };

            BlobAssetReference<Unity.Physics.Collider> sphereCollider = Unity.Physics.SphereCollider.Create(sphere, filter);

            ColliderCastInput input = new ColliderCastInput {
                Collider = (Unity.Physics.Collider*) sphereCollider.GetUnsafePtr(),
                Orientation = quaternion.identity,
                Start = rayFrom,
                End = rayTo,

            };

            ColliderCastHit hit = new ColliderCastHit();

            bool haveHit = collisionWorld.CastCollider(input, out hit);
            if (haveHit) {
                Debug.Log("Hit " + collisionWorld.Bodies[hit.RigidBodyIndex].Entity);
                return hit.SurfaceNormal;
            }

            return float3.zero;
        }
    }

}