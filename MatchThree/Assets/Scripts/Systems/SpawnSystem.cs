using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

namespace Systems
{
    public class SpawnSystem : JobComponentSystem
    {
        private struct SpawnJob:IJobForEachWithEntity<BoardData>
        {
            public NativeHashMap<int2, Entity>.ParallelWriter Pieces;
            public EntityCommandBuffer.Concurrent CommandBuffer;
            public Unity.Mathematics.Random Random;
            public void Execute(Entity entity, int index, ref BoardData board)
            {
//                for (int i = 0; i < board.width; i++)
//                {
//                    for (int j = 0; j < board.height; j++)
//                    {
//                        var type = Random.NextInt(1, 4);
//                        var instance = CommandBuffer.Instantiate(index, GetPrefab(board, type));
//                        CommandBuffer.SetComponent(index, instance, new Translation{Value = new float3(i-board.width/2, j-board.height/2, 0)});
//                        CommandBuffer.SetComponent(index, instance, new BoardPositionComponent{GridPosition = new int2(i, j)});
//                        CommandBuffer.AddComponent(index, instance, new SimpleSphereColliderComponent{radius = 0.5f});
//                        CommandBuffer.AddComponent(index, instance, new PieceDataComponent {Type = type});
//                        CommandBuffer.AddComponent(index, instance, new CanSelectComponent());
//                    }
//                }
//                
//                CommandBuffer.DestroyEntity(index, entity);
            }
        
            private static Entity GetPrefab(BoardData board, int type)
            {
                var result = board.BluePrefab;
                switch (type)
                {
                    case 1:
                        result = board.RedPrefab;
                        break;
                    case 2:
                        result = board.BluePrefab;
                        break;
                    case 3:
                        result = board.GreenPrefab;
                        break;
                }

                return result;
            }
        }

        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new SpawnJob
            {
                Pieces = Board.Pieces.AsParallelWriter(),
                CommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
                Random = new Unity.Mathematics.Random((uint)Random.Range(int.MinValue, int.MaxValue))
            }.Schedule(this, inputDeps);
            _entityCommandBufferSystem.AddJobHandleForProducer(job);
            return job;
        }
    }
}
