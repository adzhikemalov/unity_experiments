using Components;
using Unity.Entities;
using UnityEngine;

namespace Conversion
{
    public class PlayerConversion : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float Radius;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent(entity, typeof(PlayerComponent));
            dstManager.AddComponentData(entity, new CircleCollisionComponent{Radius = Radius});
            dstManager.AddComponentData(entity, new HealthComponent{HP = 100, MaxHP = 100});
        }
    }
}
