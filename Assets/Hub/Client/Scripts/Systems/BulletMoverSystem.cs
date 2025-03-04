using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Hub.Client.Scripts.Systems
{
    public partial struct BulletMoverSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((RefRW<LocalTransform> transform,
                         RefRO<Bullet> bullet,
                         RefRO<Target> target,
                         Entity entity)in SystemAPI
                         .Query
                         <RefRW<LocalTransform>,
                             RefRO<Bullet>,
                             RefRO<Target>>().WithEntityAccess())
            {
                LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);

                var distanceBeforeMove = math.distancesq(transform.ValueRO.Position, targetTransform.Position);
                float3 moveDirection = targetTransform.Position - transform.ValueRO.Position;
                moveDirection = math.normalize(moveDirection);

                transform.ValueRW.Rotation = Quaternion.LookRotation(moveDirection);
                transform.ValueRW.Position += moveDirection * bullet.ValueRO.Speed * SystemAPI.Time.DeltaTime;

                var distanceAfterMove = math.distancesq(transform.ValueRO.Position, targetTransform.Position);

                if (distanceBeforeMove < distanceAfterMove) 
                    transform.ValueRW.Position = targetTransform.Position;

                float destroyDistSq = .2f;
                if (distanceBeforeMove < destroyDistSq)
                {
                    RefRW<Health> targetHeath = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);
                    targetHeath.ValueRW.Amount -= bullet.ValueRO.DamageAmount;

                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}