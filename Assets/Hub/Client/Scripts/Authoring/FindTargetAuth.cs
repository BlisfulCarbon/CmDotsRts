using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class FindTargetAuth : MonoBehaviour
    {
        public float Range;
        public Faction TargetFaction;

        public float UpdateTimer;
        
        private class Baker : Baker<FindTargetAuth>
        {
            public override void Bake(FindTargetAuth auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FindTarget()
                {
                   Range = auth.Range,
                   TargetFaction = auth.TargetFaction,
                   TimerDelay = auth.UpdateTimer,
                   TimerState = auth.UpdateTimer,
                });
            }
        }
    }

    public struct FindTarget : IComponentData
    {
        public float Range;
        public Faction TargetFaction;
        //
        public float TimerDelay;
        public float TimerState;
    }
}