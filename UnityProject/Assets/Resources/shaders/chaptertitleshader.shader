// PC port of the Android GLES3 program (decompressed from the shipped shader blob).
// Pixels brighter than 0.5 on every channel (the white title card) fade with _WhiteAlpha,
// everything else (black backdrop, red title text) fades with _Alpha only.
Shader "Unlit/ChapterTitleShader" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Alpha ("Alpha", Float) = 1
		_WhiteAlpha ("WhiteAlpha", Float) = 1
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Alpha;
			float _WhiteAlpha;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 c = tex2D(_MainTex, i.uv).rgb;
				bool white = all(c > 0.5);
				return fixed4(c, white ? _Alpha * _WhiteAlpha : _Alpha);
			}
			ENDCG
		}
	}
}
