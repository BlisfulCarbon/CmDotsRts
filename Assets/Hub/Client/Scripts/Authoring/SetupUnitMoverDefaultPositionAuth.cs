using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class SetupUnitMoverDefaultPositionAuth : MonoBehaviour
    {
        private class Baker : Baker<SetupUnitMoverDefaultPositionAuth>
        {
            public override void Bake(SetupUnitMoverDefaultPositionAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SetupUnitMoverDefaultPosition());
            }
        }   
    }

    public struct SetupUnitMoverDefaultPosition : IComponentData
    {
 
        
        
    }
}