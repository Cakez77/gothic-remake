using Unity.Entities;
using UnityEngine;
using VelocityStateMachine;
using Unity.Burst;

public class PlayerAndAIComponents : MonoBehaviour, IConvertGameObjectToEntity
{

    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float jumpHeight;

    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
    {

        entityManager.AddComponent(entity, typeof(JumpHeight));
        entityManager.AddComponent(entity, typeof(Heading));
        entityManager.AddComponent(entity, typeof(ColAngle));
        entityManager.AddComponent(entity, typeof(JumpForce));
        entityManager.AddComponent(entity, typeof(OnGround));
        entityManager.AddComponent(entity, typeof(GroundNormal));
        entityManager.AddComponent(entity, typeof(VelocityState));
        entityManager.AddComponent(entity, typeof(VelocityEvent));
        entityManager.AddComponent(entity, typeof(TakeoffHeight));
        entityManager.AddComponent(entity, typeof(MovementSpeed));



        entityManager.SetComponentData(entity, new TakeoffHeight { Value = 100f });
        entityManager.SetComponentData(entity, new JumpHeight { Value = jumpHeight });
        entityManager.SetComponentData(entity, new MovementSpeed { Value = movementSpeed });
        entityManager.SetComponentData(entity, new ColAngle { Value = -1 });
        entityManager.SetComponentData(entity, new VelocityState { Name = VelocityStates.Standing, VelocityFunction = BurstCompiler.CompileFunctionPointer<ProcessVelocity>(VelocityFunctions.Stand) });
    }
}