using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class LoseTargetAuth : MonoBehaviour
    {
        public float Distance;
        private class Baker : Baker<LoseTargetAuth>
        {
            public override void Bake(LoseTargetAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new LoseTarget()
                {
                    Distance = auth.Distance,
                });

            }
        }
    }

    public struct LoseTarget : IComponentData
    {
        public float Distance;
    }
}