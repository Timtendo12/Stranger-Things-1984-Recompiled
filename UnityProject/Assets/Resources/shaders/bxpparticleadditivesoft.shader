// PC port replacement (original shader program not recoverable from the Android build).
// Soft additive particle: Blend One OneMinusSrcColor, texture * particle vertex color.
Shader "BonusXP/BXPParticleAdditiveSoft" {
	Properties {
		_MainTex ("Particle Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
		ZWrite Off
		Lighting Off
		Cull Off
		Blend One OneMinusSrcColor
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
			struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv) * i.color;
				c.rgb *= c.a;
				return c;
			}
			ENDCG
		}
	}
}
