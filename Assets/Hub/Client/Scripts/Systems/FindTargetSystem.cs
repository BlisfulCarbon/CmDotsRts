using Hub.Client.Scripts.MonoBehaviours;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

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

            foreach ((RefRO<LocalTransform> transform, RefRW<FindTarget> findTarget, RefRW<Target> target)
                     in SystemAPI.Query<RefRO<LocalTransform>, RefRW<FindTarget>, RefRW<Target>>())
            {
                #region Timer

                findTarget.ValueRW.TimerState -= SystemAPI.Time.DeltaTime;
                if (findTarget.ValueRO.TimerState > 0)
                    continue;
                findTarget.ValueRW.TimerState = findTarget.ValueRO.TimerDelay;

                #endregion Timer

                distanceHits.Clear();
                CollisionFilter collisionFilter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.UNITS_LAYER,
                    GroupIndex = 0,
                };

                if (collisionWorld.OverlapSphere(transform.ValueRO.Position, findTarget.ValueRO.Range, ref distanceHits,
                        collisionFilter))
                {
                    foreach (var distanceHit in distanceHits)
                    {
                        if(!SystemAPI.Exists(distanceHit.Entity) || !SystemAPI.HasComponent<Unit>(distanceHit.Entity)) 
                            continue;
                        
                        Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                        if (targetUnit.Faction == findTarget.ValueRO.TargetFaction)
                        {
                            target.ValueRW.TargetEntity = distanceHit.Entity;
                            break;
                        }
                    }
                }
            }
        }
    }
}