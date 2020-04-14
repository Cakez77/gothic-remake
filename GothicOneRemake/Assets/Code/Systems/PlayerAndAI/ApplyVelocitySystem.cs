using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateAfter(typeof(TransformSystemGroup))]
public class ApplyVelocitySystem : SystemBase {
    private float maxSpeed = 10f;
    private float accelerationTime = 1f;
    private float accuSpeed;

    protected override void OnUpdate() {
        var physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;


        float accelerationRate = maxSpeed / accelerationTime;

        Entities.ForEach(
                (ref PhysicsVelocity vel,
                ref OnGround onGround,
                in Heading heading,
                in YVelocity yVel,
                in LocalToWorld ltw,
                in BaseSpeed speed) => {
                    var magnitude = math.length(heading.Value);

                    if (magnitude > 0) {
                        accuSpeed = math.clamp(accuSpeed + accelerationRate * Time.DeltaTime, 0, 6f);
                        vel.Linear.x = ltw.Forward.x * accuSpeed;
                        vel.Linear.z = ltw.Forward.z * accuSpeed;
                    } else {
                        vel.Linear.x = math.lerp(vel.Linear.x, 0, 0.2f);
                        vel.Linear.z = math.lerp(vel.Linear.z, 0, 0.2f);
                        accuSpeed = 0;
                    }




                    var groundNormal = ColliderCast(ltw.Position, ltw.Position + new float3(0, -0.75f, 0));
                    var angle = Vector3.Angle(ltw.Up, groundNormal);
                    var cross = math.cross(ltw.Right, groundNormal);

                    var grounded = math.length(groundNormal) > 0;

                    if (grounded) {
                        if (yVel.Value != 0) {// We jumped
                            vel.Linear.y = yVel.Value;
                        } else { // We did not jump
                            if (cross.y < 0) {
                                if (magnitude == 0) {
                                    vel.Linear.y = math.lerp(vel.Linear.y, magnitude * cross.y * 6.5f, 0.2f); //TODO Check if this is a good value
                                } else {
                                    vel.Linear.y = magnitude * cross.y * 6.5f; //TODO Check if this is a good value
                                }
                            } else {
                                vel.Linear.y = magnitude * cross.y * accuSpeed; //TODO Check if this is a good value
                            }

                        }
                    }

                    onGround.Value = grounded;


                }).WithoutBurst().Run();











        unsafe float3 ColliderCast(float3 rayFrom, float3 rayTo) {

            CollisionFilter filter = new CollisionFilter {
                BelongsTo = 0b_1u, // Layer 0
                CollidesWith = 0b_100u, // Layer 2
                GroupIndex = 0
            };

            SphereGeometry sphere = new SphereGeometry {
                Center = float3.zero,
                Radius = 0.5f
            };

            BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.SphereCollider.Create(sphere, filter);

            ColliderCastInput input = new ColliderCastInput {
                Collider = (Unity.Physics.Collider*) collider.GetUnsafePtr(),
                Orientation = quaternion.identity,
                Start = rayFrom,
                End = rayTo
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