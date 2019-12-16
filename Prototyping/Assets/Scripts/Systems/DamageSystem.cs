using Components;
using Unity.Entities;
using Unity.Jobs;

namespace Systems
{
    
    public class DamageSystem:ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ApplyDamageComponent>().WithAll<HealthComponent>().ForEach(
                (Entity entity, ref HealthComponent hp, ref ApplyDamageComponent damage) =>
                {
                    hp.HP -= damage.Damage;
                    if (hp.HP <= 0)
                    {
                        EntityManager.AddComponent<DestroyComponent>(entity);
                    }

                    if (damage.MakeInvulnerable)
                    {
                        EntityManager.AddComponentData(entity, new InvulComponent{InvulTime = 1f});
                    }
                    PostUpdateCommands.RemoveComponent<ApplyDamageComponent>(entity);
                });
        }
    }
}