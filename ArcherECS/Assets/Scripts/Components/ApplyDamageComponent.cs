using Unity.Entities;

namespace Components
{
    public struct ApplyDamageComponent:IComponentData
    {
        public int Damage;
        public bool MakeInvulnerable;
    }
}