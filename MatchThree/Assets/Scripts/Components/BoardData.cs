using Unity.Entities;

public struct BoardData : IComponentData
{
    public int width;
    public int height;
    public Entity BluePrefab;
    public Entity RedPrefab;
    public Entity GreenPrefab;
}