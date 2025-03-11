using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Hub.Client.Scripts.Systems
{
    public partial struct ZombieSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

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