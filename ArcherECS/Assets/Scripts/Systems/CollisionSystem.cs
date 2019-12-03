using System;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(MovementSystem))]
    public class CollisionSystem:JobComponentSystem
    {
        struct DynamicCollisionJob:IJobChunk
        {
            [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
            [ReadOnly] public ArchetypeChunkComponentType<CircleCollisionComponent> CollisionType;

            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> TranslationsToTest;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<CircleCollisionComponent> CollisionsToTest;
            
            
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var translationChunks = chunk.GetNativeArray(TranslationType);
                var collisionChunks = chunk.GetNativeArray(CollisionType);
                
                for (int i = 0; i < chunk.Count; i++)
                {
                    Translation pos = translationChunks[i];
                    CircleCollisionComponent colA = collisionChunks[i];
                    for (int j = 0; j < TranslationsToTest.Length; j++)
                    {
                        Translation pos2 = TranslationsToTest[j];
                        CircleCollisionComponent colB = CollisionsToTest[j];
                        if (CheckCircleCollision(pos.Value, pos2.Value, colA.Radius, colB.Radius))
                        {
                            
                        }
                    }
                }
            }
            
            private bool CheckCircleCollision(float3 posA, float3 posB, float radiusA, float radiusB)
            {
                float3 delta = posA - posB;
                float distanceSquare = delta.x * delta.x + delta.y * delta.y;
                return math.sqrt(distanceSquare) <= radiusA + radiusB;
            }
        }

        struct StaticCollisionJob:IJobChunk
        {
            [ReadOnly] public ArchetypeChunkComponentType<CircleCollisionComponent> CollisionType;
            [ReadOnly] public ArchetypeChunkComponentType<MovingComponent> MovingType;
            public ArchetypeChunkComponentType<Translation> TranslationType;

            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> TranslationsToTest;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<BoxCollisionComponent> CollisionsToTest;
            
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var translationChunks = chunk.GetNativeArray(TranslationType);
                var collisionChunks = chunk.GetNativeArray(CollisionType);
                var movingChunks = chunk.GetNativeArray(MovingType);
                
                for (int i = 0; i < chunk.Count; i++)
                {
                    Translation boxPos = translationChunks[i];
                    CircleCollisionComponent circleCollision = collisionChunks[i];
                    
                    for (int j = 0; j < TranslationsToTest.Length; j++)
                    {
                        Translation circlePos = TranslationsToTest[j];
                        BoxCollisionComponent boxCollision = CollisionsToTest[j];
                        if (CheckBoxCircleCollision(circlePos.Value, circleCollision.Radius, boxPos.Value, boxCollision.Bounds.x, boxCollision.Bounds.y))
                        {
                            circlePos.Value = movingChunks[i].PreviousPosition;
                            translationChunks[i] = circlePos;
                        }
                    }
                }
            }
            
            
            private bool CheckBoxCircleCollision(float3 circlePosition, float radius, float3 boxPosition, float boxWidth, float boxHeight)
            {
                var x = math.max(boxPosition.x-boxWidth/2, math.min(circlePosition.x, boxPosition.x+boxWidth/2));
                var y = math.max(boxPosition.y - boxHeight / 2,math.min(circlePosition.y, boxPosition.y + boxHeight / 2));
                
                var distance = (x - circlePosition.x) * (x - circlePosition.x) + (y - circlePosition.y) * (y - circlePosition.y);
                
                return distance < radius * radius;
            }
        }
        
        

        private EntityQuery _playerGroup;
        private EntityQuery _bulletGroup;
        private EntityQuery _enemyGroup;
        private EntityQuery _boundsGroup;
        private EntityQuery _movingPlayerGroup;
        protected override void OnCreate()
        {
            _playerGroup = GetEntityQuery( ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<PlayerComponent>(), ComponentType.ReadOnly<CircleCollisionComponent>());
            _movingPlayerGroup = GetEntityQuery( ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadOnly<PlayerComponent>(), ComponentType.ReadOnly<CircleCollisionComponent>(), ComponentType.ReadOnly<MovingComponent>());
            _bulletGroup = GetEntityQuery(ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<BulletComponent>(), ComponentType.ReadOnly<CircleCollisionComponent>());
            _enemyGroup = GetEntityQuery(ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<EnemyComponent>(), ComponentType.ReadOnly<CircleCollisionComponent>());
            _boundsGroup = GetEntityQuery(ComponentType.ReadOnly<BoxCollisionComponent>(), ComponentType.ReadOnly<Translation>());
        }
        
         

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var translationType = GetArchetypeChunkComponentType<Translation>(false);
            var collisionType = GetArchetypeChunkComponentType<CircleCollisionComponent>(true);
            var movingType = GetArchetypeChunkComponentType<MovingComponent>(true);
            
            var jobEnemyVsPlayer = new DynamicCollisionJob()
            {
                CollisionType = collisionType,
                TranslationType = translationType,
                TranslationsToTest = _playerGroup.ToComponentDataArray<Translation>(Allocator.TempJob),
                CollisionsToTest = _playerGroup.ToComponentDataArray<CircleCollisionComponent>(Allocator.TempJob)
            };

            var colToTest = _bulletGroup.ToComponentDataArray<CircleCollisionComponent>(Allocator.TempJob);
            Debug.Log("ENEMIES COUNT "+ _bulletGroup.CalculateEntityCount());
            
            var jobBulletVsEnemy = new DynamicCollisionJob()
            {
                CollisionType = collisionType,
                TranslationType = translationType,
                TranslationsToTest = _bulletGroup.ToComponentDataArray<Translation>(Allocator.TempJob),
                CollisionsToTest = colToTest
            };
            var jobPlayerVsBounds = new StaticCollisionJob()
            {
                CollisionType = collisionType,
                TranslationType = translationType,
                MovingType = movingType,
                TranslationsToTest = _boundsGroup.ToComponentDataArray<Translation>(Allocator.TempJob),
                CollisionsToTest = _boundsGroup.ToComponentDataArray<BoxCollisionComponent>(Allocator.TempJob),
            };
            var jobHandle = jobEnemyVsPlayer.Schedule(_enemyGroup, inputDeps);
            var secondJobHandle = jobBulletVsEnemy.Schedule(_enemyGroup, jobHandle);
            return jobPlayerVsBounds.Schedule(_movingPlayerGroup, secondJobHandle);
        }
    }
}