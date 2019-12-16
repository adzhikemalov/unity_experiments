using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct BoxCollisionComponent:IComponentData
    {
        public float2 Bounds;
    }
}