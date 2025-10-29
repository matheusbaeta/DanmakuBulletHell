Shader "Custom/InstancedSpriteIndirect_NoUV"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #include "UnityCG.cginc"

            struct BulletData
            {
                float3 position;
                float3 direction;
                float radius;
                float alive;
                float2 uvOffset;
                float2 uvSize;
                float2 uvScale;
            };

            sampler2D _MainTex;
            StructuredBuffer<float4x4> _PerInstanceData;
            StructuredBuffer<BulletData> bullets;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint instanceID : SV_InstanceID;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float4x4 m = _PerInstanceData[v.instanceID];
                BulletData b = bullets[v.instanceID];
                
                // Transform vertex by instance matrix
                float4 worldPos = mul(m, v.vertex);

                // Then transform by the global ViewProjection matrix
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);

                o.uv = v.uv * b.uvSize + b.uvOffset;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}