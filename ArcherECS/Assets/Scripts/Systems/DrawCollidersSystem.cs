using System.Collections.Generic;
using Components;
using DefaultNamespace;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class DrawCollidersSystem:ComponentSystem
    {
        protected override void OnCreate()
        {
            _circleMeshes = new Dictionary<float, Mesh>();
            _boxMeshes = new Dictionary<float2, Mesh>();
            base.OnCreate();
        }
        
        private Dictionary<float, Mesh> _circleMeshes;
        private static Dictionary<float2, Mesh> _boxMeshes;
        public Mesh CreateDiskMesh(float radius,int radiusTiles,int tilesAround)
        {
            if (_circleMeshes.TryGetValue(radius, out var mesh))
            {
                return mesh;
            }
            
            var vertices = new Vector3[radiusTiles*tilesAround*6];
            var triangles = new int[radiusTiles*tilesAround*6];
            var uv = new Vector2[radiusTiles*tilesAround*6];
            var currentVertex = 0;
            
            var tileLength =  (float)radius / radiusTiles;    //the length of a tile parallel to the radius
            var radPerTile = 2 * Mathf.PI / tilesAround; //the radians the tile takes
            
            for(var angleNum = 0; angleNum < tilesAround; angleNum++)//loop around
            {
                var angle = (float)radPerTile * (float)angleNum;
                for(var offset = 0; offset < radiusTiles; offset++)//loop from the center outwards
                {
            
                    vertices[currentVertex]        =    new Vector3(Mathf.Cos(angle)*offset*tileLength                 , Mathf.Sin(angle)*offset*tileLength                ,0);
                    vertices[currentVertex + 1]    =    new Vector3(Mathf.Cos(angle + radPerTile)*offset*tileLength , Mathf.Sin(angle + radPerTile)*offset*tileLength    ,0);
                    vertices[currentVertex + 2]    =    new Vector3(Mathf.Cos(angle)*(offset + 1)*tileLength         , Mathf.Sin(angle)*(offset + 1)*tileLength            ,0);
                        
                    vertices[currentVertex + 3]    =    new Vector3(Mathf.Cos(angle + radPerTile)*offset*tileLength         , Mathf.Sin(angle + radPerTile)*offset*tileLength        ,0);
                    vertices[currentVertex + 4]    =    new Vector3(Mathf.Cos(angle + radPerTile)*(offset + 1)*tileLength     , Mathf.Sin(angle + radPerTile)*(offset + 1)*tileLength    ,0);
                    vertices[currentVertex + 5]    =    new Vector3(Mathf.Cos(angle)*(offset + 1)*tileLength                 , Mathf.Sin(angle)*(offset + 1)*tileLength                ,0);
            
                    currentVertex += 6;
                }
            }
            
            for(var j = 0; j < triangles.Length; j++)    //set the triangles
            {
                triangles[j] = j;
            }
            
            //create the mesh and apply vertices/triangles/UV
            var disk = new Mesh {vertices = vertices, triangles = triangles};
            disk.RecalculateNormals();
            disk.uv = uv;    //the UV doesnt need to be set
            _circleMeshes[radius] = disk;
            return disk;
        }
        
        public static Mesh CreateBoxMesh(float2 d1Bounds)
        {
            if (_boxMeshes.TryGetValue(d1Bounds, out var mesh))
            {
                return mesh;
            }
            
            var meshHalfWidth = d1Bounds.x/2;
            var meshHalfHeight = d1Bounds.y/2;
            
            mesh = new Mesh();
            var vertices = new Vector3[4];
            var triangles = new int[6];
            var uv = new Vector2[4];
    
            vertices[0] = new Vector3(-meshHalfWidth, meshHalfHeight);
            vertices[1] = new Vector3(meshHalfWidth, meshHalfHeight);
            vertices[2] = new Vector3(-meshHalfWidth, -meshHalfHeight);
            vertices[3] = new Vector3(meshHalfWidth, -meshHalfHeight);
        
            uv[0] = new Vector2(0,1);
            uv[1] = new Vector2(1,1);
            uv[2] = new Vector2(0,0);
            uv[3] = new Vector2(1,0);
    
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 2;
            triangles[4] = 1;
            triangles[5] = 3;

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            _boxMeshes[d1Bounds] = mesh;
            return mesh;
        }

        protected override void OnUpdate()
        {
            
            Entities.WithAll<CircleCollisionComponent>().WithAll<Translation>().ForEach(
                (ref Translation translation, ref CircleCollisionComponent collisionComponent) =>
                {
                    Graphics.DrawMesh(CreateDiskMesh(collisionComponent.Radius, 10, 100), translation.Value, Quaternion.identity, GameSettings.BulletMaterialInstance, 0);
                });
                
            Entities.WithAll<BoxCollisionComponent>().WithAll<Translation>().ForEach(
                (ref Translation translation, ref BoxCollisionComponent boxCollisionComponent) =>
                {
                    Graphics.DrawMesh(CreateBoxMesh(boxCollisionComponent.Bounds), translation.Value, quaternion.identity, GameSettings.BulletMaterialInstance, 0);
                });
        }

    }
}