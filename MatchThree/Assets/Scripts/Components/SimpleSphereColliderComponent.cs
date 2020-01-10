using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct SimpleSphereColliderComponent : IComponentData
    {
        public float radius;
        public float3 centerOffset;
    }
}