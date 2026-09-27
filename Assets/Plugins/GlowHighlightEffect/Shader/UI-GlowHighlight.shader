Shader "Custom/UI/GlowHighlight"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Space(10)]
        [Header(Glow)]
        _GlowColor ("Glow Color", Color) = (0,1,1,1)
        _GlowAlpha ("Glow Alpha", Range(0,1)) = 1.0
        _GlowSize ("Glow Size (px)", Range(1, 30)) = 8
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.2
        _GlowFalloff ("Glow Falloff", Range(0.1, 4)) = 0.45
        _ShadowThreshold("Shadow Threshold", Range(0,1)) = 0.8
        _Quality("Quality", Range(1, 12)) = 6

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma target   3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _GlowColor;
            float _GlowAlpha;
            float _GlowSize;
            float _GlowIntensity;
            float _GlowFalloff;
            float _ShadowThreshold;
            float _Quality;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, IN.texcoord);
                float texAlpha = texColor.a;
                fixed4 spriteColor = texColor * IN.color;

                if (IN.color.a < 0.01f)
                {
                    return spriteColor;
                }
                
                float glowAlpha = step(_ShadowThreshold, texAlpha);

                float2 stepSize = _MainTex_TexelSize.xy * _GlowSize * 0.25;
                float shadowScale = max(0.001, 1.0 - _ShadowThreshold);

                float glowSum = 0;
                float weightSum = 0;

                int q = (int)_Quality;
                float gaussK = 2.16 / float(q * q);
                float q2 = float(q * q);

                for (int xi = -q; xi <= q; xi++)
                {
                    for (int yi = -q; yi <= q; yi++)
                    {
                        float dist2 = float(xi * xi + yi * yi);
                        if (dist2 > q2) continue;
                        float2 offset = float2(xi, yi) * stepSize;
                        float weight = exp(-dist2 * gaussK);
                        float rawA = tex2Dlod(_MainTex, float4(IN.texcoord + offset, 0, 0)).a;
                        float sAlpha = saturate((rawA - _ShadowThreshold) / shadowScale);
                        glowSum += sAlpha * weight;
                        weightSum += weight;
                    }
                }

                float avgAlpha = glowSum / weightSum;
                
                float smoothAvg = pow(smoothstep(0.0, 1.0, avgAlpha), _GlowFalloff);

                float outerGlow = (1.0 - glowAlpha) * smoothAvg;

                fixed4 result;

                result.rgb = lerp(_GlowColor.rgb * _GlowIntensity, spriteColor.rgb, glowAlpha);
                result.a = saturate(texAlpha + outerGlow * _GlowAlpha * _GlowIntensity) * IN.color.a;

                #ifdef UNITY_UI_CLIP_RECT
                result.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(result.a - 0.001);
                #endif

                return result;
            }
            ENDCG
        }
    }
}
