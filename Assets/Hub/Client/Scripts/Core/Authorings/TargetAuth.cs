using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class TargetAuth : MonoBehaviour
    {
        public GameObject Target;
        
        private class TargetAuthBaker : Baker<TargetAuth>
        {
            public override void Bake(TargetAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Target()
                {
                    TargetEntity = GetEntity(auth.Target, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct Target : IComponentData
    {
        public Entity TargetEntity;
    }
}