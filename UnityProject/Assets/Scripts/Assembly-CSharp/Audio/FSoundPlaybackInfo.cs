using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FSoundPlaybackInfo @ 0x17cea20..0x17ceddc, Ghidra vaddr).
public class FSoundPlaybackInfo
{
	public float MinVolume = 1f;

	public float MaxVolume = 1f;

	public float MinPitch = 1f;

	public float MaxPitch = 1f;

	public float MinAttenuateDistance = 1f;

	public float MaxAttenuateDistance = 1.5f;

	public bool InWorld = true;

	public void LoadXml(XmlElement xmlRoot)
	{
		if (xmlRoot.GetAttribute("Volume").TryParseFloat(out var val))
		{
			MinVolume = val;
			MaxVolume = val;
		}
		MinVolume = xmlRoot.GetAttrib("MinVolume", MinVolume);
		MaxVolume = xmlRoot.GetAttrib("MaxVolume", MaxVolume);
		if (MaxVolume < MinVolume)
		{
			FCoreUtil.Swap(ref MinVolume, ref MaxVolume);
		}
		if (xmlRoot.GetAttribute("Pitch").TryParseFloat(out var val2))
		{
			MinPitch = val2;
			MaxPitch = val2;
		}
		MinPitch = xmlRoot.GetAttrib("MinPitch", MinPitch);
		MaxPitch = xmlRoot.GetAttrib("MaxPitch", MaxPitch);
		if (MaxPitch < MinPitch)
		{
			FCoreUtil.Swap(ref MinPitch, ref MaxPitch);
		}
		InWorld = xmlRoot.GetAttrib("InWorld", InWorld);
		MaxAttenuateDistance = xmlRoot.GetAttrib("MaxAttenuateDistance", 1.5f);
		MinAttenuateDistance = xmlRoot.GetAttrib("MinAttenuateDistance", 1f);
	}

	public float GetVolume(float parentVolume)
	{
		return FRandom.Generic.RangeFloat(MinVolume, MaxVolume) * parentVolume;
	}

	public float GetPitch(float parentPitch)
	{
		return FRandom.Generic.RangeFloat(MinPitch, MaxPitch) * parentPitch;
	}
}
