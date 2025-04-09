using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace Hub.Client.Scripts.Animations
{
    [UpdateBefore(typeof(AnimationActiveSystem))]
    public partial struct AnimationChangeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) =>
            state.RequireForUpdate<AnimationDataHolder>();

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new AnimationChangeJob()
            {
                Animations = SystemAPI.GetSingleton<AnimationDataHolder>().Animations,
            }.ScheduleParallel();
        }
    }

    public partial struct AnimationChangeJob : IJobEntity
    {
        public BlobAssetReference<BlobArray<AnimationData>> Animations;

        public void Execute(ref ActiveAnimation animation, ref MaterialMeshInfo mesh)
        {
            if (animation.AnimationID.IsUninterruptible())
                return;

            if (animation.AnimationID != animation.AnimationIDNext)
            {
                animation.Frame = 0;
                animation.FrameTimer = 0f;
                animation.AnimationID = animation.AnimationIDNext;

                ref AnimationData animationData =
                    ref Animations.Value[(int)animation.AnimationID];

                mesh.Mesh = animationData.BatchMeshId[0];
            }
        }
    }
}