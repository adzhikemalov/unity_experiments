using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public class PathNodeConnectionData:IComponentData
    {
        public int2 StartNodeId;
        public int2 EndNodeId;
    }
}