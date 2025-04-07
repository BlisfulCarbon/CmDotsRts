using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Hub.Client.Scripts.Core.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct ResetTargetSystem : ISystem
    {
        ComponentLookup<LocalTransform> _transform;
        EntityStorageInfoLookup _eStorage;

        public void OnCreate(ref SystemState state)
        {
            _transform = state.GetComponentLookup<LocalTransform>(true);
            _eStorage = state.GetEntityStorageInfoLookup();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _transform.Update(ref state);
            _eStorage.Update(ref state);

            new ResetTargetJob
            {
                Transform = _transform,
                EStorage = _eStorage,
            }.ScheduleParallel();

            new ResetTargetOverride
            {
                Transform = _transform,
                EStorage = _eStorage,
            }.ScheduleParallel();
        }
    }

    public partial struct ResetTargetOverride : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> Transform;
        [ReadOnly] public EntityStorageInfoLookup EStorage;

        public void Execute(ref TargetOverride targetOverride)
        {
            Entity entity = targetOverride.TargetEntity;

            if (entity == Entity.Null)
                return;

            if (!EStorage.Exists(entity) || !Transform.HasComponent(entity))
                targetOverride.TargetEntity = Entity.Null;
        }
    }

    public partial struct ResetTargetJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> Transform;
        [ReadOnly] public EntityStorageInfoLookup EStorage;

        public void Execute(ref Target target)
        {
            Entity entity = target.TargetEntity;

            if (entity == Entity.Null)
                return;

            if (!EStorage.Exists(entity) || !Transform.HasComponent(entity))
                target.TargetEntity = Entity.Null;
        }
    }
}