Shader "Custom/TransparentShaderWithFade"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _FadeDistance ("Fade Distance", Float) = 1.0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            float4 _MainColor;
            float3 _FadePosition;
            float _FadeDistance;

            uniform half4 _headPos;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                o.worldPos = TransformObjectToWorld(v.positionOS);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Calculate distance to the fade position
                float dist = distance(i.worldPos, _headPos);

                // Compute fade factor (1.0 at the fade position, 0.0 at the fade distance)
                float fade = saturate(1.0 - (dist / _headPos));

                // Apply fade to the main color's alpha
                float4 color = _MainColor;
                color.a *= fade;

                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Unlit/Transparent"
}