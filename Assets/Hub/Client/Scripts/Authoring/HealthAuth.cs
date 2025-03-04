using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class HealthAuth : MonoBehaviour
    {
        public int Amount;
        private class HealthAuthBaker : Baker<HealthAuth>
        {
            public override void Bake(HealthAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Health()
                {
                   Amount = auth.Amount,
                });
            }
        }
    }

    public struct Health : IComponentData
    {
        public int Amount;
    }
}