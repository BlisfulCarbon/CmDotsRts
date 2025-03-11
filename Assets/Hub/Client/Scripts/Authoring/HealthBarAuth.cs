using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class HealthBarAuth : MonoBehaviour
    {
        public GameObject BarVisual;
        public GameObject Health;
        
        private class Baker : Baker<HealthBarAuth>
        {
            public override void Bake(HealthBarAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new HealthBar()
                {
                    BarVisual = GetEntity(auth.BarVisual, TransformUsageFlags.NonUniformScale),
                    Health = GetEntity(auth.Health, TransformUsageFlags.Dynamic),
                });

            }
        }
    }

    internal struct HealthBar : IComponentData
    {
        public Entity BarVisual;
        public Entity Health;
    }
}