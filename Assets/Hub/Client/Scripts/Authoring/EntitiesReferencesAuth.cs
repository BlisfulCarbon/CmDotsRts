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
                AddComponent(entity, new EntitiesReferences()
                {
                    BulletPrefab =  GetEntity(auth.BulletPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }

    public struct EntitiesReferences : IComponentData
    {
        public Entity BulletPrefab;
    }
}