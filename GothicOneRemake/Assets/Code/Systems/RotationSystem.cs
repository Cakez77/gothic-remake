using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
public class RotationSystem : SystemBase {
    private float rotationAcc;

    protected override void OnUpdate() {

        Entities.WithAll<TestTag>().ForEach((ref Rotation rotation) => {
            rotationAcc += Time.DeltaTime;
            rotation.Value = quaternion.RotateY(rotationAcc);
            
        }).WithoutBurst().Run();
    }
}