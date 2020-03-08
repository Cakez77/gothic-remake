using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Scenes;

public class GameManager : MonoBehaviour
{
    public SubScene scene;

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
    }


}