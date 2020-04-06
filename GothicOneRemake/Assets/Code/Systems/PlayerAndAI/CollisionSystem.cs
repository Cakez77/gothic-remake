using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;


public class CollistionSystem : SystemBase {

    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate() {
        base.OnCreate();

        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }


    
    // This job handles all collisions within the world
    private struct CollisionJob : ICollisionEventsJob {

        public ComponentDataFromEntity<ColAngle> colAngles;
        [ReadOnly] public ComponentDataFromEntity<LocalToWorld> ltw;

        public void Execute(CollisionEvent collisionEvent) {

            // Collision occurred
            var collEntities = collisionEvent.Entities;
            var colNormal = collisionEvent.Normal;
            var entityA = collEntities.EntityA;
            var entityB = collEntities.EntityB;

            if(colAngles.Exists(entityA)){
                setColAngle(entityA, colAngles, ltw[entityA].Up);
            }

            if (colAngles.Exists(entityB)) {
                setColAngle(entityB, colAngles, ltw[entityB].Up);
            }



            //------------------------------------------------------------------------------------------
            // Helper Functions
            // -----------------------------------------------------------------------------------------

            float CalculateAngle(float3 normal, float3 up) {

                var angle = Vector3.Angle(up, normal);

                return angle;
            }

            void setColAngle(Entity entity, ComponentDataFromEntity<ColAngle> colAngles, float3 ltwUp) {
                var angle = colAngles[collEntities.EntityA];
                angle.Value = CalculateAngle(colNormal, ltwUp);
                colAngles[collEntities.EntityA] = angle;
            }
        }
    }

    protected override void OnUpdate() {

        var colAngles = GetComponentDataFromEntity<ColAngle>(false);
        var ltw = GetComponentDataFromEntity<LocalToWorld>(true);

        var collisionJob = new CollisionJob() {
            colAngles = colAngles,
            ltw = ltw
        }.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);

        collisionJob.Complete();

    }

}