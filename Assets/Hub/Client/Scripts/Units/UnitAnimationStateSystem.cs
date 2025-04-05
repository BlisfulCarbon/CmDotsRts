using Hub.Client.Scripts.Animations;
using Unity.Burst;
using Unity.Entities;

namespace Hub.Client.Scripts.Units
{
    public partial struct UnitAnimationStateSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                         RefRW<AnimatedMesh> mesh, 
                         RefRO<UnitMover> mover, 
                         RefRO<UnitAnimations> animations)
                     in SystemAPI.Query<
                         RefRW<AnimatedMesh>, 
                         RefRO<UnitMover>, 
                         RefRO<UnitAnimations>>())
            {
                RefRW<ActiveAnimation> activeAnimation =
                    SystemAPI.GetComponentRW<ActiveAnimation>(mesh.ValueRO.MeshRef);

                if (mover.ValueRO.IsMoving)
                    activeAnimation.ValueRW.AnimationIDNext = animations.ValueRO.Run;
                else
                    activeAnimation.ValueRW.AnimationIDNext = animations.ValueRO.Idle;
            }
        }
    }
}