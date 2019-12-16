using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class KeyboardInputSystem:ComponentSystem
    {
        protected override void OnCreate()
        {
            RequireForUpdate(EntityManager.UniversalQuery);
            base.OnCreate();
        }
        
        private float _moveSpeed = 0.02f;
        protected override void OnUpdate()
        {
            Entities.WithAll<PlayerComponent>().WithAll<MovingComponent>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent<MovingComponent>(entity);
            });
            
            var move = new float3();
            if (Input.GetKey(KeyCode.W))
            {
                move += new float3(0, 1, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                move += new float3(0, -1, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                move += new float3(-1, 0,0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                move += new float3(1, 0,0);
            }

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