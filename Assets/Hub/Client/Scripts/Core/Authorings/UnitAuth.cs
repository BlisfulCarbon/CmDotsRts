using Hub.Client.Scripts.Core;
using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class UnitAuth : MonoBehaviour
    {
        public class Baker : Baker<UnitAuth>
        {
            public override void Bake(UnitAuth authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Unit());
            }
        }
    }

    public struct Unit : IComponentData
    {
        
    }
}