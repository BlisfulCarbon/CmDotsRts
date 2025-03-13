using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Hub.Client.Scripts.Systems
{
    public partial struct MeleeAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorldSingleton physic = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collision = physic.CollisionWorld;
            NativeList<RaycastHit> raycastHits = new NativeList<RaycastHit>(Allocator.Temp);

            foreach ((
                         RefRO<LocalTransform> transform,
                         RefRW<MeleeAttack> attack,
                         RefRO<Target> target,
                         RefRW<UnitMover> mover,
                         Entity entity)
                     in SystemAPI.Query<
                             RefRO<LocalTransform>,
                             RefRW<MeleeAttack>,
                             RefRO<Target>,
                             RefRW<UnitMover>>()
                         .WithEntityAccess())
            {
                if (target.ValueRO.TargetEntity == Entity.Null)
                {
                    if (SystemAPI.HasComponent<RandomWalking>(entity))
                        SystemAPI.SetComponentEnabled<RandomWalking>(entity, true);
                    
                    continue;
                }

                if (SystemAPI.HasComponent<RandomWalking>(entity))
                    SystemAPI.SetComponentEnabled<RandomWalking>(entity, false);

                LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
                float meleeAttackDistanceSq = 2f;

                var isCloseEnoughToAttack = math.distancesq(transform.ValueRO.Position, targetTransform.Position) <
                                            meleeAttackDistanceSq;

                bool isTouchingTarget = false;

                if (!isCloseEnoughToAttack)
                {
                    float3 dirTarget = math.normalize(transform.ValueRO.Position - targetTransform.Position);
                    float distanceExtraToTestRaycast = .4f;
                    RaycastInput raycastInput = new RaycastInput
                    {
                        Start = transform.ValueRO.Position,
                        End = transform.ValueRO.Position +
                              dirTarget * (attack.ValueRO.ColliderSize + distanceExtraToTestRaycast),
                        Filter = CollisionFilter.Default,
                    };

                    raycastHits.Clear();
                    if (collision.CastRay(raycastInput, ref raycastHits))
                        foreach (RaycastHit hit in raycastHits)
                            if (hit.Entity == target.ValueRO.TargetEntity)
                            {
                                isTouchingTarget = true;
                                break;
                            }
                }

                if (!isCloseEnoughToAttack && !isTouchingTarget)
                    mover.ValueRW.TargetPosition = targetTransform.Position;
                else
                {
                    mover.ValueRW.TargetPosition = transform.ValueRO.Position;

                    attack.ValueRW.TimerState -= SystemAPI.Time.DeltaTime;
                    if (attack.ValueRO.TimerState > 0)
                        continue;

                    attack.ValueRW.TimerState = attack.ValueRO.TimerMax;
                    RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);
                    targetHealth.ValueRW.Amount -= attack.ValueRO.DamageAmount;
                    targetHealth.ValueRW.OnChange = true;
                }
            }
        }
    }
}