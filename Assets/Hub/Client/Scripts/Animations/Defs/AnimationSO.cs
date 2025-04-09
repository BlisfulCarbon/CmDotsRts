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
 
            ScoutIdle,
            ScoutRun,
            ScoutAim,
            ScoutShoot,
        }

        public AnimationID ID;
        public float FrameTimerMax;
        public Mesh[] Meshes;
        
    }
    
    public static class AnimationIDExtensions
    {
        public static bool IsUninterruptible(this AnimationSO.AnimationID source)
        {
            switch (source)
            {
                case AnimationSO.AnimationID.SoldierShoot:
                case AnimationSO.AnimationID.ScoutShoot:
                case AnimationSO.AnimationID.ZombieMeleeAttack:
                    return true;
                
                default: return false;
            }
        }
    }
}