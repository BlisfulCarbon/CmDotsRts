using UnityEngine;

namespace Hub.Client.Scripts.Animations
{
    [CreateAssetMenu(fileName = "$Name$AnimationDef", menuName = "Infrastructure/Animations/AnimationDef", order = 1)]
    public class AnimationSO : ScriptableObject
    {
        public enum AnimationID
        {
            None = 0,
            SoldierIdleA = 1,
            SoldierRun = 2,
            ZombieIdle = 3,
            ZombieRun = 4,
        }

        public AnimationID ID;
        public float FrameTimerMax;
        public Mesh[] Meshes;
    }
}