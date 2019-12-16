using Components;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public class LifeTimeSystem:ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<LifeTimeComponent>().ForEach((Entity entity, ref LifeTimeComponent lifeTimeComponent) =>
                {
                    lifeTimeComponent.Time -= Time.deltaTime;
                    if (lifeTimeComponent.Time <= 0)
                    {
                        PostUpdateCommands.DestroyEntity(entity);
                    }
                });
        }
    }
}