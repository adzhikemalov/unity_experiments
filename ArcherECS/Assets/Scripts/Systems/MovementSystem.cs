using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class MovementSystem:ComponentSystem
    {
        protected override void OnCreate()
        {
            RequireForUpdate(EntityManager.UniversalQuery);
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<MovingComponent>().WithAll<Translation>().ForEach((ref Translation translation, ref MovingComponent movingComponent) =>
            {
                movingComponent.PreviousPosition = translation.Value;
                translation.Value += movingComponent.Velocity;
            });
        }
    }
}