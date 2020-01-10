using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class SelectSystem : ComponentSystem
    {
        [BurstCompile]
        [ExcludeComponent(typeof(SelectedComponent))]
        private struct CheckRaySpheresIntersection : IJobForEachWithEntity<SimpleSphereColliderComponent, Translation, CanSelectComponent>
        {
            [ReadOnly] public Ray Ray;
            [WriteOnly] public NativeQueue<Entity>.ParallelWriter Collided;
 
            public void Execute(Entity entity, int index, [ReadOnly] ref SimpleSphereColliderComponent colliderComponent, [ReadOnly] ref Translation pos, [ReadOnly] ref CanSelectComponent c2)
            {
                if (CheckIntersection(Ray, colliderComponent, pos))
                {
                    Collided.Enqueue(entity);
                }
            }
        }

        private static bool CheckIntersection(Ray ray, SimpleSphereColliderComponent sphere, Translation sphereCenter)
        {
            // Find the vector between where the ray starts the the sphere's centre
            var center = sphereCenter.Value + sphere.centerOffset;
            var difference = center - (float3)ray.origin;
 
            var differenceLengthSquared = difference.x * difference.x + difference.y * difference.y + difference.z * difference.z;
            var sphereRadiusSquared     = sphere.radius * sphere.radius;

            // If the distance between the ray start and the sphere's centre is less than
            // the radius of the sphere, it means we've intersected. N.B. checking the LengthSquared is faster.
            if (differenceLengthSquared < sphereRadiusSquared)
            {
                return true;
            }
 
            var distanceAlongRay = math.dot(ray.direction, difference);
     
            // If the ray is pointing away from the sphere then we don't ever intersect
            if (distanceAlongRay < 0)
            {
                return false;
            }
 
            // Next we kinda use Pythagoras to check if we are within the bounds of the sphere
            // if x = radius of sphere
            // if y = distance between ray position and sphere centre
            // if z = the distance we've travelled along the ray
            // if x^2 + z^2 - y^2 < 0, we do not intersect
            var dist = sphereRadiusSquared + distanceAlongRay * distanceAlongRay - differenceLengthSquared;
 
            return !(dist < 0);
        }

        private Camera _mainCamera;
        protected override void OnCreate()
        {
            _mainCamera = Camera.main;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            var collidedEntities = new NativeQueue<Entity>(Allocator.TempJob);
            var checkIntersectionJob = new CheckRaySpheresIntersection()
            {
                Collided = collidedEntities.AsParallelWriter(),
                Ray      = mouseRay
            };
 
            checkIntersectionJob.Schedule(this).Complete();
 
            if (collidedEntities.Count > 0)
            {
                var collided = collidedEntities.Dequeue();
                PostUpdateCommands.AddComponent(collided, new SelectedComponent());
            }
 
            collidedEntities.Dispose();
        }
    }
}