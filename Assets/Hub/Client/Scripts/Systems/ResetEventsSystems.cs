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
        }
    }
}