using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FBeamRenderer : MonoBehaviour
{
	public enum TextureModes
	{
		Tile = 0,
		Stretch = 1
	}

	protected List<FBeamPoint> mPoints;

	protected bool mPointsDirty;

	public float SegmentLength;

	public bool UseWorldSpace;

	public Material Material;

	public Color OverallColor;

	public float Intensity;

	public float WidthScale;

	public bool FadeEndSegments;

	public float SinWidthPeriod;

	public float SinWidthMinScale;

	public float SinWidthMaxScale;

	public float SinWidthOffsetRads;

	public GameObject StartEffectPrefab;

	public GameObject EndEffectPrefab;

	public GameObject MiddleEffectPrefab;

	public float MiddleEffectSpacing;

	public float MiddleEffectStartMargin;

	public float MiddleEffectEndMargin;

	public TextureModes TextureMode;

	public float TextureUScale;

	public float TextureUOffset;

	public int TextureSheetTilesU;

	public int TextureSheetTilesV;

	public float TextureSheetCycleTime;

	public float CurrentTime;

	protected Mesh mMesh;

	protected static float mTotalLength;

	protected int mCurrentTextureSheetFrameU;

	protected int mCurrentTextureSheetFrameV;

	protected static List<Vector3> sTempVertices;

	protected static List<Color> sTempColors;

	protected static List<Vector2> sTempUVs;

	protected static List<Vector2> sTempUV2s;

	protected static List<int> sTempIndices;

	protected GameObject mStartEffect;

	protected GameObject mEndEffect;

	protected List<GameObject> mMiddleEffects;

	public static HashSet<FBeamRenderer> sBeamInstances;

	public int PointCount => 0;

	public void Awake()
	{
	}

	public void Init()
	{
	}

	public void Update()
	{
	}

	public void OnValidate()
	{
	}

	public void OnWillRenderObject()
	{
	}

	public void SetPointPosition(int index, Vector3 pos)
	{
	}

	protected float GetUForDistance(float distance)
	{
		return 0f;
	}

	protected float GetWidthScaleForDistance(float distance)
	{
		return 0f;
	}

	protected void AddPointToMesh(Vector3 pos, Vector3 offset, Color color, float u)
	{
	}

	protected Vector3 GetDirectionAtIndex(int index)
	{
		return default;
	}

	protected Vector3 GetWorldSpacePositionAtIndex(int index)
	{
		return default;
	}

	protected float AddSegment(int startIndex, int endIndex, Vector3 cameraPos, float distanceSoFar)
	{
		return 0f;
	}

	public void UpdateMesh(Camera camera)
	{
	}

	public void OnEnable()
	{
	}

	public void OnDisable()
	{
	}

	public void OnDestroy()
	{
	}

	public void OnPointsChanged()
	{
	}

	protected void ClearMiddleEffects()
	{
	}

	public void UpdateMiddleEffects()
	{
	}

	public Vector3 GetPositionForDistance(float distance)
	{
		return default;
	}

	public static void BillboardInstances(Camera camera)
	{
	}
}
