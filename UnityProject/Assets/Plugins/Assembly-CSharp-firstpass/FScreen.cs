using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FScreen @ 0x1696574..0x1697824, Ghidra vaddr).
public class FScreen
{
	private static float cWidescreenRatio = 1.6f;

	private const float cTargetAspect = 16f / 9f;

	public float Width;

	public float Height;

	public float HalfWidth;

	public float HalfHeight;

	public float PixelWidth;

	public float PixelHeight;

	private float mOriginX;

	private float mOriginY;

	private ScreenOrientation mCurrentOrientation;

	private ScreenOrientation mLastSeenOrientation;

	private float mScreenLongLength;

	private float mScreenShortLength;

	private bool mDidJustResize;

	private float mOldWidth;

	private float mOldHeight;

	private FResolutionLevel mResLevel;

	private FutileParams mFutileParams;

	public float OriginX
	{
		get
		{
			return mOriginX;
		}
		set
		{
			if (mOriginX != value)
			{
				mOriginX = value;
				Futile.Instance.UpdateCameraPosition();
			}
		}
	}

	public float OriginY
	{
		get
		{
			return mOriginY;
		}
		set
		{
			if (mOriginY != value)
			{
				mOriginY = value;
				Futile.Instance.UpdateCameraPosition();
			}
		}
	}

	public ScreenOrientation CurrentOrientation => mCurrentOrientation;

	public bool IsLandscape => mCurrentOrientation == ScreenOrientation.LandscapeLeft || mCurrentOrientation == ScreenOrientation.LandscapeRight;

	public bool IsPortrait => !IsLandscape;

	public bool IsWidescreen
	{
		get
		{
			if (mScreenShortLength <= 0f)
			{
				return false;
			}
			float ratio = mScreenLongLength / mScreenShortLength;
			return ratio > cWidescreenRatio || Mathf.Approximately(cWidescreenRatio, ratio);
		}
	}

	public int IntWidth => (int)Width;

	public int IntHeight => (int)Height;

	public Vector2 Size => new Vector2(Width, Height);

	protected event Action mSignalOrientationChange;

	protected event Action mSignalResize;

	public ScreenOrientation GetScreenOrientationFromDevice(bool guessFromResolution)
	{
		DeviceOrientation device = Input.deviceOrientation;
		if (device == DeviceOrientation.LandscapeLeft)
		{
			return ScreenOrientation.LandscapeLeft;
		}
		if (device == DeviceOrientation.LandscapeRight)
		{
			return ScreenOrientation.LandscapeRight;
		}
		if (device == DeviceOrientation.Portrait)
		{
			return ScreenOrientation.Portrait;
		}
		if (device == DeviceOrientation.PortraitUpsideDown)
		{
			return ScreenOrientation.PortraitUpsideDown;
		}
		if (!guessFromResolution)
		{
			return ScreenOrientation.Unknown;
		}
		ScreenOrientation current = Screen.orientation;
		if (PCDisplay.Width < PCDisplay.Height)
		{
			return current == ScreenOrientation.PortraitUpsideDown ? ScreenOrientation.PortraitUpsideDown : ScreenOrientation.Portrait;
		}
		return current == ScreenOrientation.LandscapeRight ? ScreenOrientation.LandscapeRight : ScreenOrientation.LandscapeLeft;
	}

	public FScreen(FutileParams futileParams)
	{
		mFutileParams = futileParams;
		ScreenOrientation orientation = GetScreenOrientationFromDevice(true);
		if (!IsOrientationSupported(orientation))
		{
			orientation = mFutileParams.DefaultSupportedOrientation;
		}
		mCurrentOrientation = orientation;
		mLastSeenOrientation = orientation;
		CalculatePixelSizes();
		CalculateResolutionLevel();
		CalculateDisplayScale();
		mOriginX = mFutileParams.Origin.x;
		mOriginY = mFutileParams.Origin.y;
		FLog.UnityLog("Futile: Display scale is " + Futile.DisplayScale.ToString());
		FLog.UnityLog("Futile: Resource scale is " + Futile.ResourceScale.ToString());
		FLog.UnityLog("Futile: Resource suffix is " + mResLevel.ResourceSuffix);
		FLog.UnityLog("FScreen: Screen size in pixels is (" + PixelWidth.ToString() + "px," + PixelHeight.ToString() + "px)");
		FLog.UnityLog("FScreen: Screen size in points is (" + Width.ToString() + "," + Height.ToString() + ")");
		FLog.UnityLog("FScreen: Origin is at (" + mOriginX.ToString() + "," + mOriginY.ToString() + "), Origin is at (" + (mOriginX * Width).ToString() + "," + (mOriginY * Height).ToString() + ")");
		FLog.UnityLog("FScreen: Initial orientation is " + mCurrentOrientation.ToString());
		mDidJustResize = true;
	}

	public void Update()
	{
		ScreenOrientation orientation = GetScreenOrientationFromDevice(true);
		if (mLastSeenOrientation != orientation)
		{
			SwitchOrientation(orientation);
		}
		if (mDidJustResize)
		{
			mDidJustResize = false;
			mOldWidth = PCDisplay.Width;
			mOldHeight = PCDisplay.Height;
			return;
		}
		if (mOldWidth != PCDisplay.Width || mOldHeight != PCDisplay.Height)
		{
			mOldWidth = PCDisplay.Width;
			mOldHeight = PCDisplay.Height;
			// PC port: the shipped game forced windows to 16:9 via SetResolution; PCDisplay letterboxes instead.
			UpdateScreenDimensions();
			this.mSignalResize?.Invoke();
		}
	}

	private bool IsOrientationSupported(ScreenOrientation orientation)
	{
		switch (orientation)
		{
		case ScreenOrientation.Portrait:
			return mFutileParams.SupportsPortrait;
		case ScreenOrientation.PortraitUpsideDown:
			return mFutileParams.SupportsPortraitUpsideDown;
		case ScreenOrientation.LandscapeLeft:
			return mFutileParams.SupportsLandscapeLeft;
		case ScreenOrientation.LandscapeRight:
			return mFutileParams.SupportsLandscapeRight;
		default:
			return false;
		}
	}

	public void SwitchOrientation(ScreenOrientation newOrientation)
	{
		if (!IsOrientationSupported(newOrientation))
		{
			return;
		}
		FLog.UnityLog("FScreen::SwitchOrientation: Current=" + mCurrentOrientation.ToString() + ", newOrientation=" + newOrientation.ToString());
		mCurrentOrientation = newOrientation;
		mLastSeenOrientation = newOrientation;
		UpdateScreenDimensions();
		this.mSignalOrientationChange?.Invoke();
		this.mSignalResize?.Invoke();
		mDidJustResize = true;
	}

	private void UpdateScreenDimensions()
	{
		CalculatePixelSizes();
		CalculateResolutionLevel();
		CalculateDisplayScale();
		Futile.Instance.UpdateCameraPosition();
	}

	protected void CalculatePixelSizes()
	{
		mScreenLongLength = Math.Max(PCDisplay.Height, PCDisplay.Width);
		mScreenShortLength = Math.Min(PCDisplay.Height, PCDisplay.Width);
		if (IsPortrait)
		{
			PixelWidth = mScreenShortLength;
			PixelHeight = mScreenLongLength;
		}
		else
		{
			PixelWidth = mScreenLongLength;
			PixelHeight = mScreenShortLength;
		}
	}

	protected void CalculateDisplayScale()
	{
		float scale;
		if (!mFutileParams.ShouldLerpToNearestResolutionLevel)
		{
			scale = 1f;
		}
		else if (PixelWidth / PixelHeight >= cTargetAspect)
		{
			scale = PixelHeight / mResLevel.MaxLength;
		}
		else
		{
			scale = PixelWidth / cTargetAspect / mResLevel.MaxLength;
		}
		Futile.DisplayScale = scale * mResLevel.DisplayScale;
		Futile.DisplayScaleInverse = 1f / Futile.DisplayScale;
		Width = PixelWidth * Futile.DisplayScaleInverse;
		Height = PixelHeight * Futile.DisplayScaleInverse;
		HalfWidth = Width * 0.5f;
		HalfHeight = Height * 0.5f;
	}

	protected void CalculateResolutionLevel()
	{
		mResLevel = mFutileParams.GetResolutionLevel(mScreenShortLength);
		Futile.ResourceSuffix = mResLevel.ResourceSuffix;
		Futile.ResourceScale = mResLevel.ResourceScale;
		Futile.ResourceScaleInverse = 1f / Futile.ResourceScale;
	}

	public void AddSignalOrientationChange(Action d)
	{
		mSignalOrientationChange -= d;
		mSignalOrientationChange += d;
	}

	public void RemoveSignalOrientationChange(Action d)
	{
		mSignalOrientationChange -= d;
	}

	public void AddSignalResize(Action d)
	{
		mSignalResize -= d;
		mSignalResize += d;
	}

	public void RemoveSignalResize(Action d)
	{
		mSignalResize -= d;
	}

	public void ForceResize()
	{
		this.mSignalResize?.Invoke();
	}

	public bool WithinScreen(float x, float y, float errorFactor)
	{
		if (x < -(Width * errorFactor) || x > Width + Width * errorFactor)
		{
			return false;
		}
		return y >= -(Height * errorFactor) && y <= Height + Height * errorFactor;
	}
}
