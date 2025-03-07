using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Hub.Client.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct ResetTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
            {
                Entity entity = target.ValueRO.TargetEntity;
                
                if(entity == Entity.Null)
                    continue;
                
                if (!SystemAPI.Exists(entity) || !SystemAPI.HasComponent<LocalTransform>(entity)) 
                    target.ValueRW.TargetEntity = Entity.Null;
            }
        }
    }
}