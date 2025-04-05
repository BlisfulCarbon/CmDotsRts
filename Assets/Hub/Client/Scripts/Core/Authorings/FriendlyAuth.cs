using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class FriendlyAuth : MonoBehaviour
    {
        private class FriendlyAuthBaker : Baker<FriendlyAuth>
        {
            public override void Bake(FriendlyAuth authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Friendly());
            }
        }
    }

    public struct Friendly : IComponentData
    {
    }
}