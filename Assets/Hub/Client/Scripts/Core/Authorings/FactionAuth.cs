using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts.Core
{
    public class FactionAuth : MonoBehaviour
    {
        public FactionID ID;
        
        private class FactionAuthBaker : Baker<FactionAuth>
        {
            public override void Bake(FactionAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new Faction()
                {
                   ID = auth.ID, 
                });
            }
        }
    }

    public struct Faction : IComponentData
    {
        public FactionID ID;
    }
}