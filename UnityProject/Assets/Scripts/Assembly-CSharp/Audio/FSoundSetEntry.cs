using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FSoundSetEntry @ 0x17cee10..0x17cf2e8, Ghidra vaddr).
public class FSoundSetEntry
{
	public AudioClip Clip;

	public FSoundPlaybackInfo PlaybackInfo = new FSoundPlaybackInfo();

	public int Weight = 1;

	public bool LoadXml(XmlElement xmlRoot)
	{
		string attribute = xmlRoot.GetAttribute("Name");
		if (attribute != null)
		{
			// PC port: all sounds live in Unity Resources/sounds (Resources paths are case-insensitive).
			Clip = Resources.Load("Sounds/" + attribute) as AudioClip;
			if (Clip == null)
			{
				FLog.UnityError("failed to load sound: " + attribute);
				return false;
			}
			PlaybackInfo.LoadXml(xmlRoot);
			Weight = xmlRoot.GetAttrib("Weight", Weight);
			return true;
		}
		return false;
	}

	public FAudioHelper PlayAtPoint(Vector3 position, float parentVolume, float parentPitch, bool looping, bool selfDestruct, bool inWorld, float minAttenuateDistance, float maxAttenuateDistance)
	{
		AudioClip clip = Clip;
		FSoundManager soundManager = FApp.SoundManager;
		float volume = PlaybackInfo.GetVolume(parentVolume);
		float pitch = PlaybackInfo.GetPitch(parentPitch);
		return soundManager.PlayClipAtPoint(clip, position, volume, pitch, looping, selfDestruct, inWorld, minAttenuateDistance, maxAttenuateDistance);
	}

	public FAudioHelper PlayAtGameObject(GameObject parentGO, float parentVolume, float parentPitch, bool looping, bool selfDestruct, bool inWorld, float minAttenuateDistance, float maxAttenuateDistance)
	{
		AudioClip clip = Clip;
		FSoundManager soundManager = FApp.SoundManager;
		float volume = PlaybackInfo.GetVolume(parentVolume);
		float pitch = PlaybackInfo.GetPitch(parentPitch);
		return soundManager.PlayClipAtGameObject(clip, parentGO, volume, pitch, looping, selfDestruct, inWorld, minAttenuateDistance, maxAttenuateDistance);
	}
}
