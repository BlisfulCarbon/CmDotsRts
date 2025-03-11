using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Hub.Client.Scripts.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct HealthBarSystem : ISystem
    {
        // [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Vector3 cameraForward = Vector3.zero;
            if (Camera.main != null)
                cameraForward = Camera.main.transform.forward;


            foreach ((
                         RefRW<LocalTransform> transform,
                         RefRO<HealthBar> healthBar) in
                     SystemAPI.Query
                     <RefRW<LocalTransform>,
                         RefRO<HealthBar>>())
            {
                LocalTransform parentHealthBarTransform =
                    SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.Health);

                if (transform.ValueRO.Scale == 1f)
                {
                    transform.ValueRW.Rotation =
                        parentHealthBarTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward,
                            math.up()));
                }

                Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.Health);

                if (!health.OnChange)
                {
                    continue;
                }
                
                float healthNormalized = (float)health.Amount / health.Max;

                if (healthNormalized == 1f)
                {
                    transform.ValueRW.Scale = 0f;
                }
                else
                    transform.ValueRW.Scale = 1f;

                RefRW<PostTransformMatrix> barVisualPostTransformMatrix = SystemAPI
                    .GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.BarVisual);

                barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1f, 1f);
            }
        }
    }
}