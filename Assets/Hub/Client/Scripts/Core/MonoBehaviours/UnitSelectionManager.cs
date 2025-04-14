using System;
using Hub.Client.Scripts.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Hub.Client.Scripts.MonoBehaviours
{
    public class UnitSelectionManager : MonoBehaviour
    {
        public static UnitSelectionManager Instance { get; private set; }

        public event EventHandler OnSelectionAreaStart;
        public event EventHandler OnSelectionAreaEnd;

        Vector2 _selectionMousePositionDown;

        void Awake() =>
            Instance = this;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _selectionMousePositionDown = Input.mousePosition;
                OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                //Deselect
                EntityQuery entityQuery =
                    new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
                NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<Selected> selecteds = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
                for (int i = 0; i < entities.Length; i++)
                {
                    entityManager.SetComponentEnabled<Selected>(entities[i], false);
                    Selected selected = selecteds[i];
                    selected.onDeselected = true;
                    selecteds[i] = selected;

                    entityManager.SetComponentData(entities[i], selected);
                }

                var selectionAreaRect = GetSelectionAreaRect();
                float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
                float multipleSelectionSizeMinimum = 40f;
                bool isMultipleSelection = selectionAreaSize > multipleSelectionSizeMinimum;

                //Select
                if (isMultipleSelection)
                {
                    entityQuery = new EntityQueryBuilder(Allocator.Temp)
                        .WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);
                    entities = entityQuery.ToEntityArray(Allocator.Temp);
                    NativeArray<LocalTransform> localTransformsArray =
                        entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

                    for (int i = 0; i < localTransformsArray.Length; i++)
                    {
                        LocalTransform unitLocalTransform = localTransformsArray[i];
                        Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                        if (selectionAreaRect.Contains(unitScreenPosition))
                        {
                            entityManager.SetComponentEnabled<Selected>(entities[i], true);
                            Selected selected = entityManager.GetComponentData<Selected>(entities[i]);
                            selected.onSelected = true;
                            entityManager.SetComponentData(entities[i], selected);
                        }
                    }
                }
                else
                {
                    //Single select
                    entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                    PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                    CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                    UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastInput rayCastInput = new RaycastInput()
                    {
                        Start = cameraRay.GetPoint(0f),
                        End = cameraRay.GetPoint(9999f),
                        Filter = new CollisionFilter()
                        {
                            BelongsTo = ~0u,
                            CollidesWith = 1u << GameAssets.UNITS_LAYER | 1u << GameAssets.BUILDINGS_LAYER,
                            GroupIndex = 0,
                        }
                    };

                    if (collisionWorld.CastRay(rayCastInput, out Unity.Physics.RaycastHit rayCastHit))
                        if (entityManager.HasComponent<Unit>(rayCastHit.Entity) &&
                            entityManager.HasComponent<Selected>(rayCastHit.Entity))
                        {
                            entityManager.SetComponentEnabled<Selected>(rayCastHit.Entity, true);
                            Selected selected = entityManager.GetComponentData<Selected>(rayCastHit.Entity);
                            selected.onSelected = true;
                            entityManager.SetComponentData(rayCastHit.Entity, selected);
                        }
                }

                OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetMousePosition();

                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastInput rayCastInput = new RaycastInput()
                {
                    Start = cameraRay.GetPoint(0f),
                    End = cameraRay.GetPoint(9999f),
                    Filter = new CollisionFilter()
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << GameAssets.UNITS_LAYER,
                        GroupIndex = 0,
                    }
                };

                bool isAttackingSingleTarget = false;
                if (collisionWorld.CastRay(rayCastInput, out Unity.Physics.RaycastHit rayCastHit))
                    if (entityManager.HasComponent<Faction>(rayCastHit.Entity))
                    {
                        Faction faction = entityManager.GetComponentData<Faction>(rayCastHit.Entity);

                        if (faction.ID == FactionID.Zombie)
                        {
                            isAttackingSingleTarget = true;
                            
                            EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
                                .WithAll<Selected>()
                                .WithPresent<TargetOverride>()
                                .Build(entityManager);

                            NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
                            NativeArray<TargetOverride> targetOverrides =
                                query.ToComponentDataArray<TargetOverride>(Allocator.Temp);

                            for (int i = 0; i < targetOverrides.Length; i++)
                            {
                                TargetOverride targetOverride = targetOverrides[i];
                                targetOverride.TargetEntity = rayCastHit.Entity;
                                targetOverrides[i] = targetOverride;
                                entityManager.SetComponentEnabled<MoveOverride>(entities[i], true);
                            }

                            query.CopyFromComponentDataArray(targetOverrides);
                        }
                    }

                if (!isAttackingSingleTarget)
                {
                    EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
                        .WithAll<Selected>()
                        .WithPresent<MoveOverride, TargetOverride>()
                        .Build(entityManager);

                    NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
                    NativeArray<MoveOverride> moveOverrides = query.ToComponentDataArray<MoveOverride>(Allocator.Temp);
                    NativeArray<TargetOverride> targetOverrides =
                        query.ToComponentDataArray<TargetOverride>(Allocator.Temp);
                    NativeArray<float3> movePositions = GenerateMovePositionArray(mouseWorldPosition, entities.Length);

                    for (int i = 0; i < moveOverrides.Length; i++)
                    {
                        MoveOverride unitMover = moveOverrides[i];
                        unitMover.TargetPosition = movePositions[i];
                        moveOverrides[i] = unitMover;
                        entityManager.SetComponentEnabled<MoveOverride>(entities[i], true);


                        TargetOverride targetOverride = targetOverrides[i];
                        targetOverride.TargetEntity = Entity.Null;
                        targetOverrides[i] = targetOverride;
                    }

                    query.CopyFromComponentDataArray(moveOverrides);
                    query.CopyFromComponentDataArray(targetOverrides);
                }
            }
        }

        public Rect GetSelectionAreaRect()
        {
            var selectionEndMousePosition = Input.mousePosition;

            Vector2 lowerLeftCorner = new Vector2(
                Mathf.Min(_selectionMousePositionDown.x, selectionEndMousePosition.x),
                Mathf.Min(_selectionMousePositionDown.y, selectionEndMousePosition.y));

            Vector2 upperRightCorner = new Vector2(
                Mathf.Max(_selectionMousePositionDown.x, selectionEndMousePosition.x),
                Mathf.Max(_selectionMousePositionDown.y, selectionEndMousePosition.y));

            return new Rect(
                lowerLeftCorner.x,
                lowerLeftCorner.y,
                upperRightCorner.x - lowerLeftCorner.x,
                upperRightCorner.y - lowerLeftCorner.y);
        }

        private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
        {
            NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);

            if (positionCount == 0)
                return positionArray;

            positionArray[0] = targetPosition;

            if (positionCount == 1)
                return positionArray;

            float ringSize = 2.2f;
            int ring = 0;
            int positionIndex = 1;

            while (positionIndex < positionCount)
            {
                int ringPositionCount = 3 + ring * 2;

                for (int i = 0; i < ringPositionCount; i++)
                {
                    float angle = i * (math.PI2 / ringPositionCount);

                    float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));
                    float3 ringPosition = targetPosition + ringVector;

                    positionArray[positionIndex] = ringPosition;
                    positionIndex++;

                    if (positionIndex >= positionCount)
                        break;
                }

                ring++;
            }

            return positionArray;
        }
    }
}