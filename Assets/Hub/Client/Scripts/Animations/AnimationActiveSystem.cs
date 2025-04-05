using Unity.Entities;
using Unity.Rendering;

namespace Hub.Client.Scripts.Animations
{
    public partial struct AnimationActiveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();

            foreach ((
                         RefRW<ActiveAnimation> activeAnimation,
                         RefRW<MaterialMeshInfo> meshInfo)
                     in SystemAPI.Query<
                         RefRW<ActiveAnimation>,
                         RefRW<MaterialMeshInfo>>())
            {
                ref AnimationData animationData =
                    ref animationDataHolder.Animations.Value[(int)activeAnimation.ValueRO.AnimationID];

                activeAnimation.ValueRW.FrameTimer += SystemAPI.Time.DeltaTime;

                if (activeAnimation.ValueRO.FrameTimer > animationData.FrameTimerMax)
                {
                    activeAnimation.ValueRW.FrameTimer -= animationData.FrameTimerMax;

                    if (animationData.FrameMax == 0)
                        continue;

                    activeAnimation.ValueRW.Frame =
                        (activeAnimation.ValueRO.Frame + 1) % animationData.FrameMax;

                    meshInfo.ValueRW.MeshID =
                        animationData.BatchMeshId[activeAnimation.ValueRO.Frame];
                }
            }
        }
    }
}