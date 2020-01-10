using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    [Serializable]
    public struct BoardPositionComponent : IComponentData
    {
        public int2 GridPosition;
    }
}
