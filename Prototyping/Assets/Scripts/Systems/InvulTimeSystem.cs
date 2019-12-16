using Components;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public class InvulTimeSystem:ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<InvulComponent>().ForEach((Entity entity, ref InvulComponent invulComponent) =>
                {
                    invulComponent.InvulTime -= Time.deltaTime;
                    if (invulComponent.InvulTime <= 0)
                    {
                        PostUpdateCommands.RemoveComponent(entity, typeof(InvulComponent));
                    }
                });
        }
    }
}