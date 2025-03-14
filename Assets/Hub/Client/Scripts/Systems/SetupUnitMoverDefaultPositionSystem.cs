using Unity.Entities;
using Unity.Transforms;

namespace Hub.Client.Scripts.Systems
{
    public partial struct SetupUnitMoverDefaultPositionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton
                    <EndSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((
                         RefRO<LocalTransform> transform,
                         RefRW<UnitMover> mover,
                         RefRO<SetupUnitMoverDefaultPosition> defaultPosition,
                         Entity entity)
                     in SystemAPI.Query<
                             RefRO<LocalTransform>,
                             RefRW<UnitMover>,
                             RefRO<SetupUnitMoverDefaultPosition>>()
                         .WithEntityAccess())
            {
                mover.ValueRW.TargetPosition = transform.ValueRO.Position;
                ecb.RemoveComponent<SetupUnitMoverDefaultPosition>(entity);
            }
        }
    }
}