using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FFPSGraph @ 0x1700f5c..0x1701d70, Ghidra vaddr).
public class FFPSGraph : MonoBehaviour
{
	private const int cFrameHistory = 100;

	private const float cMaxFrameTime = 0.2f;

	private const float cTargetFrameTime = 0.018f;

	private const float cWarningFrameTime = 0.034f;

	private const float cLineWidth = 2f;

	private const float cWidth = 400f;

	private const float cHeight = 400f;

	protected static FFPSGraph msFPSGraph;

	protected CircularBuffer<float> mFrameTimes = new CircularBuffer<float>(100, allowOverflow: true);

	protected float mLastTime;

	private FLineNode mGraph;

	public static FFPSGraph Instance => msFPSGraph;

	public FLineNode Graph => mGraph;

	public static void Enable(bool enable)
	{
		if (!FApp.InitComplete)
		{
			return;
		}
		if (enable && msFPSGraph == null)
		{
			GameObject gameObject = new GameObject("zzzzzzFPSGraph");
			Object.DontDestroyOnLoad(gameObject);
			gameObject.layer = FPhysicsLayers.UIIndex;
			msFPSGraph = gameObject.AddComponent<FFPSGraph>();
			msFPSGraph.Init();
		}
		if (msFPSGraph != null)
		{
			msFPSGraph.gameObject.SetActive(enable);
			if (FApp.HUD != null)
			{
				PageDebugInit(FApp.HUD.PageDebug);
			}
		}
	}

	public static void PageDebugInit(FPageDebug page)
	{
		if (page != null && msFPSGraph != null && msFPSGraph.mGraph != null)
		{
			msFPSGraph.mGraph.x = 0f - Futile.Screen.HalfWidth;
			msFPSGraph.mGraph.y = 0f - Futile.Screen.HalfHeight;
			if (!msFPSGraph.gameObject.activeSelf)
			{
				msFPSGraph.mGraph.Unparent();
			}
			else
			{
				msFPSGraph.mGraph.SetParent(page);
			}
		}
	}

	private void Init()
	{
		mGraph = new FLineNode(2f);
		mLastTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = mLastTime;
		mLastTime = Time.realtimeSinceStartup;
		mFrameTimes.Put(realtimeSinceStartup - num);
		if (mGraph == null)
		{
			return;
		}
		mGraph.Points.Clear();
		mGraph.Colors.Clear();
		int num2 = 0;
		foreach (float mFrameTime in mFrameTimes)
		{
			mGraph.Points.Add(new Vector2((float)num2 * 4f, Mathf.Min(mFrameTime, 0.2f) * 2000f));
			if (mFrameTime <= 0.034f)
			{
				if (mFrameTime <= 0.018f)
				{
					mGraph.Colors.Add(FConfig.ColorBlindFPSGraph ? new Color(1f, 1f, 1f, 1f) : new Color(0f, 1f, 0f, 1f));
				}
				else
				{
					mGraph.Colors.Add(FConfig.ColorBlindFPSGraph ? new Color(0f, 0f, 1f, 1f) : new Color(1f, 0.92156863f, 0.015686275f, 1f));
				}
			}
			else
			{
				mGraph.Colors.Add(new Color(1f, 0f, 0f, 1f));
			}
			num2++;
		}
		mGraph.Invalidate();
	}
}
