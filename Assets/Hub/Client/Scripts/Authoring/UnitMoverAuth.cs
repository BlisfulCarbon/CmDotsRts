using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class UnitMoverAuth : MonoBehaviour
    {
        public float MoveSpeed;
        public float RotationSpeed;

        public class Baker : Baker<UnitMoverAuth>
        {
            public override void Bake(UnitMoverAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitMover
                {
                    MoveSpeed = auth.MoveSpeed,
                    RotationSpeed = auth.RotationSpeed
                });
            }
        }
    }

    public struct UnitMover : IComponentData
    {
        public float MoveSpeed;
        public float RotationSpeed;
        public float3 TargetPosition;
    }
}