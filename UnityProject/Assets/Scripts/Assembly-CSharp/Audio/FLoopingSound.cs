using System;

// Rewritten from libil2cpp.so 1.0.605 (FLoopingSound @ 0x17ee390..0x17ee6cc, Ghidra vaddr).
public class FLoopingSound
{
	protected string mSoundName = string.Empty;

	public FAudioHelper AudioHelper { get; protected set; }

	public int NumPlaying { get; protected set; }

	public FLoopingSound(string soundName)
	{
		mSoundName = soundName;
		Init();
		FApp.SoundManager.EffectVolumeChangedAction += VolumeChanged;
	}

	protected void Init()
	{
		AudioHelper = FApp.SoundManager.PlaySoundSet(mSoundName, 1f, 1f, looping: true, selfDestruct: false);
		NumPlaying = 0;
	}

	public void IncrementNumPlaying()
	{
		NumPlaying++;
	}

	public void DecrementNumPlaying()
	{
		NumPlaying--;
	}

	public void ReleaseHelper()
	{
		if (!(AudioHelper == null))
		{
			AudioHelper.StopAllCoroutines();
			AudioHelper.Release();
			AudioHelper = null;
		}
	}

	protected void VolumeChanged(float volume)
	{
		ReleaseHelper();
		if (NumPlaying > 0)
		{
			Init();
		}
	}
}
