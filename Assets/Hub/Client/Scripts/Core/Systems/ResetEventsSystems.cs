using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Hub.Client.Scripts.Core.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    public partial struct ResetEventsSystems : ISystem
    {
        NativeArray<JobHandle> states;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            states = new NativeArray<JobHandle>(4, Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            states[0] = new ResetSelectedEventsJob().ScheduleParallel(state.Dependency);
            states[1] = new ResetHealthEventJob().ScheduleParallel(state.Dependency);
            states[2] = new ResetShootAttackEventJob().ScheduleParallel(state.Dependency);
            states[3] = new ResetMeleeAttackEventJob().ScheduleParallel(state.Dependency);

            state.Dependency = JobHandle.CombineDependencies(states);
        }
    }

    [BurstCompile]
    public partial struct ResetShootAttackEventJob : IJobEntity
    {
        public void Execute(ref ShootAttack shootAttack)
        {
            shootAttack.OnShoot.IsTriggered = false;
        }
    }

    [BurstCompile]
    public partial struct ResetHealthEventJob : IJobEntity
    {
        public void Execute(ref Health health)
        {
            health.OnChange = false;
        }
    }

    [BurstCompile]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    public partial struct ResetSelectedEventsJob : IJobEntity
    {
        public void Execute(ref Selected selected)
        {
            selected.onSelected = false;
            selected.onDeselected = false;
        }
    }

    [BurstCompile]
    public partial struct ResetMeleeAttackEventJob : IJobEntity
    {
        public void Execute(ref MeleeAttack shootAttack)
        {
            shootAttack.OnAttack = false;
        }
    }
}