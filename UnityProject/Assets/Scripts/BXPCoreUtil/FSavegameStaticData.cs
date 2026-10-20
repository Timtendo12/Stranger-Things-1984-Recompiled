using System;
using System.Collections.Generic;

public static class FSavegameStaticData
{
	public const int cOverallSaveVersion = 0;

	private static HashSet<Type> mEnumsToSave;

	private static Dictionary<Type, int> mSaveVersionInfo;

	static FSavegameStaticData()
	{
	}

	public static void SaveVersions(FBinWriter writer)
	{
	}

	public static Dictionary<Type, int> LoadVersions(FBinReader reader)
	{
		return null;
	}

	public static void SaveEnumMap(FBinWriter writer)
	{
	}

	public static bool IsSaveEnum<T>()
	{
		return false;
	}

	public static Dictionary<Type, FSaveEnumInfo> LoadEnumMap(FBinReader reader)
	{
		return null;
	}
}
