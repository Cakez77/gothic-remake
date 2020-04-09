using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

public class HeadingOnRampsSystem : SystemBase {
    protected override void OnUpdate() {


        Entities.ForEach((ref Heading heading, in ColNormal colNormal, in ColAngle colAngle, in LocalToWorld ltw) => {

            var m_heading = heading.Value;

            var cross = math.cross(ltw.Right, colNormal.Value);
            var projected = math.dot(heading.Value, cross)*cross;
            // Linear Velocity.y is 7, next frame heading.Value.y will be != 0 -> LinearVelocity.y will be set to almost 0
            // Is the player jumping?
            if(colAngle.Value == -1) {
                heading.Value.y = 0f;
            } else {
                if(projected.y < 0) {
                    heading.Value.y = projected.y;
                }
                
            }
            

        }).WithoutBurst().Run();
    }
}