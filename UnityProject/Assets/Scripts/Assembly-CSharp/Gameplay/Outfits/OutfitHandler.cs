using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (OutfitHandler @ 0x17710c0..0x17737d0, Ghidra vaddr).
public static class OutfitHandler
{
	private const string HOPPER = "Hopper";

	private const string LUCAS = "Lucas";

	private const string MIKE = "Mike";

	private const string MIKEONABIKE = "MikeOnABike";

	private const string ELEVEN = "Eleven";

	private const string DUSTIN = "Dustin";

	private const string MAX = "Max";

	private const string NANCNY = "Nancy";

	private const string WILL = "Will";

	private static Dictionary<string, List<Outfit>> unlockedOutfits;

	private static Dictionary<string, UIOutfit> uIImagesForCharacterOutfit;

	private static List<string> proccessedCollectibles;

	private const char KeyValuePairSpliter = '|';

	private const char KeyValueSpliter = '/';

	private const char OutfitElementSpliter = '~';

	private static bool isSerializationRequired;

	private static string lastSerializedOutfits;

	static OutfitHandler()
	{
		unlockedOutfits = new Dictionary<string, List<Outfit>>();
		unlockedOutfits.Add("Hopper", new List<Outfit>());
		unlockedOutfits.Add("Lucas", new List<Outfit>());
		unlockedOutfits.Add("Mike", new List<Outfit>());
		unlockedOutfits.Add("MikeOnABike", new List<Outfit>());
		unlockedOutfits.Add("Eleven", new List<Outfit>());
		unlockedOutfits.Add("Dustin", new List<Outfit>());
		unlockedOutfits.Add("Max", new List<Outfit>());
		unlockedOutfits.Add("Nancy", new List<Outfit>());
		unlockedOutfits.Add("Will", new List<Outfit>());
		uIImagesForCharacterOutfit = new Dictionary<string, UIOutfit>();
		uIImagesForCharacterOutfit.Add("Hopper", new UIOutfit("Hopper_Icon_Full.png", "Outfit000_Icon_Full.png", "Button002.png", "Outfit000_Icon.png", "Outfit000_IconSelected.png", "Outfit000_IconUnselected.png"));
		uIImagesForCharacterOutfit.Add("Lucas", new UIOutfit("Lucas_Icon_Full.png", "Outfit001_Icon_Full.png", "Button002.png", "Outfit001_Icon.png", "Outfit001_IconSelected.png", "Outfit001_IconUnselected.png"));
		uIImagesForCharacterOutfit.Add("Mike", new UIOutfit("Mike_Icon_Full.png", "Outfit003_Icon_Full.png", "Button002.png", "Outfit003_Icon.png", "Outfit003_IconSelected.png", "Outfit003_IconUnselected.png"));
		uIImagesForCharacterOutfit.Add("Eleven", new UIOutfit("Eleven_Icon_Full.png", "Outfit006_Icon_Full.png", "Button002.png", "Outfit006_Icon.png", "Outfit006_IconSelected.png", "Outfit006_IconUnselected.png"));
		uIImagesForCharacterOutfit.Add("Dustin", new UIOutfit("Dustin_Icon_Full.png", "Outfit005_Icon_Full.png", "Button002.png", "Outfit005_Icon.png", "Outfit005_IconSelected.png", "Outfit005_IconUnselected.png"));
		uIImagesForCharacterOutfit.Add("Max", new UIOutfit("Max_Icon_Full.png", "Outfit007_Icon_Full.png", "Button002.png", "Outfit007_Icon.png", "Outfit007_IconSelected.png", "Outfit007_IconUnselected.png"));
		uIImagesForCharacterOutfit.Add("Nancy", new UIOutfit("Nancy_Icon_Full.png", "Outfit002_Icon_Full.png", "Button002.png", "Outfit002_Icon.png", "Outfit002_IconSelected.png", "Outfit002_IconUnselected.png"));
		uIImagesForCharacterOutfit.Add("Will", new UIOutfit("Will_Icon_Full.png", "Outfit004_Icon_Full.png", "Button002.png", "Outfit004_Icon.png", "Outfit004_IconSelected.png", "Outfit004_IconUnselected.png"));
		proccessedCollectibles = new List<string>();
	}

	public static string Serialize()
	{
		if (isSerializationRequired)
		{
			lastSerializedOutfits = string.Empty;
			isSerializationRequired = false;
			int index = 0;
			foreach (KeyValuePair<string, List<Outfit>> pair in unlockedOutfits)
			{
				lastSerializedOutfits = lastSerializedOutfits + pair.Key + "/";
				List<Outfit> outfits = pair.Value;
				for (int i = 0; i < outfits.Count; i++)
				{
					lastSerializedOutfits += JsonUtility.ToJson(outfits[i]);
					if (i < outfits.Count)
					{
						lastSerializedOutfits += "~";
					}
				}
				index++;
				if (index < unlockedOutfits.Count)
				{
					lastSerializedOutfits += "|";
				}
			}
		}
		return lastSerializedOutfits;
	}

	public static void Deserialize(string outfits)
	{
		foreach (List<Outfit> value in unlockedOutfits.Values)
		{
			value.Clear();
		}
		isSerializationRequired = true;
		string[] pairs = outfits.Split('|');
		for (int i = 0; i < pairs.Length; i++)
		{
			string[] keyValue = pairs[i].Split('/');
			List<Outfit> list;
			if (unlockedOutfits.TryGetValue(keyValue[0], out list))
			{
				string[] elements = keyValue[1].Split('~');
				for (int j = 0; j < elements.Length; j++)
				{
					if (!string.IsNullOrEmpty(elements[j]))
					{
						Outfit outfit = JsonUtility.FromJson<Outfit>(elements[j]);
						if (!IsContainsWithThisName(outfit.Name, list))
						{
							list.Add(outfit);
						}
					}
				}
			}
		}
	}

	public static string GetCurrentOutfitForCharacter(string characterName)
	{
		List<Outfit> list;
		if (unlockedOutfits.TryGetValue(characterName, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].IsCurrent)
				{
					return list[i].Name;
				}
			}
		}
		return string.Empty;
	}

	public static void SetCurrentOutfitForCharacter(string characterName)
	{
		InternalSetCurrentOutfitForCharacter(characterName);
		if (characterName.Equals("Mike"))
		{
			List<Outfit> mike;
			unlockedOutfits.TryGetValue("Mike", out mike);
			List<Outfit> mikeOnABike;
			unlockedOutfits.TryGetValue("MikeOnABike", out mikeOnABike);
			if (mike.Count == mikeOnABike.Count)
			{
				for (int i = 0; i < mike.Count; i++)
				{
					mikeOnABike[i].IsCurrent = mike[i].IsCurrent;
				}
			}
		}
	}

	private static void InternalSetCurrentOutfitForCharacter(string characterName)
	{
		List<Outfit> list;
		if (!unlockedOutfits.TryGetValue(characterName, out list) || list.Count == 0)
		{
			return;
		}
		isSerializationRequired = true;
		int i = 0;
		while (i < list.Count)
		{
			if (list[i].IsCurrent)
			{
				list[i].IsCurrent = false;
				// original compares Count <= i (never true after a successful list[i]); kept as shipped
				if (list.Count <= i)
				{
					list[i + 1].IsCurrent = true;
				}
				else
				{
					list[i].IsCurrent = false;
				}
				return;
			}
			i++;
		}
		list[0].IsCurrent = true;
	}

	public static void HandleOutfitAdding(List<string> collectibles)
	{
		for (int i = 0; i < collectibles.Count; i++)
		{
			if (proccessedCollectibles.Contains(collectibles[i]))
			{
				continue;
			}
			proccessedCollectibles.Add(collectibles[i]);
			switch (collectibles[i])
			{
			case "Outfit000":
				AddOutfit("Hopper");
				break;
			case "Outfit001":
				AddOutfit("Lucas");
				break;
			case "Outfit002":
				AddOutfit("Nancy");
				break;
			case "Outfit003":
				AddOutfit("Mike");
				AddOutfit("MikeOnABike");
				break;
			case "Outfit004":
				AddOutfit("Will");
				break;
			case "Outfit005":
				AddOutfit("Dustin");
				break;
			case "Outfit006":
				AddOutfit("Eleven");
				break;
			case "Outfit007":
				AddOutfit("Max");
				break;
			}
		}
	}

	public static string GetCharacterNameFromOutfitName(string outfitName)
	{
		switch (outfitName)
		{
		case "Outfit000":
			return "Hopper";
		case "Outfit001":
			return "Lucas";
		case "Outfit002":
			return "Nancy";
		case "Outfit003":
			return "Mike";
		case "Outfit004":
			return "Will";
		case "Outfit005":
			return "Dustin";
		case "Outfit006":
			return "Eleven";
		case "Outfit007":
			return "Max";
		default:
			return string.Empty;
		}
	}

	public static string GetSelectedSpriteforCharacter(string characterName)
	{
		UIOutfit outfit;
		if (string.IsNullOrEmpty(characterName) || !uIImagesForCharacterOutfit.TryGetValue(characterName, out outfit))
		{
			return "HopperUpgrade000_Icon.png";
		}
		return outfit.SelectedOutfitIcon;
	}

	public static string GetUnselectedSpriteforCharacter(string characterName)
	{
		UIOutfit outfit;
		if (string.IsNullOrEmpty(characterName) || !uIImagesForCharacterOutfit.TryGetValue(characterName, out outfit))
		{
			return "HopperUpgrade000_Icon.png";
		}
		return outfit.UnselectedOutfitIcon;
	}

	public static string GetUICharacterImageName(string characterName)
	{
		UIOutfit outfit;
		if (!uIImagesForCharacterOutfit.TryGetValue(characterName, out outfit))
		{
			return string.Empty;
		}
		if (IsNewOutfit(characterName))
		{
			return outfit.NewOutfit;
		}
		return outfit.DefaultOutfit;
	}

	public static string GetUIOutfitButtonImageName(string characterName)
	{
		UIOutfit outfit;
		if (!uIImagesForCharacterOutfit.TryGetValue(characterName, out outfit))
		{
			return string.Empty;
		}
		if (!IsOutfitUnlocked(characterName))
		{
			return outfit.LockedOutfitButton;
		}
		return outfit.UnlockedOutfitButton;
	}

	public static bool IsOutfitUnlocked(string characterName)
	{
		List<Outfit> list;
		if (!unlockedOutfits.TryGetValue(characterName, out list))
		{
			return false;
		}
		return IsContainsWithThisName("2", list);
	}

	private static bool IsNewOutfit(string characterName)
	{
		List<Outfit> list;
		if (!unlockedOutfits.TryGetValue(characterName, out list))
		{
			return false;
		}
		return IsCurrentOutfitNew(list);
	}

	private static void AddOutfit(string name)
	{
		List<Outfit> list;
		unlockedOutfits.TryGetValue(name, out list);
		if (!IsContainsWithThisName("2", list))
		{
			list.Add(new Outfit("2"));
			isSerializationRequired = true;
		}
	}

	private static bool IsContainsWithThisName(string name, List<Outfit> outfits)
	{
		for (int i = 0; i < outfits.Count; i++)
		{
			if (name == outfits[i].Name)
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsCurrentOutfitNew(List<Outfit> outfits)
	{
		for (int i = 0; i < outfits.Count; i++)
		{
			if ("2" == outfits[i].Name && outfits[i].IsCurrent)
			{
				return true;
			}
		}
		return false;
	}
}
