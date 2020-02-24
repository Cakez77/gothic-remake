//     using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using Unity.Jobs;

// [UpdateBefore(typeof(TransformSystemGroup))]
// public class CameraRotateSystem : SystemBase {

//     UnityEngine.Transform cameraTransform;

//     // OnCreate is called once when the System is created,
//     // use this Method for initialization
//     protected override void OnCreate() {
//         // TODO: Why do I need this?
//         base.OnCreate();

//         RequireSingletonForUpdate<PlayerTag>();

//     }

//     protected override void OnStartRunning() {
//         // Since the camera can not be fully converted into an entity,
//         // we need to get the transform of the main camera to access it.
//         cameraTransform = UnityEngine.Camera.main.transform;
//     }

//     protected override void OnUpdate() {
//         float3 targetPosition;

//         Entities.WithNone<PlayerTag>().ForEach((in Translation translation, in Rotation rotation, in LocalToWorld localToWorld) => {

//             targetPosition = translation.Value + localToWorld.Forward * -5.5f;
//             cameraTransform.position = targetPosition;
//             // cameraTransform.position = math.lerp(targetPosition, cameraTransform.position, Time.DeltaTime);
//             cameraTransform.rotation = rotation.Value;
//         }).WithoutBurst().Run();

//     }
// }