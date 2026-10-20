// Rewritten from libil2cpp.so 1.0.605 (ExistSoundEntry @ 0x17cb780..0x17cb88c, Ghidra vaddr).
public class ExistSoundEntry
{
	protected int mRefCount;

	protected string mSoundSetName;

	protected FAudioHelper mCurrentAudioHelper;

	protected float mSoundSetVolume = 1f;

	protected float mVolume = 1f;

	public ExistSoundEntry(string soundSetName)
	{
		mSoundSetName = soundSetName;
		FSoundSet soundSet = (soundSetName != null) ? FApp.SoundManager.GetSoundSet(soundSetName) : null;
		if (soundSet != null)
		{
			mSoundSetVolume = soundSet.PlaybackInfo.MaxVolume;
		}
	}

	public void AddRef()
	{
		if (mCurrentAudioHelper == null)
		{
			mCurrentAudioHelper = FApp.SoundManager.PlaySoundSet(mSoundSetName, 0f, 1f, looping: true, selfDestruct: false);
			if (mCurrentAudioHelper != null)
			{
				mCurrentAudioHelper.FadeIn(0.25f, mVolume * mSoundSetVolume * FApp.SoundManager.EffectVolume, 0f);
			}
		}
		mRefCount++;
	}

	public void ReleaseRef()
	{
		mRefCount--;
		if (mRefCount >= 1)
		{
			return;
		}
		if (mRefCount < 0)
		{
			FLog.UnityError("Exist sound over-released!");
		}
		if (!(mCurrentAudioHelper == null))
		{
			mCurrentAudioHelper.FadeOut(0.25f, releaseWhenDone: true);
			mCurrentAudioHelper = null;
		}
	}

	public void SetVolume(float volume)
	{
		mVolume = volume;
		VolumeChange();
	}

	public void VolumeChange()
	{
		if (mCurrentAudioHelper != null)
		{
			mCurrentAudioHelper.SetVolume(mVolume * mSoundSetVolume * FApp.SoundManager.EffectVolume);
		}
	}
}
