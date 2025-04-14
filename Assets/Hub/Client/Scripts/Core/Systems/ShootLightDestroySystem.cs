using Unity.Burst;
using Unity.Entities;

namespace Hub.Client.Scripts.Systems
{
    public partial struct ShootLightDestroySystem : ISystem
    {
        public void OnCreate(ref SystemState state) =>
            state.RequireForUpdate<
                EndSimulationEntityCommandBufferSystem.Singleton>();

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((RefRW<ShootLight> shootLight, Entity entity) in SystemAPI.Query<RefRW<ShootLight>>()
                         .WithEntityAccess())
            {
                shootLight.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                if (shootLight.ValueRO.Timer < 0f)
                    ecb.DestroyEntity(entity);
            }
        }
    }
}