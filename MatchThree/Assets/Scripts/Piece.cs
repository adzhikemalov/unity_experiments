using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequiresEntityConversion]
public class Piece : MonoBehaviour, IConvertGameObjectToEntity
{
    public int type;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BoardPositionComponent());
    }
}
