Shader "Sprites/Default Saturate" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_SaturationColor ("Saturation Tint", Color) = (1,1,1,1)
		_SaturationPhase ("Saturation Phase", Range(0.0, 1.0)) = 0.0
		_SaturationIntensity ("Saturation Intensity", Range(0, 30)) = 5.0
	}

	SubShader {
		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _SaturationColor;
			float _SaturationPhase;
			float _SaturationIntensity;
			
			struct appdata {
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata IN) {
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target {
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color * _Color;
				
				float luma = dot(c.rgb, float3(0.2126729, 0.7151522, 0.0721750));
				fixed3 st = luma * _SaturationColor.rgb + _SaturationPhase * (c.rgb - luma);
				c.rgb += st * _SaturationIntensity * _SaturationPhase;
				
				c.rgb *= c.a;
				
				return c;
			}
			ENDCG
		}
	}
}
