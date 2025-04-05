using Unity.Burst;
using Unity.Entities;

namespace Hub.Client.Scripts.Systems
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

            /*
            foreach (RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
            {
                selected.ValueRW.onSelected = false;
                selected.ValueRW.onDeselected = false;
            }

            foreach (RefRW<Health> health in SystemAPI.Query<RefRW<Health>>())
            {
                health.ValueRW.OnChange = false;
            }

            foreach (RefRW<ShootAttack> shootAttack in SystemAPI.Query<RefRW<ShootAttack>>())
            {
                shootAttack.ValueRW.OnShoot.IsTriggered = false;
            }
            */
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
}