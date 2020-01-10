using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public struct RemoveMatchesJob:IJobForEachWithEntity<MatchingComponent, PieceDataComponent>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public void Execute(Entity entity, int index, [ReadOnly]ref MatchingComponent matching, ref PieceDataComponent pieceData)
        {
            Debug.Log(pieceData);
        }
    }
    
    public class RemoveMatchingSystem : ComponentSystem    
    {
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            var job = new RemoveMatchesJob
            {
                CommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
            };
            job.Schedule(this).Complete();
        }
    }
}