using Unity.Burst;
using Unity.Entities;

namespace Hub.Client.Scripts.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct ResetTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
            {
                if (!SystemAPI.Exists(target.ValueRO.TargetEntity))
                {
                    target.ValueRW.TargetEntity = Entity.Null;
                }
            }
        }
    }
}