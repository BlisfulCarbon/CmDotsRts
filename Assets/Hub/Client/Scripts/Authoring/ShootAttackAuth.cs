using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class ShootAttackAuth : MonoBehaviour
    {
        public float AttackDelay;

        private class Baker : Baker<ShootAttackAuth>
        {
            public override void Bake(ShootAttackAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootAttack()
                {
                    TimerMax = auth.AttackDelay,
                    TimerState = auth.AttackDelay,
                });
            }
        }
    }

    public struct ShootAttack : IComponentData
    {
        public float TimerMax;
        public float TimerState;
    }
}