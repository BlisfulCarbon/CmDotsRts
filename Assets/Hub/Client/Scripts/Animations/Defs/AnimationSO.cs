using UnityEngine;

namespace Hub.Client.Scripts.Animations
{
    [CreateAssetMenu(fileName = "$Name$AnimationDef", menuName = "Infrastructure/Animations/AnimationDef", order = 1)]
    public class AnimationSO : ScriptableObject
    {
        public enum AnimationID
        {
            None,
            SoldierIdleA,
            SoldierRun,
            SoldierShoot,
            ZombieIdle,
            ZombieRun,
            SoldierAim,
            ZombieMeleeAttack,
        }

        public AnimationID ID;
        public float FrameTimerMax;
        public Mesh[] Meshes;
    }
}