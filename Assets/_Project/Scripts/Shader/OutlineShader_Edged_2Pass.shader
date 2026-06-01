Shader "Custom/OutlineShader_Edged_2Pass"
{
    Properties
    {
        [PerRendererData][MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}

        [HDR] _OutlineTint ("Outline Tint HDR", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width px", Range(1, 32)) = 2
        _AlphaThreshold("Alpha Threshold", Range(0, 1)) = 0.08
        _BloomIntensity("Bloom Intensity", Range(1, 12)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                half4 color       : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineTint;
                float _OutlineWidth;
                float _AlphaThreshold;
                float _BloomIntensity;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;

                float2 cornerSign = step(float2(0.5, 0.5), v.uv) * 2.0 - 1.0;
                float2 outlineTexels = _MainTex_TexelSize.xy * _OutlineWidth;

                float4 positionOS = v.positionOS;
                positionOS.xy += cornerSign * outlineTexels;

                o.positionHCS = TransformObjectToHClip(positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + cornerSign * outlineTexels;

                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                if (i.uv.x >= 0.0 && i.uv.x <= 1.0 && i.uv.y >= 0.0 && i.uv.y <= 1.0)
                    return half4(0, 0, 0, 0);

                half outline = half(1.0);
                half3 rgb = _OutlineTint.rgb * _BloomIntensity;

                return half4(rgb, _OutlineTint.a * outline);
            }

            ENDHLSL
        }

        Pass
        {
            Name "Sprite"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                half4 color       : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                half4 color        : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_ST;

            Varyings vert(Attributes v)
            {
                Varyings o;

                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                if (i.uv.x < 0.0 || i.uv.x > 1.0 || i.uv.y < 0.0 || i.uv.y > 1.0)
                    return half4(0, 0, 0, 0);

                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;
            }

            ENDHLSL
        }
    }
}
