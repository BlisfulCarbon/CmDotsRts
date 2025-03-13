using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class MeleeAttackAuth : MonoBehaviour
    {
        public float TimerMax;
        public int DamageAmount;
        public float ColliderSize;
        
        private class Baker : Baker<MeleeAttackAuth>
        {
            public override void Bake(MeleeAttackAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MeleeAttack()
                {
                    TimerMax = auth.TimerMax,
                    TimerState = auth.TimerMax,
                    DamageAmount = auth.DamageAmount,
                    ColliderSize = auth.ColliderSize
                });
            }
        }

    }

    public struct MeleeAttack : IComponentData
    {
        public float TimerState;
        public float TimerMax;
        public int DamageAmount;
        public float ColliderSize;
    }
}