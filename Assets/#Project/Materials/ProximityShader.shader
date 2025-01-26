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

            uniform half3 _headPos, _bub0, _bub1,_bub2,_bub3,_bub4,_bub5,_bub6,_bub7,_bub8,_bub9;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                o.worldPos = TransformObjectToWorld(v.positionOS);
                return o;
            }

            float ApplyFade(float currentFade, float3 worldPos, half3 pos){
                float fadeDist = 0.5;
                float opaqueDist = 0.15;
                float dist = distance(worldPos, pos);
                float fade = saturate(1.0 - (dist / fadeDist));
                
                if(dist < opaqueDist)
                {
                    fade = 1.0;
                }

                return max(currentFade, fade);
            }

            half4 frag(Varyings i) : SV_Target
            {
                float fade = 0;
                fade = ApplyFade(fade, i.worldPos, _headPos);
                
                fade = ApplyFade(fade, i.worldPos, _bub0);
                fade = ApplyFade(fade, i.worldPos, _bub1);
                fade = ApplyFade(fade, i.worldPos, _bub2);
                fade = ApplyFade(fade, i.worldPos, _bub3);
                fade = ApplyFade(fade, i.worldPos, _bub4);
                fade = ApplyFade(fade, i.worldPos, _bub5);
                fade = ApplyFade(fade, i.worldPos, _bub6);
                fade = ApplyFade(fade, i.worldPos, _bub7);
                fade = ApplyFade(fade, i.worldPos, _bub8);
                fade = ApplyFade(fade, i.worldPos, _bub9);

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