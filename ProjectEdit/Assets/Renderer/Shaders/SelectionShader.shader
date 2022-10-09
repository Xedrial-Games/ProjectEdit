Shader "Custom/Selection ID"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            CGPROGRAM

            #pragma vertex SelectionVertex
            #pragma fragment SelectionFragment

            #include "UnityCG.cginc"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _SelectionColor;
            sampler2D _MainTex;

            Varyings SelectionVertex(const Attributes v)
            {
                Varyings o;

                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 SelectionFragment(const Varyings i) : SV_Target
            {
                const fixed4 o = tex2D(_MainTex, i.uv) * _Color;
                if (o.w == 0.0f)
                    return fixed4(0,0,0,0);
                
                return _SelectionColor;
            }
            
            ENDCG
        }
    }
}