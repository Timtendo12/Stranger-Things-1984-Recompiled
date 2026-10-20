// Rewritten from libil2cpp.so 1.0.605 (LoopingSoundCategory @ 0x17cc2a4..0x17cc760, Ghidra vaddr).
public class LoopingSoundCategory
{
	protected FAudioHelper mCurrentAudioHelper;

	protected string mCurrentSoundSetName;

	protected float mSoundSetVolume = 1f;

	public void Play(string soundsetName, float volume, float fadeOutTime, float fadeInTime, bool fadeIfNotCrossFading, float fadeInStartDelay)
	{
		if (soundsetName == mCurrentSoundSetName)
		{
			return;
		}
		bool flag = FadeOut(fadeOutTime);
		FSoundSet soundSet = FApp.SoundManager.GetSoundSet(soundsetName);
		if (soundSet == null)
		{
			FLog.UnityError("could not find sound set: " + soundsetName);
			return;
		}
		mSoundSetVolume = soundSet.PlaybackInfo.MaxVolume;
		mCurrentAudioHelper = FApp.SoundManager.PlaySoundSet(soundsetName, volume, 1f, looping: true, selfDestruct: false);
		if (!(mCurrentAudioHelper == null))
		{
			if (flag || fadeIfNotCrossFading)
			{
				mCurrentAudioHelper.FadeIn(fadeInTime, mSoundSetVolume * volume, fadeInStartDelay);
			}
			else
			{
				mCurrentAudioHelper.SetVolume(mSoundSetVolume * volume);
			}
			mCurrentSoundSetName = soundsetName;
		}
	}

	public bool FadeOut(float fadeOutTime)
	{
		bool flag = mCurrentAudioHelper == null;
		if (!flag)
		{
			mCurrentAudioHelper.FadeOut(fadeOutTime, releaseWhenDone: true);
			mCurrentAudioHelper = null;
			mCurrentSoundSetName = null;
		}
		return !flag;
	}

	public bool IsPlaying()
	{
		if (mCurrentAudioHelper == null)
		{
			return false;
		}
		return mCurrentAudioHelper.GetComponent<UnityEngine.AudioSource>().isPlaying;
	}

	public void SetVolume(float volume)
	{
		if (mCurrentAudioHelper != null)
		{
			mCurrentAudioHelper.SetVolume(mSoundSetVolume * volume);
		}
	}
}
