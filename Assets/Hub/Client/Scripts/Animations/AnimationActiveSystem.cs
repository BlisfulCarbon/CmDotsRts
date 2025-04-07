using Unity.Entities;
using Unity.Rendering;

namespace Hub.Client.Scripts.Animations
{
    public partial struct AnimationActiveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            new AnimationActiveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                Animations = SystemAPI.GetSingleton<AnimationDataHolder>().Animations,
            }.ScheduleParallel();

            // AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();
            //
            // foreach ((
            //              RefRW<ActiveAnimation> activeAnimation,
            //              RefRW<MaterialMeshInfo> meshInfo)
            //          in SystemAPI.Query<
            //              RefRW<ActiveAnimation>,
            //              RefRW<MaterialMeshInfo>>())
            // {
            //     ref AnimationData animationData =
            //         ref animationDataHolder.Animations.Value[(int)activeAnimation.ValueRO.AnimationID];
            //
            //     activeAnimation.ValueRW.FrameTimer += SystemAPI.Time.DeltaTime;
            //
            //     if (activeAnimation.ValueRO.FrameTimer > animationData.FrameTimerMax)
            //     {
            //         activeAnimation.ValueRW.FrameTimer -= animationData.FrameTimerMax;
            //
            //         if (animationData.FrameMax == 0)
            //             continue;
            //
            //         activeAnimation.ValueRW.Frame =
            //             (activeAnimation.ValueRO.Frame + 1) % animationData.FrameMax;
            //
            //         meshInfo.ValueRW.MeshID =
            //             animationData.BatchMeshId[activeAnimation.ValueRO.Frame];
            //
            //
            //         if (activeAnimation.ValueRO.Frame == 0 &&
            //             (activeAnimation.ValueRO.AnimationID == AnimationSO.AnimationID.SoldierShoot || 
            //              activeAnimation.ValueRO.AnimationID == AnimationSO.AnimationID.ZombieMeleeAttack))
            //         {
            //             activeAnimation.ValueRW.AnimationID = AnimationSO.AnimationID.None;
            //         }
            //     }
            // }
        }
    }
    
    public partial struct AnimationActiveJob : IJobEntity
    {
        public float DeltaTime;
        public BlobAssetReference<BlobArray<AnimationData>> Animations;
        
        public void Execute(ref ActiveAnimation animation, ref MaterialMeshInfo mesh)
        {
                ref AnimationData animationData =
                    ref Animations.Value[(int)animation.AnimationID];

                animation.FrameTimer += DeltaTime;

                if (animation.FrameTimer > animationData.FrameTimerMax)
                {
                    animation.FrameTimer -= animationData.FrameTimerMax;

                    if (animationData.FrameMax == 0)
                        return;

                    animation.Frame =
                        (animation.Frame + 1) % animationData.FrameMax;

                    mesh.MeshID =
                        animationData.BatchMeshId[animation.Frame];

                    if (animation.Frame == 0 &&
                        (animation.AnimationID == AnimationSO.AnimationID.SoldierShoot || 
                         animation.AnimationID == AnimationSO.AnimationID.ZombieMeleeAttack))
                    {
                        animation.AnimationID = AnimationSO.AnimationID.None;
                    }
                } 
        }
 
    }
}