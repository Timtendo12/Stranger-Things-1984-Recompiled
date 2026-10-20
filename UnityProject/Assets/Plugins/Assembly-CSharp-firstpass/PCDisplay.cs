using System;
using UnityEngine;

// PC port (new): window / fullscreen handling and 16:9 letterboxing.
// When the window (or fullscreen desktop) is not 16:9, every game camera is redirected into a 16:9 RenderTexture
// that has exactly the size of the letterboxed area, so it is blitted 1:1 (pixel crisp) and the rest stays black.
// The game sees the virtual screen through PCDisplay.Width/Height/MousePosition instead of UnityEngine.Screen.
[DefaultExecutionOrder(-10000)]
public class PCDisplay : MonoBehaviour
{
	public const float TargetAspect = 16f / 9f;

	private const string PrefFullscreen = "PC.Fullscreen";

	private const string PrefWindowWidth = "PC.WindowWidth";

	private const string PrefWindowHeight = "PC.WindowHeight";

	private static PCDisplay sInstance;

	private static int sWidth;

	private static int sHeight;

	private static int sOffsetX;

	private static int sOffsetY;

	private static bool sLetterbox;

	private static bool sComputed;

	private static int sLastW;

	private static int sLastH;

	private RenderTexture mTarget;

	private Camera mBackdrop;

	private readonly Camera[] mCameras = new Camera[32];

	private int mWindowedW;

	private int mWindowedH;

	private int mSavedW;

	private int mSavedH;

	private float mSizeStableSince;

	private int mLastScreenW;

	private int mLastScreenH;

	private float mIgnoreSizeUntil;

	private bool mFullscreen;

	public static bool Letterboxed
	{
		get
		{
			Compute();
			return sLetterbox;
		}
	}

	public static int Width
	{
		get
		{
			Compute();
			return sWidth;
		}
	}

	public static int Height
	{
		get
		{
			Compute();
			return sHeight;
		}
	}

	// Mouse position in virtual-screen pixels (origin bottom-left of the 16:9 area).
	public static Vector3 MousePosition
	{
		get
		{
			Vector3 p = Input.mousePosition;
			Compute();
			if (sLetterbox)
			{
				p.x -= sOffsetX;
				p.y -= sOffsetY;
			}
			return p;
		}
	}

	public static bool MouseInsideView
	{
		get
		{
			Vector3 p = MousePosition;
			return p.x >= 0f && p.y >= 0f && p.x < sWidth && p.y < sHeight;
		}
	}

	// Offset of the virtual screen inside the real window, in real pixels (bottom-left origin).
	public static Vector2 Offset
	{
		get
		{
			Compute();
			return new Vector2(sOffsetX, sOffsetY);
		}
	}

	private static void Compute()
	{
		int sw = Screen.width;
		int sh = Screen.height;
		if (sComputed && sw == sLastW && sh == sLastH)
		{
			return;
		}
		sComputed = true;
		sLastW = sw;
		sLastH = sh;
		if (sw <= 0 || sh <= 0)
		{
			sWidth = Math.Max(1, sw);
			sHeight = Math.Max(1, sh);
			sOffsetX = sOffsetY = 0;
			sLetterbox = false;
			return;
		}
		float aspect = (float)sw / sh;
		if (Mathf.Abs(aspect - TargetAspect) < 0.012f)
		{
			sWidth = sw;
			sHeight = sh;
			sOffsetX = sOffsetY = 0;
			sLetterbox = false;
		}
		else if (aspect > TargetAspect)
		{
			sHeight = sh & ~1;
			sWidth = (int)Math.Round(sHeight * TargetAspect) & ~1;
			sOffsetX = (sw - sWidth) / 2;
			sOffsetY = (sh - sHeight) / 2;
			sLetterbox = true;
		}
		else
		{
			sWidth = sw & ~1;
			sHeight = (int)Math.Round(sWidth / TargetAspect) & ~1;
			sOffsetX = (sw - sWidth) / 2;
			sOffsetY = (sh - sHeight) / 2;
			sLetterbox = true;
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Bootstrap()
	{
		if (!Application.isPlaying || sInstance != null)
		{
			return;
		}
		GameObject go = new GameObject("PCDisplay");
		DontDestroyOnLoad(go);
		sInstance = go.AddComponent<PCDisplay>();
	}

	public static void ToggleFullscreen()
	{
		if (sInstance != null)
		{
			sInstance.DoToggleFullscreen();
		}
	}

	// Apply a window / fullscreen mode requested by the game options (FApp.UpdateVideoMode).
	public static void SetMode(int width, int height, bool fullscreen)
	{
		if (sInstance == null)
		{
			Screen.SetResolution(width, height, fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
			return;
		}
		if (fullscreen)
		{
			if (!sInstance.mFullscreen)
			{
				sInstance.mWindowedW = Screen.width;
				sInstance.mWindowedH = Screen.height;
				sInstance.EnterFullscreen();
			}
		}
		else
		{
			sInstance.mFullscreen = false;
			sInstance.mWindowedW = Mathf.Max(640, width);
			sInstance.mWindowedH = Mathf.Max(360, height);
			Screen.SetResolution(sInstance.mWindowedW, sInstance.mWindowedH, FullScreenMode.Windowed);
			sInstance.mIgnoreSizeUntil = Time.realtimeSinceStartup + 1f;
		}
		sInstance.SavePrefs();
	}

	private static bool HasCommandLineArg(string name)
	{
		string[] args = Environment.GetCommandLineArgs();
		for (int i = 0; i < args.Length; i++)
		{
			if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private void Awake()
	{
		mWindowedW = Mathf.Max(640, UnityEngine.PlayerPrefs.GetInt(PrefWindowWidth, 1280));
		mWindowedH = Mathf.Max(360, UnityEngine.PlayerPrefs.GetInt(PrefWindowHeight, 720));
		mSavedW = mWindowedW;
		mSavedH = mWindowedH;
		mFullscreen = Screen.fullScreen;
		bool cmdline = HasCommandLineArg("-screen-width") || HasCommandLineArg("-screen-fullscreen");
		if (!cmdline && !Application.isEditor)
		{
			if (UnityEngine.PlayerPrefs.GetInt(PrefFullscreen, 0) == 1)
			{
				EnterFullscreen();
			}
			else
			{
				Screen.SetResolution(mWindowedW, mWindowedH, FullScreenMode.Windowed);
				mIgnoreSizeUntil = Time.realtimeSinceStartup + 1f;
			}
		}
		else
		{
			mWindowedW = Screen.width;
			mWindowedH = Screen.height;
		}
		Camera.onPreCull += OnCameraPreCull;
	}

	private void OnDestroy()
	{
		Camera.onPreCull -= OnCameraPreCull;
		ReleaseTarget();
	}

	private void OnApplicationQuit()
	{
		SavePrefs();
	}

	private void EnterFullscreen()
	{
		DisplayInfo info = Screen.mainWindowDisplayInfo;
		int w = info.width > 0 ? info.width : Screen.currentResolution.width;
		int h = info.height > 0 ? info.height : Screen.currentResolution.height;
		mFullscreen = true;
		Screen.SetResolution(w, h, FullScreenMode.FullScreenWindow);
		mIgnoreSizeUntil = Time.realtimeSinceStartup + 1f;
	}

	private void DoToggleFullscreen()
	{
		if (mFullscreen)
		{
			mFullscreen = false;
			Screen.SetResolution(Mathf.Max(640, mWindowedW), Mathf.Max(360, mWindowedH), FullScreenMode.Windowed);
		}
		else
		{
			mWindowedW = Screen.width;
			mWindowedH = Screen.height;
			EnterFullscreen();
		}
		mIgnoreSizeUntil = Time.realtimeSinceStartup + 1f;
		SavePrefs();
	}

	private void SavePrefs()
	{
		try
		{
			UnityEngine.PlayerPrefs.SetInt(PrefFullscreen, mFullscreen ? 1 : 0);
			UnityEngine.PlayerPrefs.SetInt(PrefWindowWidth, mWindowedW);
			UnityEngine.PlayerPrefs.SetInt(PrefWindowHeight, mWindowedH);
			UnityEngine.PlayerPrefs.Save();
			mSavedW = mWindowedW;
			mSavedH = mWindowedH;
		}
		catch (Exception)
		{
		}
	}

	private void Update()
	{
		bool altEnter = (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter));
		// macOS convention: Ctrl+Cmd+F
		bool macFullscreen = (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.F);
		if (Input.GetKeyDown(KeyCode.F11) || altEnter || macFullscreen)
		{
			DoToggleFullscreen();
		}
		Compute();
		// Remember the windowed size once the user stopped dragging the window edge.
		if (!mFullscreen && !Screen.fullScreen && Time.realtimeSinceStartup > mIgnoreSizeUntil)
		{
			int w = Screen.width;
			int h = Screen.height;
			if (w != mLastScreenW || h != mLastScreenH)
			{
				mLastScreenW = w;
				mLastScreenH = h;
				mSizeStableSince = Time.realtimeSinceStartup;
			}
			else if (Time.realtimeSinceStartup - mSizeStableSince > 1f && (w != mWindowedW || h != mWindowedH) && w >= 320 && h >= 180)
			{
				mWindowedW = w;
				mWindowedH = h;
				if (mSavedW != w || mSavedH != h)
				{
					SavePrefs();
				}
			}
		}
		UpdateTarget();
	}

	private void LateUpdate()
	{
		if (sLetterbox)
		{
			AssignAllCameras(null);
		}
	}

	private void ReleaseTarget()
	{
		if (mTarget != null)
		{
			mTarget.Release();
			Destroy(mTarget);
			mTarget = null;
		}
	}

	private void UpdateTarget()
	{
		if (!sLetterbox)
		{
			if (mTarget != null)
			{
				AssignAllCameras(null);
				ReleaseTarget();
			}
			if (mBackdrop != null)
			{
				mBackdrop.enabled = false;
			}
			return;
		}
		RenderTexture old = null;
		if (mTarget == null || mTarget.width != sWidth || mTarget.height != sHeight)
		{
			old = mTarget;
			mTarget = new RenderTexture(sWidth, sHeight, 24, RenderTextureFormat.Default);
			mTarget.filterMode = FilterMode.Point;
			mTarget.antiAliasing = 1;
			mTarget.useMipMap = false;
			mTarget.name = "PCDisplayTarget";
			mTarget.Create();
		}
		if (mBackdrop == null)
		{
			GameObject go = new GameObject("PCDisplayBackdrop");
			go.transform.SetParent(transform, false);
			mBackdrop = go.AddComponent<Camera>();
			mBackdrop.clearFlags = CameraClearFlags.SolidColor;
			mBackdrop.backgroundColor = Color.black;
			mBackdrop.cullingMask = 0;
			mBackdrop.depth = -1000f;
			mBackdrop.allowHDR = false;
			mBackdrop.allowMSAA = false;
		}
		mBackdrop.enabled = true;
		AssignAllCameras(old);
		if (old != null)
		{
			old.Release();
			Destroy(old);
		}
	}

	private void AssignAllCameras(RenderTexture retired)
	{
		int n = Camera.GetAllCameras(mCameras);
		for (int i = 0; i < n; i++)
		{
			AssignCamera(mCameras[i], retired);
			mCameras[i] = null;
		}
	}

	private void OnCameraPreCull(Camera cam)
	{
		AssignCamera(cam, null);
	}

	private void AssignCamera(Camera cam, RenderTexture retired)
	{
		if (cam == null || cam == mBackdrop || cam.cameraType != CameraType.Game)
		{
			return;
		}
		RenderTexture current = cam.targetTexture;
		if (sLetterbox && mTarget != null)
		{
			if (current == null || (retired != null && current == retired))
			{
				cam.targetTexture = mTarget;
			}
		}
		else if (current != null && current.name == "PCDisplayTarget")
		{
			cam.targetTexture = null;
		}
	}

	private void OnGUI()
	{
		if (!sLetterbox || mTarget == null || Event.current.type != EventType.Repaint)
		{
			return;
		}
		GUI.depth = 1000;
		GUI.DrawTexture(new Rect(sOffsetX, Screen.height - sOffsetY - sHeight, sWidth, sHeight), mTarget, ScaleMode.StretchToFill, false);
	}
}
