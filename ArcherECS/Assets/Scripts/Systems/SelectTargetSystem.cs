using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class SelectTargetSystem:ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<CurrentTargetComponent>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent<CurrentTargetComponent>(entity);
            });
            var nearestEnemy = Entity.Null;
            var nearestEnemyPosition = new float3();
            Entities.WithAll<PlayerComponent>().WithAll<Translation>().ForEach((ref Translation translation) =>
            {
                var minDistance = float.MaxValue;
                var playerPos = translation.Value;
                Entities.WithAll<EnemyComponent>().WithAll<Translation>().ForEach((Entity enemy, ref Translation enemyTranslation) =>
                {
                    var enemyPos = enemyTranslation.Value;
                    var currentDistance = math.distancesq(playerPos, enemyPos);
                    if (currentDistance < minDistance)
                    {
                        nearestEnemy = enemy;
                        minDistance = currentDistance;
                        nearestEnemyPosition = enemyPos;
                    }
                });
                
                if (nearestEnemy != Entity.Null)
                {
                    PostUpdateCommands.AddComponent<CurrentTargetComponent>(nearestEnemy);
                    Debug.DrawLine(playerPos, nearestEnemyPosition);
                }
            });
        }
    }
}