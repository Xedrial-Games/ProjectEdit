using System;

using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

using Xedrial.Graphics;

using ProjectEdit.LevelsEditor.Components;

namespace ProjectEdit.LevelsEditor.Systems
{
    public partial class SelectionSystem : SystemBase
    {
        private static readonly int s_MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int s_Color = Shader.PropertyToID("_Color");
        private static readonly int s_SelectionColor = Shader.PropertyToID("_SelectionColor");

        private Material m_SelectionMaterial;
        private Camera m_SelectionCamera;

        protected override void OnStartRunning()
        {
            var levelEditor = LevelEditor.Instance;
            m_SelectionCamera = levelEditor.SelectionCamera;
            m_SelectionMaterial = levelEditor.SelectionMaterial;
        }

        protected override void OnUpdate()
        {
            var materialPropertyBlock = new MaterialPropertyBlock();

            Entities
                .WithoutBurst()
                .WithAll<SelectableEntity>()
                .ForEach((Entity entity, in LocalToWorld matrix,
                    in SpriteRendererComponent spriteRenderer) =>
                {
                    materialPropertyBlock.SetTexture(s_MainTex, spriteRenderer.Sprite.texture);
                    materialPropertyBlock.SetColor(s_Color, spriteRenderer.Color);
                    materialPropertyBlock.SetColor(s_SelectionColor, ShortsToColor((short)entity.Index, (short)entity.Version));
    
                    Graphics.DrawMesh(
                        spriteRenderer.Mesh,
                        matrix.Value,
                        m_SelectionMaterial,
                        0,
                        m_SelectionCamera,
                        0,
                        materialPropertyBlock
                    );
                }).Run();
        }

        private static Color32 IntToColor(int number)
        {
            byte[] intBytes = BitConverter.GetBytes(number);
            return new Color32(intBytes[0], intBytes[1], intBytes[2], intBytes[3]);
        }

        private static Color32 ShortsToColor(short a, short b)
        {
            byte[] aBytes = BitConverter.GetBytes(a);
            byte[] bBytes = BitConverter.GetBytes(b);
            
            return new Color32(aBytes[0], aBytes[1], bBytes[0], bBytes[1]);
        }
    }
}
