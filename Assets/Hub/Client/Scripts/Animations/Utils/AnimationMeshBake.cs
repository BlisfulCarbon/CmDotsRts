using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hub.Client.Scripts.Animations.Utils
{
    public class AnimationMeshBake : MonoBehaviour
    {
        [SerializeField] private Animator Animator;
        [SerializeField] private SkinnedMeshRenderer SkinnedMeshRenderer;
        [SerializeField] private int FrameCount;
        [SerializeField] private float TimePerFrame;
        [SerializeField] private string AnimationName = "Base_Idle";

        [SerializeField] List<MeshFilter> AdditionalMeshes;
        int additionalPointer;

        private void Start()
        {
            Animator.Update(0f);

            for (int frame = 0; frame < FrameCount; frame++)
            {
                Mesh mesh = new Mesh();


                Mesh bakedMesh = new Mesh();
                SkinnedMeshRenderer.BakeMesh(bakedMesh);
                
                CombineInstance[] combine = new CombineInstance[1 + AdditionalMeshes.Count];
                
                combine[0].mesh = bakedMesh;
                combine[0].transform = SkinnedMeshRenderer.transform.localToWorldMatrix;
                
                for (var i = 0; i < AdditionalMeshes.Count; i++)
                {
                    additionalPointer = i + 1;
                    combine[additionalPointer].mesh = AdditionalMeshes[i].sharedMesh;
                    combine[additionalPointer].transform = AdditionalMeshes[i].transform.localToWorldMatrix;
                }
                
                mesh.CombineMeshes(combine, true);

                // SkinnedMeshRenderer.BakeMesh(mesh);
                AssetDatabase
                    .CreateAsset(mesh,
                        "Assets/MeshBakeOutput/" +
                        AnimationName +
                        "_" +
                        frame +
                        ".asset");

                
                Debug.Log(Time.deltaTime);
                Animator.Update(TimePerFrame);
            }
        }
    }
}