using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class ShootVictimAuth : MonoBehaviour
    {
        public Transform HitPosition;
        
        private class Baker : Baker<ShootVictimAuth>
        {
            public override void Bake(ShootVictimAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootVictim()
                {
                   HitLocalPosition = auth.HitPosition.localPosition,
                });
            }
        }
    }

    public struct ShootVictim : IComponentData
    {
        public float3 HitLocalPosition;
    }
}