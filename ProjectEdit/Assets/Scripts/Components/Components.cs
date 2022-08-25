using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

namespace ProjectEdit.Components
{
    [MaterialProperty("_MainTex", MaterialPropertyFormat.Float2x4)]
    public struct MainTexMaterialProperty : IComponentData
    {
        public float2x4 Value;
    }
}
