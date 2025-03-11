using Hub.Client.Scripts;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

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

public partial struct UnitMoverJob : IJobEntity
{
    public float deltaTime;

    [BurstCompile]
    public void Execute(ref LocalTransform localTransform, in UnitMover mover, ref PhysicsVelocity physics)
    {
        float3 moveDirection = mover.TargetPosition - localTransform.Position;

        if (math.lengthsq(moveDirection) < UnitMovementSystem.REACHED_TARGET_DISTANCE_SQ)
        {
            physics.Linear = float3.zero;
            physics.Linear = float3.zero;
            return;
        }

        moveDirection = math.normalize(moveDirection);

        float rotationSpeed = mover.RotationSpeed;
        localTransform.Rotation =
            math.slerp(localTransform.Rotation,
                quaternion.LookRotation(moveDirection, math.up()),
                deltaTime * rotationSpeed);

        physics.Linear = moveDirection * mover.MoveSpeed;
        physics.Angular = float3.zero;
    }
}