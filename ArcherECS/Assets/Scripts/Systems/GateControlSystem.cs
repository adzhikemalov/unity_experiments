using Components;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public class GateControlSystem:ComponentSystem
    {
        private EntityQuery _enemyQuery;
        private EntityQuery _gateQuery;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            _enemyQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyComponent>());
            _gateQuery = GetEntityQuery(ComponentType.ReadOnly<GateComponent>());
        }

        protected override void OnUpdate()
        {
            if (_enemyQuery.CalculateEntityCount() > 0)
                return;
            
            Entities.WithAll<GateComponent>().ForEach(entity =>
            {
                PostUpdateCommands.DestroyEntity(entity);    
            });
        }
    }
}