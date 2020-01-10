using System;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class MoveSystem:ComponentSystem
    {
        [BurstCompile]
        private struct MoveJob:IJobForEachWithEntity<Translation, BoardPositionComponent>
        {
            public int Width;
            public int Height;
            public void Execute(Entity entity, int index, ref Translation translation, [ReadOnly]ref BoardPositionComponent boardPositionComponent)
            {
                var x = boardPositionComponent.GridPosition.x - Width / 2;
                var y = boardPositionComponent.GridPosition.y - Height / 2;
                var oldX = translation.Value.x;
                var oldY = translation.Value.y;
                if (x != oldX || y != oldY)
                {
                    var nextX = oldX + (x - oldX) * 0.2f;
                    var nextY = oldY + (y - oldY) * 0.2f;
                    if (Math.Abs(x - nextX) < 0.02f && Math.Abs(y - nextY) < 0.02f)
                    {
                        nextX = x;
                        nextY = y;
                    }
                    translation.Value = new float3(nextX,nextY,0);
                }
            }
        }
        
        protected override void OnUpdate()
        {
            var job = new MoveJob
            {
                Width = Board.Width,
                Height = Board.Height
            };
            job.Schedule(this).Complete();
        }
    }
}