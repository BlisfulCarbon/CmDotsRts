using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class ZombieAuth : MonoBehaviour
    {
        private class Baker : Baker<ZombieAuth>
        {
            public override void Bake(ZombieAuth authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Zombie());
            }
        }
    }

    public struct Zombie : IComponentData
    {
        
    }
}