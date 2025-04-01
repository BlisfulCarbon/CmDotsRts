using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Hub.Client.Scripts.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct HealthBarSystem : ISystem
    {
        ComponentLookup<LocalTransform> _parentHealthBar;
        ComponentLookup<Health> _health;
        ComponentLookup<PostTransformMatrix> _postTransformMatrix;

        public void OnCreate(ref SystemState state)
        {
            _parentHealthBar = state.GetComponentLookup<LocalTransform>();
            _health = state.GetComponentLookup<Health>(true);
            _postTransformMatrix = state.GetComponentLookup<PostTransformMatrix>();
        }

        // [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Vector3 cameraForward = Vector3.zero;

            if (Camera.main != null)
                cameraForward = Camera.main.transform.forward;

            _parentHealthBar.Update(ref state);
            _health.Update(ref state);
            _postTransformMatrix.Update(ref state);


            new HealthBarJob()
            {
                CameraForward = cameraForward,
                Transform = _parentHealthBar,
                Health = _health,
                BarVisualPostTransformMatrix = _postTransformMatrix,
            }.ScheduleParallel();
            /*
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
                    transform.ValueRW.Scale = 0f;
                else
                    transform.ValueRW.Scale = 1f;

                RefRW<PostTransformMatrix> barVisualPostTransformMatrix = SystemAPI
                    .GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.BarVisual);

                barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1f, 1f);
                */
        }
    }

    public partial struct HealthBarJob : IJobEntity
    {
        public float3 CameraForward;

        [ReadOnly] public ComponentLookup<Health> Health;

        [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> Transform;
        [NativeDisableParallelForRestriction] public ComponentLookup<PostTransformMatrix> BarVisualPostTransformMatrix;

        public void Execute(in HealthBar healthBar, Entity entity)
        {
            RefRW<LocalTransform> transform = Transform.GetRefRW(entity);

            LocalTransform parentHealthBarTransform =
                Transform[healthBar.Health];

            if (transform.ValueRO.Scale == 1f)
            {
                transform.ValueRW.Rotation =
                    parentHealthBarTransform
                        .InverseTransformRotation(quaternion.LookRotation(CameraForward,
                            math.up()));
            }

            Health health = Health[healthBar.Health];

            if (!health.OnChange)
                return;

            float healthNormalized = (float)health.Amount / health.Max;

            if (healthNormalized == 1f)
                transform.ValueRW.Scale = 0f;
            else
                transform.ValueRW.Scale = 1f;

            BarVisualPostTransformMatrix
                .GetRefRW(healthBar.BarVisual)
                .ValueRW.Value = float4x4.Scale(healthNormalized, 1f, 1f);
        }
    }
}