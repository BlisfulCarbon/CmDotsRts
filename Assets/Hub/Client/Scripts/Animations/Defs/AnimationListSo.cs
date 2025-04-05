using System.Collections.Generic;
using UnityEngine;

namespace Hub.Client.Scripts.Animations
{
    [CreateAssetMenu(fileName = "$Name$AnimationListDef", menuName = "Infrastructure/Animations/AnimationListDef",
        order = 1)]
    public class AnimationListSo : ScriptableObject
    {
        public List<AnimationSO> Animations;

        public AnimationSO GetAnimations(AnimationSO.AnimationID id)
        {
            foreach (var def in Animations)
                if (def.ID == id)
                    return def;
            
            Debug.LogError($"{nameof(AnimationSO)}::GetAnimations Can not find animation {id}");
            return default;
        }
    }
}