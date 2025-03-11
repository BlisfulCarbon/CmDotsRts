using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class HealthAuth : MonoBehaviour
    {
        public int Amount;
        public int Max;
        private class HealthAuthBaker : Baker<HealthAuth>
        {
            public override void Bake(HealthAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Health()
                {
                   Amount = auth.Amount,
                   Max = auth.Max,
                });
            }
        }
    }

    public struct Health : IComponentData
    {
        public int Amount;
        public int Max;

        public bool OnChange;
    }
}