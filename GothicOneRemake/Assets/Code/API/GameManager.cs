using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Scenes;

public class GameManager : MonoBehaviour
{
    public SubScene scene;

    public GameObject playerPrefab;

    private EntityManager entityManager;

    // public SubScene Scene
    // {
    //     get
    //     {
    //         return scene;
    //     }
    // }

    public static GameManager instance;

    void Awake()
    {
        // Singleton
        instance = this;

        // Get the EntityManager to instantiate Entities
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype options = entityManager.CreateArchetype(
            typeof(MouseSensitivity),
            typeof(PitchYaw),
            typeof(Gravity)
        );

        Entity optionsEntity = entityManager.CreateEntity(options);
        entityManager.SetComponentData(optionsEntity, new MouseSensitivity { Value = 0.5f });
        entityManager.SetName(optionsEntity, "Options");


        Entity playerInput = entityManager.CreateEntity();
        entityManager.AddComponent(playerInput, typeof(PlayerInput));
        entityManager.SetComponentData(playerInput, new PlayerInput());
        entityManager.SetName(playerInput, "Input");


        // Adding buffer to entity
        //entityManager.AddBuffer<BufferCollisionDetails>(playerEntity);

        var assetStore = GetComponent<BlobAssetStore>();
        

        var gOCSettings = 

        // Instantiate a prefab entity
        Entity entity =  GameObjectConversionUtility.ConvertGameObjectHierarchy(playerPrefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, assetStore));
    }
}