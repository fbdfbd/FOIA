Shader "Custom/EnemyShader_Fixed"
{
    Properties
    {
        [PerRendererData][MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}

        [HDR][MainColor] _OutlineTint ("Outline Tint HDR", Color) = (1, 1, 1, 1)

        _OutlineWidth("Outline Width px", Range(1, 16)) = 2
        _AlphaThreshold("Alpha Threshold", Range(0, 1)) = 0.08
        _EdgeSoftness("Edge Softness", Range(0.5, 3)) = 1.2
        _BloomIntensity("Bloom Intensity", Range(1, 12)) = 3
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
            Name "SpriteWithOutline"
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
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineTint;
                float _OutlineWidth;
                float _AlphaThreshold;
                float _EdgeSoftness;
                float _BloomIntensity;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;

                float2 uv = TRANSFORM_TEX(v.uv, _MainTex);

                float2 cornerSign = step(float2(0.5, 0.5), v.uv) * 2.0 - 1.0;

                float4 positionHCS = TransformObjectToHClip(v.positionOS.xyz);

                // 화면 픽셀 기준으로 버텍스를 확장
                float2 pixelToClip = 2.0 / _ScreenParams.xy;
                positionHCS.xy += cornerSign * pixelToClip * _OutlineWidth * positionHCS.w;

                // 확장된 메시 영역에 맞게 UV도 바깥으로 확장
                uv += cornerSign * _MainTex_TexelSize.xy * _OutlineWidth;

                o.positionHCS = positionHCS;
                o.uv = uv;
                o.color = v.color;

                return o;
            }

            half AlphaAt(float2 uv)
            {
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                    return half(0.0);

                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
            }

            half4 SpriteAt(float2 uv, half4 tint)
            {
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                    return half4(0, 0, 0, 0);

                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * tint;
            }

            half4 frag(Varyings i) : SV_Target
            {
                const int MAX_RADIUS = 16;

                int radius = (int)clamp(_OutlineWidth + 0.5, 1.0, (float)MAX_RADIUS);

                half centerAlpha = AlphaAt(i.uv);
                half maxAlpha = half(0.0);

                [loop]
                for (int r = 1; r <= MAX_RADIUS; r++)
                {
                    if (r > radius)
                        break;

                    float2 p = _MainTex_TexelSize.xy * r;
                    float2 d = p * 0.70710678;

                    // 8방향 샘플링
                    maxAlpha = max(maxAlpha, AlphaAt(i.uv + float2( p.x, 0.0)));
                    maxAlpha = max(maxAlpha, AlphaAt(i.uv + float2(-p.x, 0.0)));
                    maxAlpha = max(maxAlpha, AlphaAt(i.uv + float2(0.0,  p.y)));
                    maxAlpha = max(maxAlpha, AlphaAt(i.uv + float2(0.0, -p.y)));

                    maxAlpha = max(maxAlpha, AlphaAt(i.uv + float2( d.x,  d.y)));
                    maxAlpha = max(maxAlpha, AlphaAt(i.uv + float2(-d.x,  d.y)));
                    maxAlpha = max(maxAlpha, AlphaAt(i.uv + float2( d.x, -d.y)));
                    maxAlpha = max(maxAlpha, AlphaAt(i.uv + float2(-d.x, -d.y)));
                }

                half rawOutline = saturate(maxAlpha - centerAlpha);

                half w = max(fwidth(rawOutline), half(0.0001)) * _EdgeSoftness;
                half outline = smoothstep(_AlphaThreshold - w, _AlphaThreshold + w, rawOutline);

                half4 spriteCol = SpriteAt(i.uv, i.color);

                half4 outlineCol = _OutlineTint * i.color;
                half3 outlineRgb = outlineCol.rgb * (_BloomIntensity * outline);
                half outlineAlpha = outlineCol.a * outline;

                half3 finalRgb = lerp(spriteCol.rgb, outlineRgb, outline);
                half finalAlpha = saturate(max(spriteCol.a, outlineAlpha));

                return half4(finalRgb, finalAlpha);
            }

            ENDHLSL
        }
    }
}