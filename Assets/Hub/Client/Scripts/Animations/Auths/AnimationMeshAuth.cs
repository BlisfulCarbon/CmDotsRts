using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts.Animations
{
    public class AnimationMeshAuth : MonoBehaviour
    {
        public GameObject Mesh;
        
        private class Baker : Baker<AnimationMeshAuth>
        {
            public override void Bake(AnimationMeshAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new AnimatedMesh()
                {
                    MeshRef = GetEntity(auth.Mesh, TransformUsageFlags.Dynamic),
                });

            }
        }
    }

    public struct AnimatedMesh : IComponentData
    {
        public Entity MeshRef;
    }
}