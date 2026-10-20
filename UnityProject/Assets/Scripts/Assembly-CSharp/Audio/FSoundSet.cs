using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FSoundSet @ 0x17cd57c..0x17cf66c, Ghidra vaddr).
public class FSoundSet
{
	public enum SelectType
	{
		Random = 0,
		Cycle = 1,
		ShuffledCycle = 2
	}

	public string Name;

	public List<FSoundSetEntry> Entries = new List<FSoundSetEntry>();

	public FSoundPlaybackInfo PlaybackInfo = new FSoundPlaybackInfo();

	public float MinTimeBetweenPlays = 0.01f;

	protected SelectType mSelectType;

	protected int mCurrentIndex;

	protected int mTotalWeight;

	protected float mLastTimePlayed = float.NegativeInfinity;

	public bool LoadXml(XmlElement xmlRoot)
	{
		Name = xmlRoot.GetAttribute("Name");
		FEnum.TryParse(xmlRoot.GetAttribute("SelectType"), ref mSelectType);
		MinTimeBetweenPlays = xmlRoot.GetAttrib("MinTimeBetweenPlays", MinTimeBetweenPlays);
		PlaybackInfo.LoadXml(xmlRoot);
		foreach (XmlNode childNode in xmlRoot.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element || !(childNode.Name == "Sound"))
			{
				continue;
			}
			FSoundSetEntry fSoundSetEntry = new FSoundSetEntry();
			if (fSoundSetEntry.LoadXml(childNode as XmlElement))
			{
				Entries.Add(fSoundSetEntry);
				mTotalWeight += fSoundSetEntry.Weight;
			}
		}
		mCurrentIndex = Entries.Count;
		return true;
	}

	protected FSoundSetEntry SelectRandom()
	{
		int num = FRandom.Generic.RangeInt(0, mTotalWeight - 1);
		int num2 = 0;
		foreach (FSoundSetEntry entry in Entries)
		{
			num2 += entry.Weight;
			if (num2 > num)
			{
				return entry;
			}
		}
		return null;
	}

	protected FSoundSetEntry SelectCycle(bool shuffleOnRollover)
	{
		mCurrentIndex++;
		if (mCurrentIndex >= Entries.Count)
		{
			if (Entries.Count > 2 && shuffleOnRollover)
			{
				int num = FRandom.Generic.RangeInt(0, Entries.Count - 2);
				FSoundSetEntry value = Entries[num];
				Entries[num] = Entries[0];
				Entries[0] = value;
				Entries.Shuffle(1);
				mCurrentIndex = 0;
				return Entries[0];
			}
			mCurrentIndex = 0;
		}
		return Entries[mCurrentIndex];
	}

	protected FSoundSetEntry Select()
	{
		switch (mSelectType)
		{
		case SelectType.ShuffledCycle:
			return SelectCycle(shuffleOnRollover: true);
		case SelectType.Cycle:
			return SelectCycle(shuffleOnRollover: false);
		case SelectType.Random:
			return SelectRandom();
		default:
			return null;
		}
	}

	public FAudioHelper PlayAtPoint(Vector3 position, float parentVolume = 1f, float parentPitch = 1f, bool looping = false, bool selfDestruct = true)
	{
		if (Entries.Count < 1 || Time.time - mLastTimePlayed < MinTimeBetweenPlays)
		{
			return null;
		}
		FSoundSetEntry fSoundSetEntry = Select();
		if (fSoundSetEntry == null)
		{
			return null;
		}
		float volume = PlaybackInfo.GetVolume(parentVolume);
		float pitch = PlaybackInfo.GetPitch(parentPitch);
		FAudioHelper fAudioHelper = fSoundSetEntry.PlayAtPoint(position, volume, pitch, looping, selfDestruct, PlaybackInfo.InWorld, PlaybackInfo.MinAttenuateDistance, PlaybackInfo.MaxAttenuateDistance);
		if (fAudioHelper == null)
		{
			return null;
		}
		mLastTimePlayed = Time.time;
		return fAudioHelper;
	}

	public FAudioHelper PlayAtGameObject(GameObject parentGO, float parentVolume = 1f, float parentPitch = 1f, bool looping = false, bool selfDestruct = true)
	{
		if (Entries.Count < 1 || Time.time - mLastTimePlayed < MinTimeBetweenPlays)
		{
			return null;
		}
		FSoundSetEntry fSoundSetEntry = Select();
		if (fSoundSetEntry == null)
		{
			return null;
		}
		float volume = PlaybackInfo.GetVolume(parentVolume);
		float pitch = PlaybackInfo.GetPitch(parentPitch);
		FAudioHelper fAudioHelper = fSoundSetEntry.PlayAtGameObject(parentGO, volume, pitch, looping, selfDestruct, PlaybackInfo.InWorld, PlaybackInfo.MinAttenuateDistance, PlaybackInfo.MaxAttenuateDistance);
		if (fAudioHelper == null)
		{
			return null;
		}
		mLastTimePlayed = Time.time;
		return fAudioHelper;
	}
}
