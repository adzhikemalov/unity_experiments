using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct MovingComponent:IComponentData
    {
        public float3 Velocity;
        public float3 PreviousPosition;
    }
}