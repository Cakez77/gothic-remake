using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
//[UpdateAfter(typeof(TRSToLocalToWorldSystem))]
public class RotationSystem : SystemBase {
    private float rotationAcc;

    protected override void OnUpdate() {
        var inputX = Input.GetAxis("Vertical");
        var inputY = Input.GetAxis("Horizontal");

        Entities.WithAll<TestTag>().ForEach((ref Rotation rotation) => {


                      
        }).WithoutBurst().Run();
    }
}