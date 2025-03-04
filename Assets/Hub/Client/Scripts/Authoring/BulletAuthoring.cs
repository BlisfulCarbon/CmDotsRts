using System;
using Unity.Entities;
using UnityEngine;

namespace Hub.Client.Scripts
{
    public class BulletAuthoring : MonoBehaviour
    {
        public float Speed;
        public int DamageAmount;

        public GameObject TrailPrefab;

        void Awake()
        {
            SpawnTrail();
        }

        void SpawnTrail()
        {
            var trail = Instantiate(TrailPrefab);
            Destroy(trail, 0.5f); 
        }

        private class Baker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring auth)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Bullet()
                {
                    Speed = auth.Speed,
                    DamageAmount = auth.DamageAmount,
                });
            }
        }
    }

    public struct Bullet : IComponentData
    {
        public float Speed;
        public int DamageAmount;
    }
}