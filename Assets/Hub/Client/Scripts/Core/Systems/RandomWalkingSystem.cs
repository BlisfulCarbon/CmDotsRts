using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Hub.Client.Scripts.Systems
{
    [UpdateBefore(typeof(FindTargetSystem))]
    public partial struct RandomWalkingSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                         RefRW<RandomWalking> randomWalking, 
                         RefRW<UnitMover> mover, 
                         RefRO<LocalTransform> transform, RefRO<Target> target) 
                     in SystemAPI.Query<
                         RefRW<RandomWalking>, 
                         RefRW<UnitMover>, 
                         RefRO<LocalTransform>, RefRO<Target>>())
            {
                
                if(target.ValueRO.TargetEntity != Entity.Null)
                    continue;
                
                RandomWalking walkingRO = randomWalking.ValueRO;
                if (math.distancesq(transform.ValueRO.Position, walkingRO.TargetPosition) <
                    UnitMovementSystem.REACHED_TARGET_DISTANCE_SQ)
                {
                    Random random = randomWalking.ValueRO.Random;
                    float3 randomDirection = new float3(random.NextFloat(-1f, +1f), 0, random.NextFloat(-1f, +1f));
                    randomDirection = math.normalize(randomDirection);

                    randomWalking.ValueRW.TargetPosition = walkingRO.TargetPosition + randomDirection *
                        random.NextFloat(walkingRO.DistanceMin, walkingRO.DistanceMax);

                    randomWalking.ValueRW.Random = random;
                }
                else
                {
                    mover.ValueRW.TargetPosition = walkingRO.TargetPosition;
                }
            }
        }
    }
}