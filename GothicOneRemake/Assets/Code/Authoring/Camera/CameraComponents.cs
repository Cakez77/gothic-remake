using Unity.Entities;
using UnityEngine;

public class CameraComponents : MonoBehaviour, IConvertGameObjectToEntity {

    [SerializeField]
    private float rotationSpeed = 5f;
    
    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem) {

        entityManager.AddComponents(entity, new ComponentTypes(
            typeof(PitchYaw),
            typeof(PlayerDistance),
            typeof(RotationSpeed)));

        entityManager.SetComponentData(entity, new PlayerDistance { Value = 5.5f });
        entityManager.SetComponentData(entity, new RotationSpeed { Value = rotationSpeed });
    }
}