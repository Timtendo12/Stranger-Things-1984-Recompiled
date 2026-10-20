using UnityEngine;

namespace JetFistGames.RetroTVFX
{
	[ExecuteInEditMode]
	public class CRTEffect : MonoBehaviour
	{
		private const int PASS_COMPOSITE_ENCODE = 0;

		private const int PASS_COMPOSITE_DECODE = 1;

		private const int PASS_COMPOSITE_FINAL = 2;

		private const int PASS_VGA = 4;

		private const int PASS_COMPONENT = 5;

		private const int PASS_SVIDEO_ENCODE = 6;

		private const int PASS_SVIDEO_DECODE = 7;

		private const int PASS_TV_OVERLAY = 3;

		[Header("Shader Properties")]
		[HideInInspector]
		public Shader shader;

		[Tooltip("Which style of video to use")]
		public VideoType VideoMode;

		public int DisplaySizeX;

		public int DisplaySizeY;

		public bool StretchToDisplay;

		public float AspectRatio;

		[Tooltip("Apply curvature to display")]
		public bool EnableTVCurvature;

		[Range(0f, 1f)]
		public float Curvature;

		[Tooltip("Overlay image applied (before curvature)")]
		public Texture2D TVOverlay;

		public bool EnablePixelMask;

		public Texture2D PixelMaskTexture;

		public int MaskRepeatX;

		public int MaskRepeatY;

		[Range(1f, 2f)]
		public float PixelMaskBrightness;

		public Vector2 IQOffset;

		public Vector2 IQScale;

		[Range(0f, 2f)]
		public float RFNoise;

		[Range(0f, 4f)]
		public float LumaSharpen;

		public bool QuantizeRGB;

		[Range(2f, 8f)]
		public int RBits;

		[Range(2f, 8f)]
		public int GBits;

		[Range(2f, 8f)]
		public int BBits;

		public bool EnableBurstCountAnimation;

		public bool AntiFlicker;

		public bool EnableRollingFlicker;

		[Range(0f, 1f)]
		public float RollingFlickerFactor;

		[Range(0f, 2f)]
		public float RollingVSyncTime;

		private Material material;

		private RenderTexture compositeTemp;

		private int frameCount;

		private float flickerOffset;

		private bool antiFlickerEnabled;

		private bool rollingFlickerEnabled;

		private bool pixelMaskEnabled;

		private bool tvCurvatureEnabled;

		private bool quantizeRGBEnabled;

		private bool rfEnabled;

		private float[] lumaFilter;

		private float[] chromaFilter;

		private Matrix4x4 rgb2yiq_mat;

		private Matrix4x4 yiq2rgb_mat;

		private void OnDisable()
		{
		}

		private void Update()
		{
		}

		private void LateUpdate()
		{
		}

		private void ensureResources()
		{
		}

		private void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
		}

		private void setKeyword(string keyword, bool enabled, ref bool keywordEnabled)
		{
		}

		private void blitQuad(RenderTexture src, RenderTexture dest, Material material, int pass)
		{
		}

		private void blitQuad(Rect rect, RenderTexture src, RenderTexture dest, Material material, int pass)
		{
		}
	}
}
