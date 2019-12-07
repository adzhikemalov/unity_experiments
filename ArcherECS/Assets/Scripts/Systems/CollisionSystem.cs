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
    public class CollisionSystem : JobComponentSystem
    {
        private struct DynamicCollisionJob : IJobChunk
        {
            [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
            [ReadOnly] public ArchetypeChunkComponentType<CircleCollisionComponent> CollisionType;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;

            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> TranslationsToTest;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<CircleCollisionComponent> CollisionsToTest;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> DamageEntities;
            public EntityCommandBuffer.Concurrent Buffer;
            public bool DamageToPlayer;
            public bool IsBullet;
            public int JobIndex;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var translationChunks = chunk.GetNativeArray(TranslationType);
                var collisionChunks = chunk.GetNativeArray(CollisionType);
                var entities = chunk.GetNativeArray(EntityType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    var pos = translationChunks[i];
                    var colA = collisionChunks[i];
                    for (var j = 0; j < TranslationsToTest.Length; j++)
                    {
                        var pos2 = TranslationsToTest[j];
                        var colB = CollisionsToTest[j];
                        if (CheckCircleCollision(pos.Value, pos2.Value, colA.Radius, colB.Radius))
                        {
                            Buffer.AddComponent(JobIndex, DamageEntities[j], new ApplyDamageComponent{Damage = 10, MakeInvulnerable = DamageToPlayer});
                            if (IsBullet)
                            {
                                Buffer.DestroyEntity(JobIndex, entities[i]);
                            }
                        }
                    }
                }
            }

            private static bool CheckCircleCollision(float3 posA, float3 posB, float radiusA, float radiusB)
            {
                var delta = posA - posB;
                var distanceSquare = delta.x * delta.x + delta.y * delta.y;
                return math.sqrt(distanceSquare) <= radiusA + radiusB;
            }
        }

        private struct StaticCollisionJob : IJobChunk
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

                for (var i = 0; i < chunk.Count; i++)
                {
                    var boxPos = translationChunks[i];
                    var circleCollision = collisionChunks[i];

                    for (var j = 0; j < TranslationsToTest.Length; j++)
                    {
                        var circlePos = TranslationsToTest[j];
                        var boxCollision = CollisionsToTest[j];
                        if (CheckBoxCircleCollision(circlePos.Value, circleCollision.Radius, boxPos.Value,
                            boxCollision.Bounds.x, boxCollision.Bounds.y))
                        {
                            circlePos.Value = movingChunks[i].PreviousPosition;
                            translationChunks[i] = circlePos;
                        }
                    }
                }
            }


            private static bool CheckBoxCircleCollision(float3 circlePosition, float radius, float3 boxPosition,
                float boxWidth, float boxHeight)
            {
                var x = math.max(boxPosition.x - boxWidth / 2,
                    math.min(circlePosition.x, boxPosition.x + boxWidth / 2));
                var y = math.max(boxPosition.y - boxHeight / 2,
                    math.min(circlePosition.y, boxPosition.y + boxHeight / 2));

                var distance = (x - circlePosition.x) * (x - circlePosition.x) +
                               (y - circlePosition.y) * (y - circlePosition.y);

                return distance < radius * radius;
            }
        }



        private EntityQuery _playerGroup;
        private EntityQuery _bulletGroup;
        private EntityQuery _enemyGroup;
        private EntityQuery _boundsGroup;
        private EntityQuery _movingPlayerGroup;
        private EntityCommandBufferSystem _bufferSystem;
        protected override void OnCreate()
        {
            var playerDes = new EntityQueryDesc
            {
                None = new ComponentType[]{typeof(InvulComponent)},
                All = new ComponentType[]{typeof(PlayerComponent), typeof(Translation), typeof(CircleCollisionComponent)}
            };
            _playerGroup = GetEntityQuery(playerDes);
            _movingPlayerGroup = GetEntityQuery(ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadOnly<PlayerComponent>(), ComponentType.ReadOnly<CircleCollisionComponent>(),
                ComponentType.ReadOnly<MovingComponent>());
            _bulletGroup = GetEntityQuery(ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<BulletComponent>(), ComponentType.ReadOnly<CircleCollisionComponent>());
            _enemyGroup = GetEntityQuery(ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<EnemyComponent>(), ComponentType.ReadOnly<CircleCollisionComponent>());
            _boundsGroup = GetEntityQuery(ComponentType.ReadOnly<BoxCollisionComponent>(),
                ComponentType.ReadOnly<Translation>());
            _bufferSystem = World.Active.GetExistingSystem<EntityCommandBufferSystem>();
        }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var translationType = GetArchetypeChunkComponentType<Translation>(false);
            var collisionType = GetArchetypeChunkComponentType<CircleCollisionComponent>(true);
            var movingType = GetArchetypeChunkComponentType<MovingComponent>(true);
            var entityChunkType = this.GetArchetypeChunkEntityType();
            var jobId = 0;
            var jobEnemyVsPlayer = new DynamicCollisionJob()
            {
                CollisionType = collisionType,
                TranslationType = translationType,
                TranslationsToTest = _playerGroup.ToComponentDataArray<Translation>(Allocator.TempJob),
                CollisionsToTest = _playerGroup.ToComponentDataArray<CircleCollisionComponent>(Allocator.TempJob),
                DamageEntities = _playerGroup.ToEntityArray(Allocator.TempJob),
                JobIndex = jobId++,
                Buffer = _bufferSystem.CreateCommandBuffer().ToConcurrent(),
                DamageToPlayer = true,
                EntityType = entityChunkType
            };
            var jobBulletVsEnemy = new DynamicCollisionJob()
            {
                CollisionType = collisionType,
                TranslationType = translationType,
                TranslationsToTest = _enemyGroup.ToComponentDataArray<Translation>(Allocator.TempJob),
                CollisionsToTest =  _enemyGroup.ToComponentDataArray<CircleCollisionComponent>(Allocator.TempJob),
                DamageEntities = _enemyGroup.ToEntityArray(Allocator.TempJob),
                JobIndex = jobId,
                Buffer = _bufferSystem.CreateCommandBuffer().ToConcurrent(),
                IsBullet = true,
                EntityType = entityChunkType
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
            var secondJobHandle = jobBulletVsEnemy.Schedule(_bulletGroup, jobHandle);
            return jobPlayerVsBounds.Schedule(_movingPlayerGroup, secondJobHandle);
        }
    }
}