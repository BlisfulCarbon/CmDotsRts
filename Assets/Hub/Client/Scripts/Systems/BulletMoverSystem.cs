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
                if (target.ValueRO.TargetEntity == Entity.Null 
                    || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.TargetEntity))
                {
                    ecb.DestroyEntity(entity);
                    continue;
                }
                
                LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
                ShootVictim shootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.TargetEntity);
                // float3 targetPosition = targetTransform.Position;// targetTransform.TransformPoint(shootVictim.HitLocalPosition);
                float3 targetPosition = targetTransform.TransformPoint(shootVictim.HitLocalPosition);
                
                var distanceBeforeMove = math.distancesq(transform.ValueRO.Position, targetPosition);
                // var distanceBeforeMove = math.distancesq(transform.ValueRO.Position, targetTransform.Position);
                float3 moveDirection = targetPosition - transform.ValueRO.Position;
                moveDirection = math.normalize(moveDirection);

                transform.ValueRW.Rotation = Quaternion.LookRotation(moveDirection);
                transform.ValueRW.Position += moveDirection * bullet.ValueRO.Speed * SystemAPI.Time.DeltaTime;

                var distanceAfterMove = math.distancesq(transform.ValueRO.Position, targetPosition);

                if (distanceBeforeMove < distanceAfterMove) 
                    transform.ValueRW.Position = targetPosition;

                float destroyDistSq = .2f;
                if (distanceBeforeMove < destroyDistSq)
                {
                    RefRW<Health> health = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);
                    health.ValueRW.Amount -= bullet.ValueRO.DamageAmount;
                    health.ValueRW.OnChange = true;
                    
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}