using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public class PathNodeData:IComponentData
    {
        public int2 PathId;
        public float2 Position;
    }
}