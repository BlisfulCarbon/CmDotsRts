using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Hub.Client.Scripts.Systems
{
    [UpdateBefore(typeof(ResetEventsSystems))]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct SelectedVisualSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
            {
                if (selected.ValueRO.onDeselected)
                {
                    RefRW<LocalTransform> vLocalTransform =
                        SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VE);

                    vLocalTransform.ValueRW.Scale = 0f;
                }

                if (selected.ValueRO.onSelected)
                {
                    RefRW<LocalTransform> vLocalTransform =
                        SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VE);

                    vLocalTransform.ValueRW.Scale = selected.ValueRO.ShowScale;
                }
            }
        }
    }
}