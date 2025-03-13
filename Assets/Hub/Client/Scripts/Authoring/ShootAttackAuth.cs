using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class ShootAttackAuth : MonoBehaviour
    {
        public int DamageAmount;
        public float AttackDelay;
        public float AttackDistance;
        public Transform BulletSpawnPosition;
        
        private class Baker : Baker<ShootAttackAuth>
        {
            public override void Bake(ShootAttackAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootAttack()
                {
                    AttackDistance = auth.AttackDistance,
                    TimerMax = auth.AttackDelay,
                    TimerState = auth.AttackDelay,
                    DamageAmount = auth.DamageAmount,
                    BulletSpawnLocalPosition = auth.BulletSpawnPosition.localPosition,
                });
            }
        }
    }

    public struct ShootAttack : IComponentData
    {
        public int DamageAmount;
        public float AttackDistance;
        public float3 BulletSpawnLocalPosition;

        public OnShootEvent OnShoot;
        
        //Timer
        public float TimerMax;
        public float TimerState;
        
        public struct OnShootEvent
        {
            public bool IsTriggered;
            public float3 ShootFromPosition;
        }
    }
}