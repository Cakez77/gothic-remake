using Unity.Entities;
using Unity.Transforms;


[UpdateAfter(typeof(TransformSystemGroup))]
public class CameraRotaionSystem : SystemBase {
    UnityEngine.Transform cameraTransform;

    protected override void OnCreate() {

    }

    protected override void OnStartRunning() {
        cameraTransform = UnityEngine.Camera.main.transform;
    }

    protected override void OnUpdate() {

        var playerRotationInfo = GetSingleton<PlayerRotationInfo>();
        var cameraEntity = GetSingletonEntity<CameraTag>();
        var translations = GetComponentDataFromEntity<Translation>(false);

        Entities.ForEach((in PlayerRotationInfo playerInfo) => {
            // rotation.Value = playerRotationInfo.Rotation;
            // translation.Value = playerRotationInfo.LTW.Position + playerRotationInfo.LTW.Forward * -2.5f;

            var cameraPosition = translations[cameraEntity];
            cameraPosition.Value = playerInfo.LTW.Position +  playerInfo.LTW.Forward * -5f;

            translations[cameraEntity] = cameraPosition;

            cameraTransform.rotation = playerRotationInfo.Rotation;
            // cameraTransform.position = playerRotationInfo.LTW.Position + playerRotationInfo.LTW.Forward * -5f;
        }).WithoutBurst().Run();
    }
}