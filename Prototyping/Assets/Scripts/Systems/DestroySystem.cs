using Components;
using Unity.Entities;

namespace Systems
{
    public class DestroySystem:ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<DestroyComponent>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.DestroyEntity(entity);
            });
        }
    }
}