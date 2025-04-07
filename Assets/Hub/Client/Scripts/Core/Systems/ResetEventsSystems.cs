using Unity.Burst;
using Unity.Entities;

namespace Hub.Client.Scripts.Core.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    public partial struct ResetEventsSystems : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new ResetSelectedEventsJob().ScheduleParallel();
            new ResetHealthEventJob().ScheduleParallel();
            new ResetShootAttackEventJob().ScheduleParallel();
            new ResetMeleeAttackEventJob().ScheduleParallel();
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