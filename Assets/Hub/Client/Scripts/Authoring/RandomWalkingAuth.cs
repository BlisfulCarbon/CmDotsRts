using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Hub.Client.Scripts
{
    public class RandomWalkingAuth : MonoBehaviour
    {
        public float DistanceMin;
        public float DistanceMax;

        public float3 TargetPosition;
        public float3 OriginPosition;

        public uint RandomSeed;

        private class Baker : Baker<RandomWalkingAuth>
        {
            public override void Bake(RandomWalkingAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RandomWalking()
                {
                    DistanceMin = auth.DistanceMin,
                    DistanceMax = auth.DistanceMax,

                    TargetPosition = auth.TargetPosition,
                    OriginPosition = auth.OriginPosition,
                    Random = new Random(auth.RandomSeed),
                });
            }
        }
    }

    public struct RandomWalking : IComponentData, IEnableableComponent
    {
        public float3 TargetPosition;
        public float3 OriginPosition;

        public float DistanceMin;
        public float DistanceMax;

        public Unity.Mathematics.Random Random;
    }
}