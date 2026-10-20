using System;

// Rewritten from libil2cpp.so 1.0.605 (FVersionInfo @ 0x17e0d7c..0x17e18cc, Ghidra vaddr).
public class FVersionInfo
{
	public static string VersionNumberString = "0.0.0";

	protected static int mBuildNumber = -1;

	protected internal static int mCoreBuildNumber = -1;

	protected static int mMajorVersionNumber = -1;

	protected static int mMinorVersionNumber = -1;

	public static string Branch = "unset";

	protected static string mVersionString;

	public static string BuildVariant = "worldwide";

	// TODO(port): original value is the placeholder literal below (unused on PC). [left: placeholder, unused on PC]
	public static string GooglePlayPublicKey = "TODO GET KEY";

	public static string FacebookAppID = string.Empty;

	public static string TwitterAppID = string.Empty;

	public static int BuildNumber
	{
		get
		{
			return mBuildNumber;
		}
		set
		{
			mBuildNumber = value;
		}
	}

	public static int CoreBuildNumber
	{
		get
		{
			return mCoreBuildNumber;
		}
		set
		{
			mCoreBuildNumber = value;
		}
	}

	public static int MajorVersionNumber
	{
		get
		{
			return mMajorVersionNumber;
		}
		set
		{
			mMajorVersionNumber = value;
		}
	}

	public static int MinorVersionNumber
	{
		get
		{
			return mMinorVersionNumber;
		}
		set
		{
			mMinorVersionNumber = value;
		}
	}

	public static string CompoundCoreBuildNumber => mCoreBuildNumber + "-final";

	public static void Init()
	{
		LoadVersion();
		string text = "release BUILD_FINAL";
		mVersionString = string.Concat(VersionNumberString, " (", mCoreBuildNumber.ToString(), ") -- ", text);
		mVersionString = mVersionString + " (" + mBuildNumber + ")";
	}

	public static void LoadVersion()
	{
		string text = FUtil.LoadTextAsset("Data/BuildNumber");
		if (text == null)
		{
			return;
		}
		if (!int.TryParse(text.Trim(), out mBuildNumber))
		{
			mBuildNumber = -1;
			mCoreBuildNumber = -1;
		}
		else
		{
			mCoreBuildNumber = mBuildNumber % 1000000;
		}
		string text2 = FUtil.LoadTextAsset("Data/VersionNumber.txt");
		if (text2 != null)
		{
			text2 = text2.Trim();
			if (text2.Length > 0)
			{
				VersionNumberString = text2;
			}
		}
		mMajorVersionNumber = -1;
		mMinorVersionNumber = -1;
		if (!string.IsNullOrEmpty(VersionNumberString))
		{
			string[] array = VersionNumberString.Split('.');
			if (array.Length > 1 && int.TryParse(array[0], out mMajorVersionNumber))
			{
				int.TryParse(array[1], out mMinorVersionNumber);
			}
		}
		string text3 = FUtil.LoadTextAsset("Data/Branch.txt");
		if (text3 != null)
		{
			text3 = text3.Trim();
			if (text3.Length > 0)
			{
				Branch = text3;
			}
		}
	}

	public static string GetVersionString()
	{
		return mVersionString ?? "(unknown)";
	}
}
