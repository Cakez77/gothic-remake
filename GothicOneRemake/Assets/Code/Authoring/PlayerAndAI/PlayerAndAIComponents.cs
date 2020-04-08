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

        entityManager.AddComponents(entity, new ComponentTypes(
            typeof(BaseSpeed),
            typeof(JumpHeight),
            typeof(Heading),
            typeof(ColAngle),
            typeof(ColNormal)));

        entityManager.SetComponentData(entity, new BaseSpeed { Value = movementSpeed });
        entityManager.SetComponentData(entity, new JumpHeight { Value = jumpHeight });
        entityManager.SetComponentData(entity, new ColAngle { Value = -1 });
    }
}