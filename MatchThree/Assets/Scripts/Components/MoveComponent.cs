using Unity.Entities;

namespace Components
{
    public struct MoveComponent : IComponentData
    {
        public bool IsMoving;
    }
}