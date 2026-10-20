// PC port replacement (original shader program not recoverable from the Android build).
Shader "Custom/Pixelate" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_PixelateX ("Pixelate X", Float) = 5
		_PixelateY ("Pixelate Y", Float) = 5
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _PixelateX;
			float _PixelateY;
			fixed4 frag(v2f_img i) : SV_Target
			{
				float2 res = _MainTex_TexelSize.zw;
				float2 block = max(float2(_PixelateX, _PixelateY), 1.0);
				float2 uv = (floor(i.uv * res / block) + 0.5) * block / res;
				return tex2D(_MainTex, uv);
			}
			ENDCG
		}
	}
}
