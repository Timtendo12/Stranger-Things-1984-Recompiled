using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (Futile @ 0x16c1030..0x16cacc4, Ghidra vaddr).
// BonusXP's fork of Futile: a single hidden ortho camera rendered manually (Camera.Render) per clipping region,
// a helper camera that drives rendering (FutileRender) and owns the AudioListener, and FScissorRect clipping.
public class Futile : MonoBehaviour
{
	public static Futile Instance;

	public static FScreen Screen;

	public static FAtlasManager AtlasManager;

	public static FTouchManager TouchManager;

	public static bool NeedHalfTexelOffset;

	public static float DisplayScale;

	public static float DisplayScaleInverse;

	public static float ResourceScale;

	public static float ResourceScaleInverse;

	public static string ResourceSuffix;

	public static bool OnSmallScreen;

	internal static int StartingQuadsPerLayer;

	internal static int QuadsPerLayerExpansion;

	internal static int MaxEmptyQuadsPerLayer;

	protected static List<FStage> mStages = new List<FStage>();

	protected static bool mDepthChangeNeeded;

	protected List<Action> mSignalUpdateList = new List<Action>();

	protected List<Action> mSignalLateUpdateList = new List<Action>();

	protected List<Action<string>> mSignalKeyInputList = new List<Action<string>>();

	public const int cActiveCameraLayer = 30;

	public const int cInactiveCameraLayer = 31;

	protected static Camera mCamera;

	protected static bool mFirstCameraGotten;

	protected static List<FRenderLayer> mCurrentCameraRenderLayers = new List<FRenderLayer>();

	protected static bool mCameraClearColor = true;

	protected static Camera mHelperCamera;

	protected static FStage mCurrentRenderStage;

	protected static List<Rect> mClippingStack = new List<Rect>();

	protected static bool mJustClearedClipStack;

	protected static List<FNode> mFocusStack = new List<FNode>();

	protected FutileParams mFutileParams;

	protected float mGOZValue;

	public Action<Action<bool, string>, FKeyboardOptions, string, string> OpenAppKeyboard;

	protected static FNode mSepiaUnderNode;

	protected static HashSet<FUISepiaCameraHelper> mSepiaCameraHelpers = new HashSet<FUISepiaCameraHelper>();

	protected static float mCurrentSepiaFactor;

	public static bool mIsForceRendering;

	public static bool mIsFutileRenderingOuter;

	public static bool mIsFutileRenderingDraw;

	private int LastDFGlobalCounter;

	public static bool IsInAGame;

	private static int mCollectCount;

	private static float mNextCollectTime;

	protected int mLastFrameUpdate = -1;

	public static bool UseSignalLists = true;

	public float GOZValue
	{
		get
		{
			return mGOZValue;
		}
		set
		{
			mGOZValue = value;
			float minimum = mCamera.nearClipPlane + 100f;
			if (mGOZValue < minimum)
			{
				mGOZValue = minimum;
			}
		}
	}

	public static List<FStage> Stages => mStages;

	protected event Action mSignalUpdate;

	protected event Action mSignalLateUpdate;

	protected event Action<string> mSignalKeyInput;

	protected void Awake()
	{
		Instance = this;
		NeedHalfTexelOffset = (int)SystemInfo.graphicsDeviceType == 1;
		enabled = false;
		name = "Futile";
		Go.defaultEaseType = (GoEaseType)0;
		Go.duplicatePropertyRule = (GoDuplicatePropertyRuleType)1;
	}

	public void Init(FutileParams futileParams)
	{
		enabled = true;
		mFutileParams = futileParams;
		StartingQuadsPerLayer = mFutileParams.StartingQuadsPerLayer;
		QuadsPerLayerExpansion = mFutileParams.QuadsPerLayerExpansion;
		MaxEmptyQuadsPerLayer = mFutileParams.MaxEmptyQuadsPerLayer;
		Screen = new FScreen(mFutileParams);

		GameObject cameras = new GameObject("zzzzCameras");
		cameras.transform.parent = gameObject.transform;
		GameObject cameraObject = new GameObject();
		cameraObject.transform.parent = cameras.transform;
		cameraObject.layer = gameObject.layer;
		mCamera = cameraObject.AddComponent<Camera>();
		mCamera.name = "Camera" + 0.ToString();
		mCamera.clearFlags = CameraClearFlags.Depth;
		mCamera.nearClipPlane = -150f;
		mCamera.farClipPlane = 5000f;
		mCamera.depth = 90f;
		mCamera.rect = new Rect(0f, 0f, 1f, 1f);
		mCamera.backgroundColor = mFutileParams.BackgroundColor;
		mCamera.cullingMask = 1 << cActiveCameraLayer;
		mCamera.allowHDR = false;
		mCamera.allowMSAA = false;
		mCamera.orthographic = true;
		mCamera.orthographicSize = Screen.PixelHeight * DisplayScaleInverse * 0.5f;

		GameObject helper = cameraObject.Clone(Vector3.zero);
		helper.name = "FutileHelperCam";
		helper.AddComponent<FutileRender>();
		helper.transform.parent = cameras.transform;
		mHelperCamera = helper.GetComponent<Camera>();
		mHelperCamera.cullingMask = 0;
		mHelperCamera.rect = new Rect(0.0001f, 0f, 1f, 1f);
		EnableCameraClearing(false);
		helper.AddComponent<AudioListener>();
		cameraObject.AddComponent<FScissorRect>();
		cameraObject.SetActive(false);
		UpdateCameraPosition();

		TouchManager = new FTouchManager();
		AtlasManager = new FAtlasManager();
		Shader.SetGlobalFloat("_SEPIA_GLOBAL", 0f);
	}

	public static void AddStage(FStage stageToAdd)
	{
		int index = mStages.IndexOf(stageToAdd);
		if (index == -1)
		{
			stageToAdd.HandleAddedToFutile();
		}
		else
		{
			if (index == mStages.Count - 1)
			{
				return;
			}
			mStages.RemoveAt(index);
		}
		mStages.Add(stageToAdd);
		UpdateStageIndices();
	}

	public static void AddStageAtIndex(FStage stageToAdd, int newIndex)
	{
		int index = mStages.IndexOf(stageToAdd);
		if (newIndex > mStages.Count)
		{
			newIndex = mStages.Count;
		}
		if (index == newIndex)
		{
			return;
		}
		if (index == -1)
		{
			stageToAdd.HandleAddedToFutile();
		}
		else
		{
			mStages.RemoveAt(index);
			if (index < newIndex)
			{
				newIndex--;
			}
		}
		mStages.Insert(newIndex, stageToAdd);
		UpdateStageIndices();
	}

	public static void RemoveStage(FStage stageToRemove)
	{
		if (stageToRemove == null)
		{
			return;
		}
		stageToRemove.HandleRemovedFromFutile();
		stageToRemove.Index = -1;
		mStages.Remove(stageToRemove);
		UpdateStageIndices();
	}

	public static void UpdateStageIndices()
	{
		int count = mStages.Count;
		for (int i = 0; i < count; i++)
		{
			mStages[i].Index = i;
		}
		mDepthChangeNeeded = true;
	}

	public static int GetStageCount()
	{
		return mStages.Count;
	}

	public static FStage GetStageAt(int index)
	{
		return mStages[index];
	}

	public static FStage GetStage(string name)
	{
		for (int i = 0; i < mStages.Count; i++)
		{
			if (mStages[i].Name == name)
			{
				return mStages[i];
			}
		}
		return null;
	}

	public static void HandleUpdateTextures()
	{
		for (int i = 0; i < mStages.Count; i++)
		{
			mStages[i].mNeedsTextureUpdate = true;
		}
	}

	protected void Update()
	{
		UpdateGCCollect();
		if (this.mSignalKeyInput != null || mSignalKeyInputList.Count > 0)
		{
			StringBuilder keys = new StringBuilder();
			string input = Input.inputString;
			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];
				// Skip the private-use / function key range Unity reports (0xF700..0xF8FF).
				if (((c + 0x2000) >> 8 & 0xFF) > 0x18)
				{
					keys.Append(c);
				}
			}
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				keys.Append(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? (char)11 : (char)9);
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				keys.Append((char)27);
			}
			if (keys.Length > 0)
			{
				string text = keys.ToString();
				if (UseSignalLists)
				{
					for (int i = 0; i < mSignalKeyInputList.Count; i++)
					{
						mSignalKeyInputList[i]?.Invoke(text);
					}
				}
				else
				{
					this.mSignalKeyInput?.Invoke(text);
				}
			}
		}
		Screen.Update();
		TouchManager.Update();
		if (UseSignalLists)
		{
			for (int i = 0; i < mSignalUpdateList.Count; i++)
			{
				mSignalUpdateList[i]?.Invoke();
			}
		}
		else
		{
			this.mSignalUpdate?.Invoke();
		}
	}

	public static void UpdateGCCollect()
	{
		if (mCollectCount > 0 && Time.time > mNextCollectTime)
		{
			mCollectCount--;
			mNextCollectTime = Time.time + 1f;
			GC.Collect();
		}
	}

	public void Render()
	{
		if (mLastFrameUpdate == Time.frameCount)
		{
			return;
		}
		mLastFrameUpdate = Time.frameCount;
		mIsFutileRenderingOuter = true;
		GOZValue = mCamera.farClipPlane * 0.9f;
		mIsFutileRenderingDraw = true;
		mFirstCameraGotten = false;
		for (int i = 0; i < mStages.Count; i++)
		{
			mCurrentRenderStage = mStages[i];
			mCurrentRenderStage.Redraw(false, mDepthChangeNeeded);
		}
		mIsFutileRenderingDraw = false;
		RenderPendingCamera();
		mDepthChangeNeeded = false;
		for (int i = 0; i < mStages.Count; i++)
		{
			mStages[i].Renderer.Update();
		}
		if (UseSignalLists)
		{
			for (int i = 0; i < mSignalLateUpdateList.Count; i++)
			{
				mSignalLateUpdateList[i]?.Invoke();
			}
		}
		else
		{
			this.mSignalLateUpdate?.Invoke();
		}
		if (LastDFGlobalCounter != FFont.DF_GlobalCounter)
		{
			FFont.DF_GlobalCounter++;
			LastDFGlobalCounter = FFont.DF_GlobalCounter;
		}
		for (int i = 0; i < mStages.Count; i++)
		{
			mStages[i].Renderer.CleanUnRenderedLayers();
		}
		List<FAtlas> unloaded = AtlasManager.UnloadUnusedAtlases();
		if (unloaded != null)
		{
			for (int i = 0; i < mStages.Count; i++)
			{
				mStages[i].Renderer.UnlinkAtlases(unloaded);
			}
			if (FConfigCore.GCFix1)
			{
				QueueGCCollect(false, 10f);
			}
			else
			{
				GC.Collect();
			}
		}
		mIsFutileRenderingOuter = false;
	}

	protected static void RenderPendingCamera()
	{
		int frame = Time.frameCount;
		for (int i = 0; i < mCurrentCameraRenderLayers.Count; i++)
		{
			FRenderLayer layer = mCurrentCameraRenderLayers[i];
			layer.SetCameraLayer(cActiveCameraLayer);
			if (!layer.IsActive())
			{
				layer.AddToWorld();
			}
			layer.UpdateLayerMeshIfNeeded();
			layer.LastFrameRendered = frame;
		}
		AtlasManager.Flush();
		mCamera.Render();
		for (int i = 0; i < mCurrentCameraRenderLayers.Count; i++)
		{
			mCurrentCameraRenderLayers[i].SetCameraLayer(cInactiveCameraLayer);
		}
		mCurrentCameraRenderLayers.Clear();
	}

	public static void QueueGCCollect(bool doubleCollect, float delay = 1f)
	{
		mCollectCount = doubleCollect ? Math.Max(mCollectCount, 2) : Math.Max(mCollectCount, 1);
		mNextCollectTime = Time.time + delay;
	}

	public static void AddCurrentCameraRenderLayer(FRenderLayer layer)
	{
		int oldDepth = layer.depth;
		int newDepth = 0;
		if (mCurrentCameraRenderLayers.Count < 1)
		{
			mCurrentCameraRenderLayers.Add(layer);
		}
		else
		{
			FRenderLayer last = mCurrentCameraRenderLayers[mCurrentCameraRenderLayers.Count - 1];
			newDepth = last.depth;
			int nextDepth = last.depth + last.NumDepthsUsed;
			if (last != layer)
			{
				mCurrentCameraRenderLayers.Add(layer);
				last.VirtualCloseFrame = Time.frameCount;
				newDepth = nextDepth;
			}
			if (nextDepth <= oldDepth && !FStage.DoFullDepthUpdate)
			{
				newDepth = oldDepth;
			}
		}
		if (oldDepth != newDepth)
		{
			layer.depth = newDepth;
		}
		layer.SortingDirty = false;
		layer.UpdateTextureIfNeeded();
		FRenderer.VirtualBatchIndex = layer.BatchIndex;
	}

	public static void RemoveCurrentCameraRenderLayer(FRenderLayer layer)
	{
		int index = mCurrentCameraRenderLayers.IndexOf(layer);
		if (index >= 0 && index <= mCurrentCameraRenderLayers.Count)
		{
			mCurrentCameraRenderLayers.RemoveAt(index);
		}
	}

	protected void OnApplicationQuit()
	{
	}

	protected void OnDestroy()
	{
		AtlasManager?.OnDestroy();
		Instance = null;
	}

	public void UpdateCameraPosition()
	{
		if (Screen == null || mCamera == null)
		{
			return;
		}
		float pixelWidth = Screen.PixelWidth;
		float pixelHeight = Screen.PixelHeight;
		mCamera.orthographicSize = pixelHeight * 0.5f * DisplayScaleInverse;
		float x = (Screen.OriginX - 0.5f) * -pixelWidth * DisplayScaleInverse;
		float y = (Screen.OriginY - 0.5f) * -pixelHeight * DisplayScaleInverse;
		mCamera.transform.position = new Vector3(x, y, -10f);
	}

	public static bool GetTopClippingRect(out Rect clippingRect)
	{
		clippingRect = new Rect(0f, 0f, Screen.Width, Screen.Height);
		if (mClippingStack.Count < 1)
		{
			return false;
		}
		clippingRect = mClippingStack[mClippingStack.Count - 1];
		return true;
	}

	public static void StartRedraw(FNode node)
	{
		if (mClippingStack.Count == 0 && mJustClearedClipStack)
		{
			GetNextCamera(0f, 0f, Screen.Width, Screen.Height);
			mJustClearedClipStack = false;
		}
		else if (mSepiaUnderNode == node)
		{
			GetNextCamera(0f, 0f, Screen.Width, Screen.Height);
		}
		if (mSepiaUnderNode == node)
		{
			Shader.SetGlobalFloat("_SEPIA_GLOBAL", 0f);
		}
	}

	public static void ForceRenderActiveNodes()
	{
		mIsForceRendering = true;
		if (mIsFutileRenderingDraw)
		{
			RenderPendingCamera();
		}
		mIsForceRendering = false;
	}

	public static string GetRenderStateString()
	{
		return string.Format("mIsForceRendering:{0} mIsFutileRenderingOuter:{1} mIsFutileRenderingDraw:{2}", mIsForceRendering, mIsFutileRenderingOuter, mIsFutileRenderingDraw);
	}

	public static void StartRedraw(FNode node, float clipMinX, float clipMinY, float clipMaxX, float clipMaxY)
	{
		mClippingStack.Add(new Rect(clipMinX, clipMinY, clipMaxX - clipMinX, clipMaxY - clipMinY));
		GetNextCamera(clipMinX, clipMinY, clipMaxX, clipMaxY);
	}

	public static void StopRedraw(bool popClipStack)
	{
		if (!popClipStack)
		{
			return;
		}
		mClippingStack.RemoveAt(mClippingStack.Count - 1);
		if (mClippingStack.Count == 0)
		{
			mJustClearedClipStack = true;
		}
	}

	public static Camera GetNextCamera(float clipMinX, float clipMinY, float clipMaxX, float clipMaxY)
	{
		if (!mFirstCameraGotten)
		{
			mFirstCameraGotten = true;
		}
		mCurrentRenderStage.Renderer.CloseCurrentLayer();
		RenderPendingCamera();
		FScissorRect scissor = mCamera.GetComponent<FScissorRect>();
		scissor.scissorRect = new Rect(clipMinX / Screen.Width, clipMinY / Screen.Height,
			(clipMaxX - clipMinX) / Screen.Width, (clipMaxY - clipMinY) / Screen.Height);
		return mCamera;
	}

	public static void EnableCameraClearing(bool enable)
	{
		// The shipped build ignores the flag and always clears depth only.
		if (mHelperCamera == null)
		{
			return;
		}
		mHelperCamera.clearFlags = CameraClearFlags.Depth;
	}

	public static Vector2 GlobalTouchToScreen(FTouch touch)
	{
		return GlobalPosToScreen(touch.Position);
	}

	public static Vector2 GlobalPosToScreen(Vector2 globalPos)
	{
		return new Vector2(globalPos.x * DisplayScale + Screen.PixelWidth * Screen.OriginX, globalPos.y * DisplayScale + Screen.PixelHeight * Screen.OriginY);
	}

	public static Vector2 GlobalPosToScreen(FTouch touch)
	{
		return GlobalPosToScreen(touch.Position);
	}

	public static Vector2 GetMousePosition()
	{
		Vector2 mouse = FUtil.mousePosition;
		float inverse = 1f / DisplayScale;
		return new Vector2((mouse.x - Screen.PixelWidth * Screen.OriginX) * inverse, (mouse.y - Screen.PixelHeight * Screen.OriginY) * inverse);
	}

	public static Vector3 WorldPositionToScreen(Camera camera, Vector3 position)
	{
		if (camera == null)
		{
			return Vector3.zero;
		}
		return ScreenPositionToUI(camera.WorldToScreenPoint(position));
	}

	public static Vector3 UIPositionToScreen(Vector3 position)
	{
		return new Vector3(position.x * DisplayScale + Screen.PixelWidth * 0.5f, position.y * DisplayScale + Screen.PixelHeight * 0.5f, 0f);
	}

	public static Vector3 ScreenPositionToUI(Vector3 position)
	{
		return new Vector3((position.x - Screen.PixelWidth * 0.5f) * DisplayScaleInverse, (position.y - Screen.PixelHeight * 0.5f) * DisplayScaleInverse, position.z * DisplayScaleInverse);
	}

	public static Vector3 FNodePositionToWorldPosition(Camera worldCamera, FNode node, Vector2 nodeSpaceExtraOffset, float worldCameraDistance = 15f)
	{
		Vector2 global = node.LocalToGlobal(nodeSpaceExtraOffset);
		return ScreenToWorldPosition(worldCamera, global, worldCameraDistance);
	}

	public static Vector3 ScreenToWorldPosition(Camera camera, Vector3 position, float distance)
	{
		if (camera == null)
		{
			return Vector3.zero;
		}
		return camera.ScreenToWorldPoint(new Vector3(position.x * DisplayScale + Screen.PixelWidth * 0.5f, position.y * DisplayScale + Screen.PixelHeight * 0.5f, distance));
	}

	public bool AddFocusNode(FNode node)
	{
		if (node == null)
		{
			return false;
		}
		if (mFocusStack.Contains(node))
		{
			if (mFocusStack[mFocusStack.Count - 1] == node)
			{
				return true;
			}
			mFocusStack.Remove(node);
		}
		if (mFocusStack.Count > 0)
		{
			mFocusStack[mFocusStack.Count - 1].HandleLoseFocus();
		}
		mFocusStack.Add(node);
		node.HandleGainFocus();
		return true;
	}

	public bool RemoveFocusNode(FNode node)
	{
		if (node == null || !mFocusStack.Contains(node))
		{
			return false;
		}
		if (mFocusStack[mFocusStack.Count - 1] != node)
		{
			mFocusStack.Remove(node);
			return true;
		}
		node.HandleLoseFocus();
		mFocusStack.Remove(node);
		if (mFocusStack.Count > 0)
		{
			mFocusStack[mFocusStack.Count - 1].HandleGainFocus();
		}
		return true;
	}

	public static void DumpFutileFocusStack()
	{
		// TODO(port): debug dump (0x16ca004). [left: debug dump text]
	}

	public static void DumpFutileUpdateDelegates()
	{
		// TODO(port): debug dump (0x16ca1f4). [left: debug dump text]
	}

	public void AddSignalUpdate(Action d)
	{
		if (UseSignalLists)
		{
			if (!mSignalUpdateList.Contains(d))
			{
				mSignalUpdateList.Add(d);
			}
			return;
		}
		mSignalUpdate -= d;
		mSignalUpdate += d;
	}

	public void RemoveSignalUpdate(Action d)
	{
		if (UseSignalLists)
		{
			mSignalUpdateList.Remove(d);
			return;
		}
		mSignalUpdate -= d;
	}

	public void AddSignalLateUpdate(Action d)
	{
		if (UseSignalLists)
		{
			if (!mSignalLateUpdateList.Contains(d))
			{
				mSignalLateUpdateList.Add(d);
			}
			return;
		}
		mSignalLateUpdate -= d;
		mSignalLateUpdate += d;
	}

	public void RemoveSignalLateUpdate(Action d)
	{
		if (UseSignalLists)
		{
			mSignalLateUpdateList.Remove(d);
			return;
		}
		mSignalLateUpdate -= d;
	}

	public void AddSignalKeyInput(Action<string> d)
	{
		if (UseSignalLists)
		{
			if (!mSignalKeyInputList.Contains(d))
			{
				mSignalKeyInputList.Add(d);
			}
			return;
		}
		mSignalKeyInput -= d;
		mSignalKeyInput += d;
	}

	public void RemoveSignalKeyInput(Action<string> d)
	{
		if (UseSignalLists)
		{
			mSignalKeyInputList.Remove(d);
			return;
		}
		mSignalKeyInput -= d;
	}

	public void OnGUI()
	{
		TouchManager?.ProcessEvent(Event.current);
	}

	public static void AddSepiaCameraHelper(FUISepiaCameraHelper helper)
	{
		mSepiaCameraHelpers.Add(helper);
	}

	public static void RemoveSepiaCameraHelper(FUISepiaCameraHelper helper)
	{
		mSepiaCameraHelpers.Remove(helper);
	}

	public void DestroyOldRenderLayers()
	{
		for (int i = 0; i < mStages.Count; i++)
		{
			mStages[i].Renderer.DestroyOldLayers();
		}
	}
}
