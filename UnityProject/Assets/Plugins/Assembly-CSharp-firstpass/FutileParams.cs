using System.Collections.Generic;
using UnityEngine;

public class FutileParams
{
	public readonly ScreenOrientation DefaultSupportedOrientation;

	public List<FResolutionLevel> ResLevels = new List<FResolutionLevel>();

	public int StartingQuadsPerLayer = 10;

	public int QuadsPerLayerExpansion = 10;

	public int MaxEmptyQuadsPerLayer = 60;

	public Vector2 Origin = new Vector2(0.5f, 0.5f);

	public int TargetFrameRate = 60;

	public ScreenOrientation SingleOrientation;

	public bool SupportsLandscapeLeft;

	public bool SupportsLandscapeRight;

	public bool SupportsPortrait;

	public bool SupportsPortraitUpsideDown;

	public Color BackgroundColor = Color.black;

	public bool ShouldLerpToNearestResolutionLevel = true;

	public FutileParams(bool supportsLandscapeLeft, bool supportsLandscapeRight, bool supportsPortrait, bool supportsPortraitUpsideDown, ScreenOrientation defaultSupportedOrientation)
	{
		DefaultSupportedOrientation = defaultSupportedOrientation;
		SupportsLandscapeLeft = supportsLandscapeLeft;
		SupportsLandscapeRight = supportsLandscapeRight;
		SupportsPortrait = supportsPortrait;
		SupportsPortraitUpsideDown = supportsPortraitUpsideDown;
		FAssert.That((defaultSupportedOrientation == ScreenOrientation.Portrait && supportsPortrait) || (defaultSupportedOrientation == ScreenOrientation.LandscapeLeft && supportsLandscapeLeft) || (defaultSupportedOrientation == ScreenOrientation.LandscapeRight && supportsLandscapeRight) || (defaultSupportedOrientation == ScreenOrientation.PortraitUpsideDown && supportsPortraitUpsideDown), "defaultSupportedOrientation not supported!  Bad programmer!");
	}

	public FResolutionLevel AddResolutionLevel(float maxLength, float displayScale, float resourceScale, string resourceSuffix)
	{
		return AddResolutionLevel(ResLevels, maxLength, displayScale, resourceScale, resourceSuffix);
	}

	public FResolutionLevel AddResolutionLevel(List<FResolutionLevel> levels, float maxLength, float displayScale, float resourceScale, string resourceSuffix)
	{
		FResolutionLevel level = new FResolutionLevel
		{
			MaxLength = maxLength,
			DisplayScale = displayScale,
			ResourceScale = resourceScale,
			ResourceSuffix = resourceSuffix
		};
		for (int i = 0; i < levels.Count; i++)
		{
			if (level.MaxLength < levels[i].MaxLength)
			{
				levels.Insert(i, level);
				return level;
			}
		}
		levels.Add(level);
		return level;
	}

	public FResolutionLevel GetResolutionLevel(float length)
	{
		return GetResolutionLevel(ResLevels, length);
	}

	public FResolutionLevel GetResolutionLevel(List<FResolutionLevel> levels, float length)
	{
		foreach (FResolutionLevel level in levels)
		{
			if (length <= level.MaxLength)
			{
				return level;
			}
		}
		FResolutionLevel last = levels.Count > 0 ? levels[levels.Count - 1] : null;
		FAssert.That(last != null, "You must add at least one FResolutionLevel");
		return last;
	}
}
