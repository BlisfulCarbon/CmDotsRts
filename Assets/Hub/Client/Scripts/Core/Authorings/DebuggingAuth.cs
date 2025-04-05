using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class DebuggingAuth : MonoBehaviour
    {

        public class Baker : Baker<DebuggingAuth>
        {
            public override void Bake(DebuggingAuth authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Debugging>(entity, new Debugging());
                SetComponentEnabled<Debugging>(entity, false);
            }
        }
    }

    public struct Debugging : IComponentData, IEnableableComponent
    {
        
    }
}