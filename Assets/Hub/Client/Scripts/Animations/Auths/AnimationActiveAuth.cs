using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Hub.Client.Scripts.Animations
{
    public class AnimationActiveAuth : MonoBehaviour
    {
        public AnimationSO.AnimationID NextAnimation;
        
        private class Baker : Baker<AnimationActiveAuth>
        {
            public override void Bake(AnimationActiveAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new ActiveAnimation()
                {
                   AnimationID = auth.NextAnimation,
                });
            }
        }
    }

    public struct ActiveAnimation : IComponentData
    {
        public int Frame;
        public float FrameTimer;

        public AnimationSO.AnimationID AnimationID;
        public AnimationSO.AnimationID AnimationIDNext;
    }
}