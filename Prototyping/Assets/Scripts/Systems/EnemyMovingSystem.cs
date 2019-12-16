using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public class EnemyMovingSystem:ComponentSystem
    {
        private EntityQuery _enemyQuery;
        private EntityQuery _playerQuery;
        private float _moveSpeed = 0.03f;
        protected override void OnCreate()
        {
            base.OnCreate();
            _enemyQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyComponent>());
            _playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<EnemyComponent>().WithAll<MovingComponent>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent<MovingComponent>(entity);
            });
            
            Entities.WithAll<EnemyComponent>().WithAll<Translation>().WithNone<ChargeComponent>().ForEach((Entity entity, ref Translation translation) =>
            {
                var minDistance = float.MaxValue;
                var enemyPos = translation.Value;
                var playerPos = enemyPos;
                var chargeEntity = Entity.Null;
                Entities.WithAll<PlayerComponent>().WithAll<Translation>().ForEach((Entity playerEntity, ref Translation playerTranslation) =>
                {
                    var currentDistance = math.distancesq(enemyPos, playerTranslation.Value);
                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        chargeEntity = playerEntity;
                        playerPos = playerTranslation.Value;
                    }
                });

                if (minDistance > 15)
                {
                    PostUpdateCommands.AddComponent(entity, new MovingComponent{Velocity = math.normalize(playerPos-enemyPos)*_moveSpeed});   
                }
            });
        }
    }
}