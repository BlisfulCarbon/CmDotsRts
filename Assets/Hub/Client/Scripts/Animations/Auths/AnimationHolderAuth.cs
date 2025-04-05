using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hub.Client.Scripts.Animations
{
    public class AnimationHolderAuth : MonoBehaviour
    {
        public AnimationListSo Refs;

        private class Baker : Baker<AnimationHolderAuth>
        {
            public override void Bake(AnimationHolderAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                EntitiesGraphicsSystem graphics = World
                    .DefaultGameObjectInjectionWorld
                    .GetExistingSystemManaged<EntitiesGraphicsSystem>();

                AnimationDataHolder animationDataHolder = new AnimationDataHolder();
                BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
                ref BlobArray<AnimationData> data = ref blobBuilder.ConstructRoot<BlobArray<AnimationData>>();

                var animationKeys = Enum.GetValues(typeof(AnimationSO.AnimationID));

                BlobBuilderArray<AnimationData> animationDataBlobBuilderArray =
                    blobBuilder.Allocate<AnimationData>(ref data, animationKeys.Length);

                int index = 0;

                foreach (AnimationSO.AnimationID animationID in animationKeys)
                {
                    AnimationSO def = auth.Refs.GetAnimations(animationID);

                    BlobBuilderArray<BatchMeshID> blobBuilderArray =
                        blobBuilder.Allocate<BatchMeshID>(ref
                            animationDataBlobBuilderArray[index].BatchMeshId, def.Meshes.Length);
                    //
                    animationDataBlobBuilderArray[index].FrameTimerMax = def.FrameTimerMax;
                    animationDataBlobBuilderArray[index].FrameMax = def.Meshes.Length;
                    //
                    for (int i = 0; i < def.Meshes.Length; i++)
                    {
                        Mesh mesh = def.Meshes[i];
                        blobBuilderArray[i] = graphics.RegisterMesh(mesh);
                    }

                    index++;
                }

                animationDataHolder.Animations =
                    blobBuilder.CreateBlobAssetReference<BlobArray<AnimationData>>(Allocator.Persistent);

                blobBuilder.Dispose();

                AddBlobAsset(ref animationDataHolder.Animations, out Unity.Entities.Hash128 objectHash);

                AddComponent(entity, animationDataHolder);
            }
        }
    }

    public struct AnimationDataHolder : IComponentData
    {
        public BlobAssetReference<BlobArray<AnimationData>> Animations;
    }

    public struct AnimationData
    {
        public float FrameTimerMax;
        public int FrameMax;
        public BlobArray<BatchMeshID> BatchMeshId;
    }
}