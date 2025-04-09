using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Hub.Client.Scripts.Animations
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateInGroup(typeof(PostBakingSystemGroup))]
    public partial struct AnimationDataHolderBakingSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            AnimationListSO listSo = default;
            foreach (RefRO<AnimationDefsRef> item in SystemAPI.Query<RefRO<AnimationDefsRef>>())
                listSo = item.ValueRO.Animations;

            Dictionary<AnimationSO.AnimationID, int[]> blobAsset = new Dictionary<AnimationSO.AnimationID, int[]>();

            var animationKeys = Enum.GetValues(typeof(AnimationSO.AnimationID));
            foreach (AnimationSO.AnimationID animationID in animationKeys)
            {
                AnimationSO def = listSo.GetAnimations(animationID);
                blobAsset[animationID] = new int[def.Meshes.Length];
            }

            foreach ((
                         RefRO<AnimationDataHolderSubEntity> animation,
                         RefRO<MaterialMeshInfo> mesh)
                     in SystemAPI.Query<
                         RefRO<AnimationDataHolderSubEntity>,
                         RefRO<MaterialMeshInfo>>())
            {
                blobAsset[animation.ValueRO.ID][animation.ValueRO.MeshIndex] = mesh.ValueRO.Mesh;

                // Debug.Log($"{animation.ValueRO.ID}::{animation.ValueRO.MeshIndex} = {mesh.ValueRO.Mesh}");
            }

            foreach (RefRW<AnimationDataHolder> animationDataHolder in SystemAPI.Query<RefRW<AnimationDataHolder>>())
            {
                BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
                ref BlobArray<AnimationData> data = ref blobBuilder.ConstructRoot<BlobArray<AnimationData>>();

                BlobBuilderArray<AnimationData> animationDataBlobBuilderArray =
                    blobBuilder.Allocate<AnimationData>(ref data, animationKeys.Length);

                int index = 0;
                foreach (AnimationSO.AnimationID animationID in animationKeys)
                {
                    AnimationSO def = listSo.GetAnimations(animationID);

                    BlobBuilderArray<int> blobBuilderArray =
                        blobBuilder.Allocate<int>(ref
                            animationDataBlobBuilderArray[index].BatchMeshId, def.Meshes.Length);
                    //
                    animationDataBlobBuilderArray[index].FrameTimerMax = def.FrameTimerMax;
                    animationDataBlobBuilderArray[index].FrameMax = def.Meshes.Length;

                    //
                    for (int i = 0; i < def.Meshes.Length; i++)
                    {
                        blobBuilderArray[i] = blobAsset[animationID][i];
                    }

                    index++;
                }

                animationDataHolder.ValueRW.Animations =
                    blobBuilder.CreateBlobAssetReference<BlobArray<AnimationData>>(Allocator.Persistent);

                blobBuilder.Dispose();
            }
        }
    }
}