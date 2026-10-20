// PC port replacement (original shader program not recoverable from the Android build).
Shader "BonusXP/BXPDynamicFontOutlineVertShadow" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OutlineColor ("OutlineColor", Vector) = (0,0,0,1)
		_ShadowColor ("ShadowColor", Vector) = (0,0,0,1)
		_OutlineParams ("OutlineParams", Vector) = (1.5,3.5,6,0.8)
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
			float4 _OutlineParams;
			fixed4 frag(bxp_v2f i) : SV_Target
			{
				float a = tex2D(_MainTex, i.uv).a;
				float2 d = _MainTex_TexelSize.xy * _OutlineParams.x;
				float o = 0;
				o = max(o, tex2D(_MainTex, i.uv + float2(d.x, 0)).a);
				o = max(o, tex2D(_MainTex, i.uv - float2(d.x, 0)).a);
				o = max(o, tex2D(_MainTex, i.uv + float2(0, d.y)).a);
				o = max(o, tex2D(_MainTex, i.uv - float2(0, d.y)).a);
				float s = tex2D(_MainTex, i.uv + float2(0, d.y * _OutlineParams.y * 0.5)).a;
				fixed4 back = lerp(fixed4(_ShadowColor.rgb, _ShadowColor.a * s * _OutlineParams.w), fixed4(_OutlineColor.rgb, _OutlineColor.a), o);
				fixed4 c = lerp(back, fixed4(i.color.rgb + i.tint.rgb, 1), a);
				c.a *= i.color.a;
				c.rgb = bxp_sepia(c.rgb);
				return c;
			}
			ENDCG
		}
	}
}
