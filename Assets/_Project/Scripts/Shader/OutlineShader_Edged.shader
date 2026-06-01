Shader "Custom/OutlineShader_Edged"
{
    Properties
    {
        [PerRendererData][MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}

        [HDR] _OutlineTint ("Outline Tint HDR", Color) = (1, 1, 1, 1)

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
            Name "SpriteWithPixelOutline"
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

                // UV 기준으로 좌하/우상 방향 결정.
                float2 cornerSign = step(float2(0.5, 0.5), v.uv) * 2.0 - 1.0;

                float4 positionHCS = TransformObjectToHClip(v.positionOS.xyz);

                // 화면 픽셀 기준으로 메시 확장.
                float2 pixelToClip = 2.0 / _ScreenParams.xy;
                positionHCS.xy += cornerSign * pixelToClip * _OutlineWidth * positionHCS.w;

                // 확장된 메시 영역에 맞춰 UV도 바깥으로 확장.
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

                // 사각형 커널 샘플링.
                // 원형 외곽선이 아니라 네모난 픽셀 외곽선 느낌.
                [loop]
                for (int y = -MAX_RADIUS; y <= MAX_RADIUS; y++)
                {
                    if (abs(y) > radius)
                        continue;

                    [loop]
                    for (int x = -MAX_RADIUS; x <= MAX_RADIUS; x++)
                    {
                        if (abs(x) > radius)
                            continue;

                        float2 offset = float2(x, y) * _MainTex_TexelSize.xy;
                        maxAlpha = max(maxAlpha, AlphaAt(i.uv + offset));
                    }
                }

                // 중심은 비어 있는데 주변에 알파가 있으면 outline.
                half rawOutline = saturate(maxAlpha - centerAlpha);

                // 픽셀처럼 딱 끊기게 처리.
                half outline = step(_AlphaThreshold, rawOutline);

                // 원본 스프라이트는 SpriteRenderer Color 영향을 받음.
                // SpriteRenderer Color를 검정으로 하면 몸체는 검정.
                half4 spriteCol = SpriteAt(i.uv, i.color);

                // 외곽선은 SpriteRenderer Color 영향을 받지 않음.
                // 몸체가 검정이어도 외곽선은 Outline Tint 그대로.
                half4 outlineCol = _OutlineTint;
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