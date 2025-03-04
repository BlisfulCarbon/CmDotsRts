using Unity.Entities;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class UnitAuth : MonoBehaviour
    {
        public Faction Faction;
        
        public class Baker : Baker<UnitAuth>
        {
            public override void Bake(UnitAuth authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Unit()
                {
                    Faction = authoring.Faction,
                });
            }
        }
        
    }

    public struct Unit : IComponentData
    {
        public Faction Faction;
    }
}