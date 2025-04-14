using Hub.Client.Scripts.Core;
using Hub.Client.Scripts.MonoBehaviours;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Hub.Client.Scripts.Systems
{
    public partial struct FindTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

            foreach ((
                         RefRO<LocalTransform> transform,
                         RefRW<FindTarget> findTarget,
                         RefRW<Target> target,
                         RefRO<TargetOverride> targetOverride)
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<FindTarget>,
                         RefRW<Target>,
                         RefRO<TargetOverride>>())
            {
                findTarget.ValueRW.TimerState -= SystemAPI.Time.DeltaTime;
                if (findTarget.ValueRO.TimerState > 0)
                    continue;
                findTarget.ValueRW.TimerState = findTarget.ValueRO.TimerDelay;

                if (targetOverride.ValueRO.TargetEntity != Entity.Null)
                {
                    target.ValueRW.TargetEntity = targetOverride.ValueRO.TargetEntity;
                    continue;
                }

                distanceHits.Clear();
                CollisionFilter collisionFilter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.UNITS_LAYER | 1u << GameAssets.BUILDINGS_LAYER,
                    GroupIndex = 0,
                };

                if (collisionWorld.OverlapSphere(transform.ValueRO.Position, findTarget.ValueRO.Range, ref distanceHits,
                        collisionFilter))
                {
                    Entity closetTargetEntity = Entity.Null;
                    float closetTargetDistance = float.MaxValue;
                    float closetTargetDistanceOffset = 0f;

                    if (target.ValueRO.TargetEntity != Entity.Null)
                    {
                        closetTargetEntity = target.ValueRO.TargetEntity;
                        LocalTransform targetTransform =
                            SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
                        closetTargetDistance = math.distance(transform.ValueRO.Position, targetTransform.Position);
                        closetTargetDistanceOffset = 2f;
                    }

                    foreach (var hit in distanceHits)
                    {
                        if (!SystemAPI.Exists(hit.Entity) ||
                            !SystemAPI.HasComponent<Faction>(hit.Entity))
                            continue;

                        Faction faction = SystemAPI.GetComponent<Faction>(hit.Entity);
                        if (faction.ID == findTarget.ValueRO.TargetFactionID)
                        {
                            if (closetTargetEntity == Entity.Null)
                            {
                                closetTargetEntity = hit.Entity;
                                closetTargetDistance = hit.Distance;
                            }
                            else
                            {
                                if (closetTargetDistance + closetTargetDistanceOffset > hit.Distance)
                                {
                                    closetTargetEntity = hit.Entity;
                                    closetTargetDistance = hit.Distance;
                                }
                            }
                        }
                    }

                    
                    if (closetTargetEntity != Entity.Null)
                        target.ValueRW.TargetEntity = closetTargetEntity;
                }
            }
        }
    }
}