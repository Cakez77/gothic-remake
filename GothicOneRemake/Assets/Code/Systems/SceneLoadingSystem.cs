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

    protected override void OnStartRunning() {
        base.OnStartRunning();

        sceneSystem.LoadSceneAsync(GameManager.instance.scene.SceneGUID);
    }

    protected override void OnUpdate()
    {
    }
}