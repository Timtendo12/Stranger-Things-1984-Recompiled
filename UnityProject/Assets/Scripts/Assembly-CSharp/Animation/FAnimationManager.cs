using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAnimationManager @ 0x17e9914..0x17eabf4, Ghidra vaddr).
public static class FAnimationManager
{
	private static Dictionary<string, Dictionary<string, FAnimation>> mAnimations = new Dictionary<string, Dictionary<string, FAnimation>>();

	private static Dictionary<string, int> mUnitUsages = new Dictionary<string, int>();

	public static FAnimation GetOrCreateAnimation(string unitName, FAnimEntry animEntry)
	{
		Dictionary<string, FAnimation> orCreateUnitDict = GetOrCreateUnitDict(unitName);
		if (!orCreateUnitDict.ContainsKey(animEntry.Name))
		{
			Sprite[] array = LoadSheet(unitName, animEntry.SpriteSheet);
			List<Sprite> list = new List<Sprite>();
			if (array.Length != 0)
			{
				int num = array.Length - 1;
				if (animEntry.FrameEnd >= 0)
				{
					num = animEntry.FrameEnd;
				}
				for (int i = 0; i < animEntry.Loops; i++)
				{
					int num2 = ((animEntry.FrameStart > 0) ? animEntry.FrameStart : 0);
					if (!animEntry.Reversed)
					{
						for (int j = num2; j <= num; j++)
						{
							list.Add(array[j]);
						}
					}
					else
					{
						for (int num3 = num; num3 >= num2; num3--)
						{
							list.Add(array[num3]);
						}
					}
				}
				animEntry.ClampTagFrames(0, list.Count - 1);
			}
			FAnimation value = new FAnimation
			{
				Name = animEntry.Name,
				FPS = animEntry.FPS,
				Frames = list.ToArray(),
				AnimEntry = animEntry
			};
			orCreateUnitDict[animEntry.Name] = value;
		}
		if (!mUnitUsages.ContainsKey(unitName))
		{
			mUnitUsages.Add(unitName, 0);
		}
		mUnitUsages[unitName] = mUnitUsages[unitName] + 1;
		return orCreateUnitDict[animEntry.Name];
	}

	private static Dictionary<string, Dictionary<string, Sprite[]>> mSheetCache = new Dictionary<string, Dictionary<string, Sprite[]>>();

	// PC port: the AssetRipper export lost the sprite lists of the sheet PNGs and extracted every slice as a standalone
	// Sprite asset (m_Name = slice index, texture = sheet PNG) next to the PNGs, so the shipped
	// Resources.LoadAll("SpriteSheets/<unit>/<sheet>") returns nothing. Rebuild the sheet from the folder's loose sprites.
	private static Sprite[] LoadSheet(string unitName, string sheetName)
	{
		Sprite[] direct = Resources.LoadAll<Sprite>(string.Format("SpriteSheets/{0}/{1}", unitName, sheetName));
		if (direct.Length != 0)
		{
			return direct;
		}
		string key = unitName.ToLowerInvariant();
		if (!mSheetCache.TryGetValue(key, out Dictionary<string, Sprite[]> sheets))
		{
			sheets = new Dictionary<string, Sprite[]>();
			Dictionary<string, List<Sprite>> groups = new Dictionary<string, List<Sprite>>();
			Sprite[] all = Resources.LoadAll<Sprite>(string.Format("SpriteSheets/{0}", unitName));
			for (int i = 0; i < all.Length; i++)
			{
				if (all[i].texture == null)
				{
					continue;
				}
				string tex = all[i].texture.name.ToLowerInvariant();
				if (!groups.TryGetValue(tex, out List<Sprite> list))
				{
					list = new List<Sprite>();
					groups[tex] = list;
				}
				list.Add(all[i]);
			}
			foreach (KeyValuePair<string, List<Sprite>> group in groups)
			{
				group.Value.Sort(delegate(Sprite a, Sprite b)
				{
					int ia, ib;
					bool pa = int.TryParse(a.name, out ia);
					bool pb = int.TryParse(b.name, out ib);
					if (pa && pb)
					{
						return ia.CompareTo(ib);
					}
					return string.CompareOrdinal(a.name, b.name);
				});
				sheets[group.Key] = group.Value.ToArray();
			}
			mSheetCache[key] = sheets;
		}
		if (sheets.TryGetValue(sheetName.ToLowerInvariant(), out Sprite[] result))
		{
			return result;
		}
		return direct;
	}

	public static void ClearLeastUsedUnits()
	{
		List<KeyValuePair<string, int>> list = mUnitUsages.OrderBy((KeyValuePair<string, int> u) => u.Value).ToList();
		while (mAnimations.Count >= 21)
		{
			string key = list.ElementAt(0).Key;
			mAnimations.Remove(key);
			mUnitUsages.Remove(key);
			list.RemoveAt(0);
		}
		mUnitUsages = new Dictionary<string, int>();
	}

	public static FAnimation GetAnimation(string unitName, string anim)
	{
		if (HasAnimation(unitName, anim))
		{
			return GetOrCreateUnitDict(unitName)[anim];
		}
		return null;
	}

	private static Dictionary<string, FAnimation> GetOrCreateUnitDict(string unitName)
	{
		if (!mAnimations.ContainsKey(unitName))
		{
			mAnimations[unitName] = new Dictionary<string, FAnimation>();
		}
		return mAnimations[unitName];
	}

	public static bool HasAnimation(string unitName, string anim)
	{
		if (!mAnimations.ContainsKey(unitName))
		{
			return false;
		}
		return mAnimations[unitName].ContainsKey(anim);
	}
}
