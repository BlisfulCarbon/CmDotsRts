using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Hub.Client.Scripts.Animations
{
    public partial struct AnimationActiveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AnimationDataHolder>();
        }

        public void OnUpdate(ref SystemState state)
        {
            new AnimationActiveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                Animations = SystemAPI.GetSingleton<AnimationDataHolder>().Animations,
            }.ScheduleParallel();
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

                    mesh.Mesh =
                        animationData.BatchMeshId[animation.Frame];

                    if (animation.Frame == 0 && animation.AnimationID.IsUninterruptible())
                    {
                        animation.AnimationID = AnimationSO.AnimationID.None;
                    }
                } 
        }
 
    }
}