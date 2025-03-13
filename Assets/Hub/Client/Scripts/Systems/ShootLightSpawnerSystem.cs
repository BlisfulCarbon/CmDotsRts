using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Hub.Client.Scripts.Systems
{
    public partial struct ShootLightSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReferences refs = SystemAPI.GetSingleton<EntitiesReferences>();

            foreach (RefRO<ShootAttack> shootAttack in SystemAPI.Query<RefRO<ShootAttack>>())
            {
                if (shootAttack.ValueRO.OnShoot.IsTriggered)
                {
                    Entity entity = state.EntityManager.Instantiate(refs.ShootLightPrefab);
                    SystemAPI.SetComponent(entity, LocalTransform.FromPosition(shootAttack.ValueRO.OnShoot.ShootFromPosition));
                }
            }
        }
    }
}