using Hub.Client.Scripts.Animations;
using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts.Units
{
    public class UnitAnimationsAuth : MonoBehaviour
    {
        public AnimationSO.AnimationID Idle;
        public AnimationSO.AnimationID Run;
        public AnimationSO.AnimationID Shoot;
        public AnimationSO.AnimationID Aim;
        public AnimationSO.AnimationID MeleeAttack;

        private class Baker : Baker<UnitAnimationsAuth>
        {
            public override void Bake(UnitAnimationsAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitAnimations()
                {
                    Idle = auth.Idle,
                    Run = auth.Run,
                    Shoot = auth.Shoot,
                    Aim = auth.Aim,
                    MeleeAttack = auth.MeleeAttack
                });
            }
        }
    }

    public struct UnitAnimations : IComponentData
    {
        public AnimationSO.AnimationID Idle;
        public AnimationSO.AnimationID Run;
        public AnimationSO.AnimationID Shoot;
        public AnimationSO.AnimationID Aim;
        public AnimationSO.AnimationID MeleeAttack;
    }
}