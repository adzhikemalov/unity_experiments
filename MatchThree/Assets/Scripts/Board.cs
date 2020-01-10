using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequiresEntityConversion]
public class Board : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public static int Width;
    public static int Height;
    public static Material SelectMaterialInstance;
    public static NativeHashMap<int2, Entity> Pieces;

    public Material SelectionMaterial;
    public GameObject BluePrefab;
    public GameObject RedPrefab;
    public GameObject GreenPrefab;
    public int width = 2;
    public int height = 3;
    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BluePrefab);
        referencedPrefabs.Add(RedPrefab);
        referencedPrefabs.Add(GreenPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Width = width;
        Height = height;
        SelectMaterialInstance = SelectionMaterial;
        Pieces = new NativeHashMap<int2, Entity>(Width*Height, Allocator.Persistent);
        
        var boardData = new BoardData
        {
            BluePrefab = conversionSystem.GetPrimaryEntity(BluePrefab),
            GreenPrefab = conversionSystem.GetPrimaryEntity(GreenPrefab),
            RedPrefab = conversionSystem.GetPrimaryEntity(RedPrefab),
            width = width, height = height
        };
        dstManager.AddComponentData(entity, boardData);
    }
}
