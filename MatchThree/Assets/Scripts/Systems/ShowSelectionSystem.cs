using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class ShowSelectionSystem:ComponentSystem
    {
        private static Mesh _selectionMesh;
        protected override void OnUpdate()
        {
            if (_selectionMesh == null)
            {
                _selectionMesh = new Mesh();
                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];
                var meshHalfWidth = .5f;
                var meshHalfHeight = .5f;
                
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

                _selectionMesh.vertices = vertices;
                _selectionMesh.uv = uv;
                _selectionMesh.triangles = triangles;
            }
            Entities.WithAll<MatchingComponent>().ForEach( (ref Translation translation) =>
            {
                Graphics.DrawMesh(_selectionMesh, translation.Value, Quaternion.identity, Board.SelectMaterialInstance, 0);
            });
            
        }
    }
}