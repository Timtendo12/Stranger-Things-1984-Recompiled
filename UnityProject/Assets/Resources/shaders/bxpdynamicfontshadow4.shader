// PC port replacement (original shader program not recoverable from the Android build).
Shader "BonusXP/BXPDynamicFontShadow4" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OutlineColor ("OutlineColor", Vector) = (0,0,0,1)
		_ShadowColor ("ShadowColor", Vector) = (1,0,0,1)
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		ZWrite Off
		Lighting Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			CGPROGRAM
			#pragma vertex bxp_vert
			#pragma fragment frag
			#include "Assets/ShaderIncludes/BXPSprite.cginc"
			float4 _MainTex_TexelSize;
			fixed4 _OutlineColor;
			fixed4 _ShadowColor;
			fixed4 frag(bxp_v2f i) : SV_Target
			{
				float a = tex2D(_MainTex, i.uv).a;
				float2 d = _MainTex_TexelSize.xy;
				float s = max(max(tex2D(_MainTex, i.uv - d).a, tex2D(_MainTex, i.uv - float2(d.x, 0)).a), max(tex2D(_MainTex, i.uv - float2(0, d.y)).a, tex2D(_MainTex, i.uv - 2 * d).a));
				fixed4 c = lerp(fixed4(_ShadowColor.rgb, _ShadowColor.a * s), fixed4(i.color.rgb + i.tint.rgb, 1), a);
				c.a *= i.color.a;
				c.rgb = bxp_sepia(c.rgb);
				return c;
			}
			ENDCG
		}
	}
}
