using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class ZombieSpawnerAuth : MonoBehaviour
    {
        public float TimerMax;
        public float RandomWalkingDistMin;
        public float RandomWalkingDistMax;
        
        private class Baker : Baker<ZombieSpawnerAuth>
        {
            public override void Bake(ZombieSpawnerAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ZombieSpawner()
                {
                    TimerMax = auth.TimerMax,
                    TimerState = auth.TimerMax,
                    
                    RandomWalkingDistMin = auth.RandomWalkingDistMin,
                    RandomWalkingDistMax = auth.RandomWalkingDistMax,
                });
            }
        }
    }

    public struct ZombieSpawner : IComponentData
    {
        public float RandomWalkingDistMin;
        public float RandomWalkingDistMax;
        // Timer
        public float TimerState;
        public float TimerMax;
    }
}