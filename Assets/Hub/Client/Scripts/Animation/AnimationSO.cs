using UnityEngine;

namespace Hub.Client.Scripts.Animation
{
    [CreateAssetMenu(fileName = "$Name$AnimationDef", menuName = "Infrastructure/AnimationDef", order = 1)]
    public class AnimationSO : ScriptableObject
    {
        public float FrameTimerMax;
        public Mesh[] Meshes;
    }
}