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
            AnimationListSO listSo = GetAnimations(ref state);

            Dictionary<AnimationSO.AnimationID, int[]> blobAsset = new Dictionary<AnimationSO.AnimationID, int[]>();

            foreach (AnimationSO animation in listSo.Animations) 
                blobAsset[animation.ID] = new int[animation.Meshes.Length];

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
                    blobBuilder.Allocate<AnimationData>(ref data, listSo.Animations.Count);

                int index = 0;
                foreach (AnimationSO animation in listSo.Animations)
                {
                    BlobBuilderArray<int> blobBuilderArray =
                        blobBuilder.Allocate<int>(ref
                            animationDataBlobBuilderArray[index].BatchMeshId, animation.Meshes.Length);
                    //
                    animationDataBlobBuilderArray[index].FrameTimerMax = animation.FrameTimerMax;
                    animationDataBlobBuilderArray[index].FrameMax = animation.Meshes.Length;

                    //
                    for (int i = 0; i < animation.Meshes.Length; i++)
                    {
                        blobBuilderArray[i] = blobAsset[animation.ID][i];
                    }

                    index++;
                }

                animationDataHolder.ValueRW.Animations =
                    blobBuilder.CreateBlobAssetReference<BlobArray<AnimationData>>(Allocator.Persistent);

                blobBuilder.Dispose();
            }
        }

        AnimationListSO GetAnimations(ref SystemState state)
        {
            foreach (RefRO<AnimationDefsRef> item in SystemAPI.Query<RefRO<AnimationDefsRef>>())
                return item.ValueRO.Animations;
            
            return default;
        }
    }

    public abstract class Parent
    {
        public abstract void SomeSome();
    }

    public abstract class Child : Parent
    {
        public override void SomeSome()
        {
            
        }
    }

    public class GrandChild : Child
    {
        public new void SomeSome()
        {
            
        }
    }
}