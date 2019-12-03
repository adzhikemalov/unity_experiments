using Components;
using DefaultNamespace;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class DrawHpBarSystem:ComponentSystem
    {
        private Mesh _mesh;
        protected override void OnCreate()
        {
            base.OnCreate();

            _mesh = DrawCollidersSystem.CreateBoxMesh(new float2(1, 1));
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<HealthComponent>().WithAll<Translation>().ForEach((ref Translation translation, ref HealthComponent healthComponent) =>
            {
                var matrix = Matrix4x4.TRS(translation.Value+new float3(0, 0.7f, 0), Quaternion.identity, new Vector3(2, 0.35f, 0));
                Graphics.DrawMesh(_mesh, matrix, GameSettings.BulletMaterialInstance, 0);
                float health = (float)healthComponent.HP/(float)healthComponent.MaxHP;
                var hpMatrix = Matrix4x4.TRS(translation.Value+new float3(0.005f, 0.7f, 0), Quaternion.identity, new Vector3(1.9f*health, 0.3f, 0));
                Graphics.DrawMesh(_mesh, hpMatrix, GameSettings.HPBarMaterialInstance, 0);
            });
        }
    }
}