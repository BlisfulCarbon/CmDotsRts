using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class MoveOverrideAuth : MonoBehaviour
    {
        private class Baker : Baker<MoveOverrideAuth>
        {
            public override void Bake(MoveOverrideAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MoveOverride());
                SetComponentEnabled<MoveOverride>(entity, false);

            }
        }
    }

    public struct MoveOverride : IComponentData, IEnableableComponent
    {
        public float3 TargetPosition;
    }
}