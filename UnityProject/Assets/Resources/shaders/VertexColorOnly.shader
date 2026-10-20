// PC port replacement (original shader program not recoverable from the Android build).
Shader "VertexColorOnly" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}

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
			fixed4 frag(bxp_v2f i) : SV_Target
			{
				return fixed4(bxp_sepia(i.color.rgb), i.color.a);
			}
			ENDCG
		}
	}
}
