using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

[DisableAutoCreation]
[UpdateAfter(typeof(InputProcessingSystem))]
public class HeadingOnRampsSystem : SystemBase {
    protected override void OnUpdate() {

        Entities.ForEach((ref YVelocity gravity, in ColNormal colNormal, in LocalToWorld ltw) => {
            var normal = colNormal.Value;

            if (Magnitude(normal) > 0 && gravity.Value == 0) {

                var cross = math.cross(ltw.Right, colNormal.Value);

                if (cross.y < 0) {
                    gravity.Value = cross.y;
                }

            }


            float Magnitude(float3 vector) {
                return math.length(normal);
            }
        }).WithoutBurst().Run();
    }
}