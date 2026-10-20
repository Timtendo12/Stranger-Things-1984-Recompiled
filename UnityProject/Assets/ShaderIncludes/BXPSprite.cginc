// PC port: shared code for the hand-written replacements of BonusXP's 2D shaders (the shipped GLES/Vulkan
// shader programs cannot be decompiled). Vertex color tints, tangent.rgb is Futile's additive tint color,
// _SEPIA_GLOBAL fades the result to sepia (Futile sets it while drawing behind sepia overlays).
#include "UnityCG.cginc"

sampler2D _MainTex;
float4 _MainTex_ST;
float _SEPIA_GLOBAL;

struct bxp_appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float4 color : COLOR;
	float4 tangent : TANGENT;
};

struct bxp_v2f
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	fixed4 color : COLOR;
	fixed4 tint : TEXCOORD1;
};

bxp_v2f bxp_vert(bxp_appdata v)
{
	bxp_v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	o.color = v.color;
	o.tint = v.tangent;
	return o;
}

fixed3 bxp_sepia(fixed3 c)
{
	fixed3 s = fixed3(dot(c, fixed3(0.393, 0.769, 0.189)), dot(c, fixed3(0.349, 0.686, 0.168)), dot(c, fixed3(0.272, 0.534, 0.131)));
	return lerp(c, saturate(s), _SEPIA_GLOBAL);
}
