using Components;
using DefaultNamespace;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(KeyboardInputSystem))]
    public class JoystickInputSystem:ComponentSystem
    {
        private float _moveSpeed = 0.1f;
        private EntityQuery _playerQuery;

        protected override void OnCreate()
        {
            base.OnCreate();            
            _playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<PlayerComponent>().WithAll<MovingComponent>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent<MovingComponent>(entity);
            });
 
            var move = new float3(0,1,0) * GameSettings.JoystickInstance.Vertical + new float3(1,0,0) * GameSettings.JoystickInstance.Horizontal;
            move = move * _moveSpeed;
            if (!move.Equals(float3.zero))
            {
                Entities.WithAll<PlayerComponent>().ForEach((Entity entity) =>
                {
                    PostUpdateCommands.AddComponent(entity, new MovingComponent{Velocity = move});
                });
            }
        }
    }
}