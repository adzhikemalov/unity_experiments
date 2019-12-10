using Unity.Entities;

namespace Components
{
    public struct ChargeComponent:IComponentData
    {
        public Entity EntityToCharge;
    }
}