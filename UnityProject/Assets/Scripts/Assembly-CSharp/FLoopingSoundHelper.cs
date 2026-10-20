using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FLoopingSoundHelper @ 0x17ee6a0..0x17eeb14, Ghidra vaddr).
public class FLoopingSoundHelper
{
	protected Dictionary<string, FLoopingSound> mSounds = new Dictionary<string, FLoopingSound>();

	public void PlayLoopingSound(string soundName)
	{
		if (!mSounds.ContainsKey(soundName))
		{
			mSounds[soundName] = new FLoopingSound(soundName);
		}
		mSounds[soundName].IncrementNumPlaying();
	}

	public void StopLoopingSound(string soundName)
	{
		if (mSounds.ContainsKey(soundName) && mSounds[soundName] != null)
		{
			mSounds[soundName].DecrementNumPlaying();
			if (mSounds[soundName].NumPlaying <= 0)
			{
				mSounds[soundName].ReleaseHelper();
				mSounds.Remove(soundName);
			}
		}
	}

	public bool IsPlaying(string soundName)
	{
		return mSounds.ContainsKey(soundName);
	}

	public void StopAllLoopingSounds()
	{
		foreach (FLoopingSound value in mSounds.Values)
		{
			value.ReleaseHelper();
		}
		mSounds.Clear();
	}
}
