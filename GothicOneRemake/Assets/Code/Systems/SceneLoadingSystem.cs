using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

public class SceneLoadingSystem : SystemBase
{

    private SceneSystem sceneSystem;

    protected override void OnCreate()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();

    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            sceneSystem.LoadSceneAsync(GameManager.instance.scene.SceneGUID);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            sceneSystem.UnloadScene(GameManager.instance.scene.SceneGUID);
        }
    }
}