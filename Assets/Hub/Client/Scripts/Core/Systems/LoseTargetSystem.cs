using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Hub.Client.Scripts.Systems
{
    public partial struct LoseTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                         RefRO<LocalTransform> transform, 
                         RefRO<LoseTarget> loseTarget, 
                         RefRW<Target> target, 
                         RefRO<TargetOverride> targetOverride) 
                     in SystemAPI.Query<
                         RefRO<LocalTransform>, 
                         RefRO<LoseTarget>, 
                         RefRW<Target>, 
                         RefRO<TargetOverride>>())
            {
                if(target.ValueRO.TargetEntity == Entity.Null)
                    continue;

                if (targetOverride.ValueRO.TargetEntity != Entity.Null)
                    continue;
                
                LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
                
                if(math.distance(transform.ValueRO.Position, targetTransform.Position) < loseTarget.ValueRO.Distance)
                    continue;

                target.ValueRW.TargetEntity = Entity.Null;
            }
        }

    }
}