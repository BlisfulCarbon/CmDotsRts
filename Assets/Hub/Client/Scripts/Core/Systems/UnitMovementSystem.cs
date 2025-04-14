using Hub.Client.Scripts;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct UnitMovementSystem : ISystem
{
    public const float REACHED_TARGET_DISTANCE_SQ = .2f;

    public void OnUpdate(ref SystemState state)
    {
        foreach ((
                     RefRW<LocalTransform> localTransform,
                     RefRO<UnitMover> mover,
                     RefRW<PhysicsVelocity> physics) in
                 SystemAPI.Query<
                     RefRW<LocalTransform>,
                     RefRO<UnitMover>,
                     RefRW<PhysicsVelocity>>())
        {
            UnitMoverJob unitMoverJob = new UnitMoverJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
            };

            unitMoverJob.ScheduleParallel();
        }
    }
}

[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float deltaTime;

    [BurstCompile]
    public void Execute(ref LocalTransform transform, ref UnitMover mover, ref PhysicsVelocity physics)
    {
        float3 moveDirection = mover.TargetPosition - transform.Position;

        if (math.lengthsq(moveDirection) < UnitMovementSystem.REACHED_TARGET_DISTANCE_SQ)
        {
            physics.Linear = float3.zero;
            physics.Linear = float3.zero;
            mover.IsMoving = false;
            return;
        }
        
        mover.IsMoving = true;
        moveDirection = math.normalize(moveDirection);
        transform.Rotation =
            math.slerp(transform.Rotation,
                quaternion.LookRotation(moveDirection, math.up()),
                deltaTime * mover.RotationSpeed);

        physics.Linear = moveDirection * mover.MoveSpeed;
        physics.Angular = float3.zero;
    }
}