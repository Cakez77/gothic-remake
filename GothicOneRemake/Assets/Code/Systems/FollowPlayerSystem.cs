using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[DisableAutoCreation]
public class FollowPlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {

        var playerEntity = GetSingletonEntity<PlayerTag>();
        var followerEntity = GetSingletonEntity<FollowTag>();

        Entities.WithAll<FollowTag>().ForEach((ref Translation translation) =>
        {



            var transforms = GetComponentDataFromEntity<Translation>(false);
            var targetPos = transforms[playerEntity];
            var targetDirection = targetPos.Value - translation.Value;

            translation.Value += targetDirection * Time.DeltaTime;
        }).WithoutBurst().Run();
    }
}