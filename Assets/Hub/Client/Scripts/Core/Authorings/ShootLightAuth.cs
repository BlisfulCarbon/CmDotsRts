using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class ShootLightAuth : MonoBehaviour
    {
        public float Timer;
        
        private class ShootLightAuthBaker : Baker<ShootLightAuth>
        {
            public override void Bake(ShootLightAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootLight()
                {
                    Timer = auth.Timer,
                });
            }
        }
    }

    public struct ShootLight : IComponentData
    {
        public float Timer;
    }
}