using Unity.Entities;
using UnityEngine;

public class PlayerAndAIComponents : MonoBehaviour, IConvertGameObjectToEntity {

    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float jumpHeight;

    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem) {

        entityManager.AddComponent(entity, typeof(JumpHeight));
        entityManager.AddComponent(entity, typeof(Heading));
        entityManager.AddComponent(entity, typeof(ColAngle));
        entityManager.AddComponent(entity, typeof(JumpForce));
        entityManager.AddComponent(entity, typeof(OnGround));
        entityManager.AddComponent(entity, typeof(MovementAcceleration));


        entityManager.SetComponentData(entity, new MovementAcceleration { AccelerationDuration = 1.5f, MaxSpeed = movementSpeed });
        entityManager.SetComponentData(entity, new JumpHeight { Value = jumpHeight });
        entityManager.SetComponentData(entity, new ColAngle { Value = -1 });
    }
}