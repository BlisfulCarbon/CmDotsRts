using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class SelectedAuth : MonoBehaviour
    {
        public GameObject VGO;
        public float ShowScale;
        
        public class Baker : Baker<SelectedAuth>
        {
            public override void Bake(SelectedAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Selected()
                {
                    VE = GetEntity(auth.VGO, TransformUsageFlags.Dynamic),
                    ShowScale = auth.ShowScale,
                });
                SetComponentEnabled<Selected>(entity, false);
            }
        }
    }

    public struct Selected : IComponentData, IEnableableComponent
    {
        public Entity VE;
        public float ShowScale;

        public bool onSelected;
        public bool onDeselected;
    }
}