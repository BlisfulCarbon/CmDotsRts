using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Hub.Client.Scripts.Systems
{
    public partial struct ShootAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
            
            foreach (
                (
                    RefRO<LocalTransform> transform,
                    RefRW<ShootAttack> attack,
                    RefRO<Target> target)
                in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<ShootAttack>,
                    RefRO<Target>>())
            {
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;

                if (attack.ValueRO.TimerState > 0)
                {
                    attack.ValueRW.TimerState -= SystemAPI.Time.DeltaTime;
                    continue;
                }

                attack.ValueRW.TimerState = attack.ValueRO.TimerMax;
            }
        }
    }
}