using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;

public class PlayerAndAIComponents : MonoBehaviour, IConvertGameObjectToEntity {

    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float jumpHeight;

    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem) {

        var playerAndUIArchetype = entityManager.CreateArchetype(
            typeof(BaseSpeed), 
            typeof(JumpHeight),
            typeof(Heading),
            typeof(ColAngle),
            typeof(ColNormal),
            typeof(YVelocity),
            typeof(OnGround),
            typeof(OnSlope));

        entityManager.AddComponent(entity, typeof(BaseSpeed));
        entityManager.AddComponent(entity, typeof(JumpHeight));
        entityManager.AddComponent(entity, typeof(Heading));
        entityManager.AddComponent(entity, typeof(ColAngle));
        entityManager.AddComponent(entity, typeof(ColNormal));
        entityManager.AddComponent(entity, typeof(YVelocity));
        entityManager.AddComponent(entity, typeof(OnGround));
        entityManager.AddComponent(entity, typeof(OnSlope));

        entityManager.SetComponentData(entity, new BaseSpeed { Value = movementSpeed });
        entityManager.SetComponentData(entity, new JumpHeight { Value = jumpHeight });
        entityManager.SetComponentData(entity, new ColAngle { Value = -1 });
    }
}