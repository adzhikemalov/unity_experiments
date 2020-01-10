using Unity.Entities;

namespace Components
{
    public struct SelectedComponent:IComponentData
    {
        public Entity SelectedView;
    }

    public struct CanSelectComponent : IComponentData
    {
        
    }

    public struct MatchingComponent : IComponentData
    {
        
    }
}