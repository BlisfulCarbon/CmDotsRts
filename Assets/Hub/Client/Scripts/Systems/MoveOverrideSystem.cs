using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Hub.Client.Scripts.Systems
{
    public partial struct MoveOverrideSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                         RefRO<LocalTransform> transform, 
                         RefRO<MoveOverride> moveOverride, 
                         EnabledRefRW<MoveOverride> moveOverrideEnable,
                         RefRW<UnitMover> mover)
                     in SystemAPI.Query<
                         RefRO<LocalTransform>, 
                         RefRO<MoveOverride>, 
                         EnabledRefRW<MoveOverride>,
                         RefRW<UnitMover>>())
            {
                if (math.distancesq(transform.ValueRO.Position, moveOverride.ValueRO.TargetPosition) >
                    UnitMovementSystem.REACHED_TARGET_DISTANCE_SQ)
                {
                    mover.ValueRW.TargetPosition = moveOverride.ValueRO.TargetPosition;
                }
                else
                {
                    moveOverrideEnable.ValueRW = false;
                }
            }
        }
    }
}