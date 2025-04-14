using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Hub.Client.Scripts.Animations
{
    public class AnimationHolderAuth : MonoBehaviour
    {
        public AnimationListSO Refs;
        public Material MockMaterial;

        private class Baker : Baker<AnimationHolderAuth>
        {
            public override void Bake(AnimationHolderAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AnimationDataHolder animationDataHolder = new AnimationDataHolder();

                foreach (AnimationSO animation in auth.Refs.Animations)
                {
                    for (int i = 0; i < animation.Meshes.Length; i++)
                    {
                        Mesh mesh = animation.Meshes[i];

                        Entity additionalEntity = CreateAdditionalEntity(TransformUsageFlags.None, true);
                        AddComponent(additionalEntity, new MaterialMeshInfo());
                        AddComponent(additionalEntity, new RenderMeshUnmanaged()
                        {
                            materialForSubMesh = auth.MockMaterial,
                            mesh = mesh,
                        });
                        AddComponent(additionalEntity, new AnimationDataHolderSubEntity()
                        {
                            ID = animation.ID,
                            MeshIndex = i,
                        });
                    }
                }

                AddComponent(entity, new AnimationDefsRef()
                {
                    Animations = auth.Refs,
                });
                AddComponent(entity, animationDataHolder);
            }
        }
    }

    public struct AnimationDefsRef : IComponentData
    {
        public UnityObjectRef<AnimationListSO> Animations;
    }
    
    public struct AnimationDataHolderSubEntity : IComponentData
    {
        public AnimationSO.AnimationID ID;
        public int MeshIndex;
    }
    
    public struct AnimationDataHolder : IComponentData
    {
        public BlobAssetReference<BlobArray<AnimationData>> Animations;
    }

    public struct AnimationData
    {
        public float FrameTimerMax;
        public int FrameMax;
        public BlobArray<int> BatchMeshId;
    }
}