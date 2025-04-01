using Unity.Entities;
using Unity.Rendering;

namespace Hub.Client.Scripts.Systems
{
    public partial struct ActiveAnimationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                         RefRW<ActiveAnimation> activeAnimation,
                         RefRW<MaterialMeshInfo> meshInfo)
                     in SystemAPI.Query<
                         RefRW<ActiveAnimation>,
                         RefRW<MaterialMeshInfo>>())
            {
                activeAnimation.ValueRW.FrameTimer += SystemAPI.Time.DeltaTime;

                if (activeAnimation.ValueRO.FrameTimer > activeAnimation.ValueRO.FrameMax)
                {
                    activeAnimation.ValueRW.FrameTimer -= activeAnimation.ValueRW.FrameTimerMax;
                    activeAnimation.ValueRW.Frame =
                        (activeAnimation.ValueRO.Frame + 1) % activeAnimation.ValueRO.FrameMax;

                    switch (activeAnimation.ValueRO.Frame)
                    {
                        default:
                        case 0:
                            meshInfo.ValueRW.MeshID = activeAnimation.ValueRO.Frame0;
                            break;
                        case 1:
                            meshInfo.ValueRW.MeshID = activeAnimation.ValueRO.Frame1;
                            break;
                        
                    }
                }
            }
        }
    }
}