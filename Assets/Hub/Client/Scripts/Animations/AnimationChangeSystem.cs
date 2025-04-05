using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace Hub.Client.Scripts.Animations
{
    [UpdateBefore(typeof(AnimationActiveSystem))]
    public partial struct AnimationChangeSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();
            
            foreach ((
                         RefRW<ActiveAnimation> activeAnimation, 
                         RefRW<MaterialMeshInfo> materialMeshInfo) in SystemAPI.Query<
                         RefRW<ActiveAnimation>,
                         RefRW<MaterialMeshInfo>>())
            {
                if (activeAnimation.ValueRO.AnimationID != activeAnimation.ValueRO.AnimationIDNext)
                {
                    activeAnimation.ValueRW.Frame = 0;
                    activeAnimation.ValueRW.FrameTimer = 0f;
                    activeAnimation.ValueRW.AnimationID = activeAnimation.ValueRO.AnimationIDNext;

                    ref AnimationData animationData =
                        ref animationDataHolder.Animations.Value[(int)activeAnimation.ValueRW.AnimationID];

                    materialMeshInfo.ValueRW.MeshID = animationData.BatchMeshId[0];
                }
            }
        }
    }
}