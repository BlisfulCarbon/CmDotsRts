using Hub.Client.Scripts.MonoBehaviours;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Hub.Client.Scripts.Systems
{
    public partial struct ZombieSpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferences>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            PhysicsWorldSingleton physic = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collision = physic.CollisionWorld;
            NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

            foreach ((
                         RefRO<LocalTransform> transform,
                         RefRW<ZombieSpawner> zombieSpawner)
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<ZombieSpawner>>())
            {
                zombieSpawner.ValueRW.TimerState -= SystemAPI.Time.DeltaTime;

                if (zombieSpawner.ValueRO.TimerState > 0)
                    continue;

                zombieSpawner.ValueRW.TimerState = zombieSpawner.ValueRO.TimerMax;

                distanceHits.Clear();
                CollisionFilter collisionFilter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.UNITS_LAYER,
                    GroupIndex = 0,
                };

                int nearbyZombieAmount = 0;
                if (collision.OverlapSphere(
                        transform.ValueRO.Position,
                        zombieSpawner.ValueRO.NearbyZombieDistance,
                        ref distanceHits, collisionFilter))
                {
                    foreach (var hit in distanceHits)
                    {
                        if (!SystemAPI.Exists(hit.Entity))
                            continue;

                        if (SystemAPI.HasComponent<Unit>(hit.Entity) && SystemAPI.HasComponent<Zombie>(hit.Entity))
                            nearbyZombieAmount++;
                    }
                }

                if (nearbyZombieAmount >= zombieSpawner.ValueRO.NearbyZombieSpawnerMax)
                    continue;


                Entity zombieEntity = state.EntityManager.Instantiate(entitiesReferences.ZombiePrefab);
                SystemAPI.SetComponent(zombieEntity, LocalTransform.FromPosition(transform.ValueRO.Position));
                ecb.AddComponent(zombieEntity, new RandomWalking()
                {
                    TargetPosition = transform.ValueRO.Position,
                    OriginPosition = transform.ValueRO.Position,

                    DistanceMax = zombieSpawner.ValueRO.RandomWalkingDistMax,
                    DistanceMin = zombieSpawner.ValueRO.RandomWalkingDistMin,

                    Random = new Unity.Mathematics.Random((uint)zombieEntity.Index),
                });
            }
        }
    }
}