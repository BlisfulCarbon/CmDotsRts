using Unity.Burst;
using Unity.Entities;

namespace Hub.Client.Scripts.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
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
        }
    }
}