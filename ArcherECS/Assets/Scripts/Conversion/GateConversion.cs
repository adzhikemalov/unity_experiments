using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Conversion
{
    public class GateConversion : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var localScale = transform.localScale;
            dstManager.AddComponentData(entity, new BoxCollisionComponent{Bounds = new float2(localScale.x, localScale.y)});
            dstManager.AddComponent(entity, typeof(GateComponent));
        }
    }
}
