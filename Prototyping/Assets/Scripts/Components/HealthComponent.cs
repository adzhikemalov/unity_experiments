using Unity.Entities;

namespace Components
{
    public struct HealthComponent:IComponentData
    {
        public int HP;
        public int MaxHP;
    }
}