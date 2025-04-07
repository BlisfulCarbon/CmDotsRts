using Hub.Client.Scripts.Animations;
using Hub.Client.Scripts.Core.Systems;
using Hub.Client.Scripts.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Hub.Client.Scripts.Units
{
    [UpdateAfter(typeof(MeleeAttackSystem))]
    [UpdateAfter(typeof(ShootAttackSystem))]
    public partial struct UnitAnimationStateSystem : ISystem
    {
        ComponentLookup<ActiveAnimation> _activeAnimation;

        public void OnCreate(ref SystemState state)
        {
            _activeAnimation = state.GetComponentLookup<ActiveAnimation>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _activeAnimation.Update(ref state);

            new IdleRunAnimationStateJob()
            {
                Animation = _activeAnimation,
            }.ScheduleParallel();

            new ShootAnimationStateJob()
            {
                Animation = _activeAnimation,
            }.ScheduleParallel();

            new MeleeAttackAnimationStateJob()
            {
                Animation = _activeAnimation,
            }.ScheduleParallel();
        }
    }


    [BurstCompile]
    public partial struct IdleRunAnimationStateJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> Animation;

        public void Execute(in AnimatedMesh mesh, in UnitMover mover, in UnitAnimations animations)
        {
            RefRW<ActiveAnimation> active = Animation.GetRefRW(mesh.MeshRef);

            if (mover.IsMoving)
                active.ValueRW.AnimationIDNext = animations.Run;
            else
                active.ValueRW.AnimationIDNext = animations.Idle;
        }
    }

    [BurstCompile]
    public partial struct ShootAnimationStateJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> Animation;

        public void Execute(
            in AnimatedMesh mesh,
            in ShootAttack attack,
            in UnitMover mover,
            in Target target,
            in UnitAnimations animations)
        {
            if (!mover.IsMoving && target.TargetEntity != Entity.Null)
            {
                RefRW<ActiveAnimation> activeAnimation = Animation.GetRefRW(mesh.MeshRef);
                activeAnimation.ValueRW.AnimationIDNext = animations.Aim;
            }

            if (attack.OnShoot.IsTriggered)
            {
                RefRW<ActiveAnimation> activeAnimation =
                    Animation.GetRefRW(mesh.MeshRef);

                activeAnimation.ValueRW.AnimationIDNext = animations.Shoot;
            }
        }
    }

    [BurstCompile]
    public partial struct MeleeAttackAnimationStateJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> Animation;

        public void Execute(in AnimatedMesh mesh, in MeleeAttack attack, in UnitAnimations animations)
        {
            RefRW<ActiveAnimation> active = Animation.GetRefRW(mesh.MeshRef);

            if (attack.OnAttack)
                active.ValueRW.AnimationIDNext = animations.MeleeAttack;
        }
    }
}