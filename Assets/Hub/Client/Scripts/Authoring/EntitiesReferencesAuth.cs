using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class EntitiesReferencesAuth : MonoBehaviour
    {
        public GameObject BulletPrefab;
        
        
        private class Baker : Baker<EntitiesReferencesAuth>
        {
            public override void Bake(EntitiesReferencesAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntitiesReferences());
            }
        }
    }

    public struct EntitiesReferences : IComponentData
    {
        
    }
}