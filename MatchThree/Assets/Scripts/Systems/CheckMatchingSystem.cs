using System.Collections;
using System.Collections.Generic;
using System.Text;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CheckMatchingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var pieces = Board.Pieces;
        if (Board.Pieces.Length == 0)
        {
            Entities.WithAll<PieceDataComponent>().ForEach((Entity entity, ref BoardPositionComponent component) =>
            {
                Board.Pieces.TryAdd(component.GridPosition, entity);
            });
        }
        
        Entities.WithAll<MatchingComponent>().ForEach((entity =>
        {
            EntityManager.RemoveComponent<MatchingComponent>(entity);
        }));
        for (int i = 0; i < Board.Height; i++)
        {
            for (int j = 0; j < Board.Width; j++)
            {
                var matchesHorizontal = GetHorizontalMatches(j, i);
                if (matchesHorizontal.Count > 2)
                {
                    j += matchesHorizontal.Count - 1;
                    foreach (var entity in matchesHorizontal)
                    {
                        if (entity != Entity.Null && !EntityManager.HasComponent<MatchingComponent>(entity))
                            EntityManager.AddComponent<MatchingComponent>(entity);
                    }
                }
            }
        }

        for (int i = 0; i < Board.Width; i++)
        {
            for (int j = 0; j < Board.Height; j++)
            {
                var matchesVertical = GetVerticalMatches(i, j);
                if (matchesVertical.Count > 2)
                {
                    j += matchesVertical.Count - 1;
                    foreach (var entity in matchesVertical)
                    {
                        if (entity != Entity.Null && !EntityManager.HasComponent<MatchingComponent>(entity))
                            EntityManager.AddComponent<MatchingComponent>(entity);
                    }
                }
            }
        }
    }

    private List<Entity> GetHorizontalMatches(int x, int y)
    {
        var pieces = Board.Pieces;
        var match = new List<Entity>();
        var currentPiecePos = new int2(x, y);
        pieces.TryGetValue(currentPiecePos, out var currentPiece);
        if (currentPiece == Entity.Null)
            return match;
        match.Add(currentPiece);
        for (var i = 1; x+i < Board.Width; i++)
        {
            var nextPiecePos = new int2(x+i, y);
            pieces.TryGetValue(nextPiecePos, out var nextPiece);
            if (EntityManager.GetComponentData<PieceDataComponent>(currentPiece).Type == EntityManager.GetComponentData<PieceDataComponent>(nextPiece).Type)
            {
                match.Add(nextPiece);
            }
            else
            {
                return match;
            }
        }
        return match;
    }
    
    private List<Entity> GetVerticalMatches(int x, int y)
    {
        var pieces = Board.Pieces;
        var match = new List<Entity>();
        var currentPiecePos = new int2(x, y);
        pieces.TryGetValue(currentPiecePos, out var currentPiece);
        if (currentPiece == Entity.Null)
            return match;
        match.Add(currentPiece);
        for (var i = 1; y+i < Board.Height; i++)
        {
            var nextPiecePos = new int2(x, y+i);
            pieces.TryGetValue(nextPiecePos, out var nextPiece);
            if (EntityManager.GetComponentData<PieceDataComponent>(currentPiece).Type == EntityManager.GetComponentData<PieceDataComponent>(nextPiece).Type)
            {
                match.Add(nextPiece);
            }
            else
            {
                return match;
            }
        }
        return match;
    }
}
