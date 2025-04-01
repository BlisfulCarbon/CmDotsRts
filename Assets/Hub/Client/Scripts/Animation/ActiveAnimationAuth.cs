using Hub.Client.Scripts.Animation;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hub.Client.Scripts
{
    public class ActiveAnimationAuth : MonoBehaviour
    {
        public AnimationSO Animation;

        private class Baker : Baker<ActiveAnimationAuth>
        {
            public override void Bake(ActiveAnimationAuth auth)
            {
                EntitiesGraphicsSystem graphics = World.DefaultGameObjectInjectionWorld
                    .GetExistingSystemManaged<EntitiesGraphicsSystem>();

                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new ActiveAnimation()
                {
                    Frame0 = graphics.RegisterMesh(auth.Animation.Meshes[0]),
                    Frame1 = graphics.RegisterMesh(auth.Animation.Meshes[auth.Animation.Meshes.Length / 2]),

                    FrameMax = auth.Animation.Meshes.Length,
                    FrameTimerMax = auth.Animation.FrameTimerMax,
                });
            }
        }
    }

    public struct ActiveAnimation : IComponentData
    {
        public BatchMeshID Frame0;
        public BatchMeshID Frame1;

        public int Frame;
        public int FrameMax;

        public float FrameTimer;
        public float FrameTimerMax;
    }
}