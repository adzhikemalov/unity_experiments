using System.Collections.Generic;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    public class SwapSystem:ComponentSystem
    {
        protected override void OnUpdate()
        {
            EntityQuery m_Group = GetEntityQuery(typeof(SelectedComponent), typeof(BoardPositionComponent));
            var array = m_Group.ToEntityArray(Allocator.TempJob);
            var components = m_Group.ToComponentDataArray<BoardPositionComponent>(Allocator.TempJob);
            if (array.Length != 2 && components.Length != 2)
            {
                array.Dispose();
                components.Dispose();
                return;
            }
            
            var firstEntity = array[0];
            var secondEntity = array[1];
            var firstComponent = components[0];
            var secondComponent = components[1]; 
            
            PostUpdateCommands.SetComponent(firstEntity, secondComponent);
            PostUpdateCommands.SetComponent(secondEntity, firstComponent);
            PostUpdateCommands.RemoveComponent(firstEntity, typeof(SelectedComponent));
            PostUpdateCommands.RemoveComponent(secondEntity, typeof(SelectedComponent));

            Board.Pieces[firstComponent.GridPosition] = secondEntity; 
            Board.Pieces[secondComponent.GridPosition] = firstEntity; 
            
            array.Dispose();
            components.Dispose();
        }
    }
}