using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Hub.Client.Scripts.Systems
{
    public partial struct ShootAttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferences>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            foreach (
                (
                    RefRW<LocalTransform> transform,
                    RefRW<ShootAttack> attack,
                    RefRO<Target> target,
                    RefRW<UnitMover> mover, 
                    Entity entity)
                in SystemAPI.Query<
                        RefRW<LocalTransform>,
                        RefRW<ShootAttack>,
                        RefRO<Target>,
                        RefRW<UnitMover>>()
                    .WithDisabled<MoveOverride>()
                    .WithEntityAccess())
            {
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;

                if (attack.ValueRO.TimerState > 0)
                {
                    attack.ValueRW.TimerState -= SystemAPI.Time.DeltaTime;
                    continue;
                }

                LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);

                if (math.distance(transform.ValueRO.Position, targetTransform.Position) > attack.ValueRO.AttackDistance)
                {
                    mover.ValueRW.TargetPosition = targetTransform.Position;
                    continue;
                }
                else
                {
                    mover.ValueRW.TargetPosition = transform.ValueRO.Position;
                }

                RefRW<TargetOverride> enemyTargetOverride =
                    SystemAPI.GetComponentRW<TargetOverride>(target.ValueRO.TargetEntity);
                if (enemyTargetOverride.ValueRO.TargetEntity == Entity.Null)
                    enemyTargetOverride.ValueRW.TargetEntity = entity;
                
                float3 aimDirection = targetTransform.Position - transform.ValueRO.Position;
                aimDirection = math.normalize(aimDirection);

                quaternion targetRotation = quaternion.LookRotation(aimDirection, math.up());
                transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, targetRotation,
                    SystemAPI.Time.DeltaTime * 100);

                attack.ValueRW.TimerState = attack.ValueRO.TimerMax;

                float3 bulletSpawnWorldPosition =
                    transform.ValueRO.TransformPoint(attack.ValueRO.BulletSpawnLocalPosition);
                Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.BulletPrefab);
                SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));

                RefRW<Bullet> bullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
                bullet.ValueRW.DamageAmount = attack.ValueRO.DamageAmount;

                RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
                bulletTarget.ValueRW.TargetEntity = target.ValueRO.TargetEntity;

                attack.ValueRW.OnShoot.IsTriggered = true;
                attack.ValueRW.OnShoot.ShootFromPosition = bulletSpawnWorldPosition;

                Entity shootLightEntity = state.EntityManager.Instantiate(entitiesReferences.ShootLightPrefab);
                SystemAPI.SetComponent(shootLightEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));
            }
        }
    }
}