using System;

using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

using Xedrial.Rendering;
using Xedrial.Rendering.Systems;

using ProjectEdit.LevelsEditor.Components;

namespace ProjectEdit.LevelsEditor.Systems
{
    [UpdateBefore(typeof(SpriteRendererSystem))]
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
            Entities.WithoutBurst().WithAny<SelectableEntity>().ForEach((Entity entity, in LocalToWorld matrix, in SpriteRendererComponent spriteRenderer) =>
            {
                MaterialPropertyBlock materialPropertyBlock = new();

                materialPropertyBlock.SetTexture(s_MainTex, spriteRenderer.Sprite.texture);
                materialPropertyBlock.SetColor(s_Color, spriteRenderer.Color);
                materialPropertyBlock.SetColor(s_SelectionColor, IntToColor(entity.Index));

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
    }
}
