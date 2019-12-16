using Components;
using DefaultNamespace;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class PlayerShootingSystem:ComponentSystem
    {
        private EntityManager _manager;
        private float _lastShootTime = 0f;
        private float _shootRecoil = 0.5f;
        private EntityQuery _query;
        protected override void OnCreate()
        {
            _manager = World.Active.EntityManager;
            _query = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            _lastShootTime += Time.deltaTime;
            if (_lastShootTime >= _shootRecoil)
            {
                GameSettings.ShootingReadyInstance.gameObject.SetActive(true);
                Entities.WithAll<PlayerComponent>().WithNone<MovingComponent>().ForEach((ref Translation translation) =>
                {
                    var playerPosition = translation.Value;
                    Entities.WithAll<CurrentTargetComponent>().ForEach((ref Translation targetTranslation) =>
                    {
                        var entity = _manager.CreateEntity();
                        _manager.AddComponentData(entity, new MovingComponent{Velocity = math.normalize(targetTranslation.Value-playerPosition)*0.05f});
                        _manager.AddComponentData(entity, new Translation{Value = playerPosition});
                        _manager.AddComponentData(entity, new BulletComponent());
                        _manager.AddComponentData(entity, new LifeTimeComponent{Time = 5f});
                        _manager.AddComponentData(entity, new CircleCollisionComponent{Radius = 0.05f});
                        GameSettings.ShootingReadyInstance.gameObject.SetActive(false);
                    });
                    _lastShootTime = 0;
                });
            }
        }
    }
}