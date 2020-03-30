using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Jobs;
using Unity.Mathematics;

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

        public BufferFromEntity<BufferCollisionDetails> collBuffers;

        public void Execute(CollisionEvent collisionEvent) {

            // Collision occurred
            var collEntities = collisionEvent.Entities;
            var colNormal = collisionEvent.Normal;

            // Update the buffers for each entity, if they have a buffer compoonent
            CheckAndAddBufferToEntity(collBuffers, collEntities.EntityA, colNormal);
            CheckAndAddBufferToEntity(collBuffers, collEntities.EntityB, colNormal);




            /**
            * Function that checks if an entity has a buffer component of type BufferCollisionDetails. 
            * If the entity has such a component it will then be added to it.
            * 
            */
            void CheckAndAddBufferToEntity(BufferFromEntity<BufferCollisionDetails> colBuffers, Entity entity, float3 normal) {
                // Check if the entity has a buffer
                if (colBuffers.Exists(entity)) {
                    var buffer = colBuffers[entity];

                    // Add a buffer element to the entity
                    buffer.Add(new BufferCollisionDetails {
                        CollisionNormal = normal,
                        // TODO: Might be removed in the future or be replaced by the layer ID
                        CollisionPoint = float3.zero
                    });
                }
            }
        }
    }

    protected override void OnUpdate() {

        var collBuffers = GetBufferFromEntity<BufferCollisionDetails>();

        var collisionJob = new CollisionJob() {
            collBuffers = collBuffers,
        }.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);

        collisionJob.Complete();

    }

}