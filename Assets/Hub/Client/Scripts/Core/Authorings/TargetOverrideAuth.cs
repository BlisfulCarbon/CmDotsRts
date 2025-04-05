using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class TargetOverrideAuth : MonoBehaviour
    {
        private class Baker : Baker<TargetOverrideAuth>
        {
            public override void Bake(TargetOverrideAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TargetOverride());
            }
        }
    }

    public struct TargetOverride : IComponentData
    {
        public Entity TargetEntity;
    }
}