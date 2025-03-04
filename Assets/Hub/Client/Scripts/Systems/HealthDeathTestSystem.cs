using Unity.Burst;
using Unity.Entities;

namespace Hub.Client.Scripts.Systems
{
    public partial struct HealthDeathTestSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((RefRW<Health> health, Entity entity)
                     in SystemAPI.Query<RefRW<Health>>().WithEntityAccess())
            {
                if (health.ValueRO.Amount <= 0)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}