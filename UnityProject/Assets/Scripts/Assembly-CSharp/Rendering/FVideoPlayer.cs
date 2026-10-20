using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

// Rewritten from libil2cpp.so 1.0.605 (FVideoPlayer @ 0x1752ed0..0x1754070, Ghidra vaddr).
public static class FVideoPlayer
{
	private static float mBeforeSoundVolume;

	private static float mBeforeMusicVolume;

	private static VideoPlayer mCurrentPlayer;

	private static Action m_mLoopPointReachedAction;

	private static string mCurrentVideoName = string.Empty;

	private static event Action mLoopPointReachedAction
	{
		add
		{
			m_mLoopPointReachedAction = (Action)Delegate.Combine(m_mLoopPointReachedAction, value);
		}
		remove
		{
			m_mLoopPointReachedAction = (Action)Delegate.Remove(m_mLoopPointReachedAction, value);
		}
	}

	public static event Action LoopPointReachedAction
	{
		add
		{
			mLoopPointReachedAction -= value;
			mLoopPointReachedAction += value;
		}
		remove
		{
			mLoopPointReachedAction -= value;
		}
	}

	public static bool PlayVideo(string path)
	{
		if (!(FApp.World == null) && FCoreUtil.Valid(path))
		{
			StartVideoFromResources(path);
			return true;
		}
		return false;
	}

	private static void StartVideoFromResources(string name)
	{
		Camera worldCamera = FApp.GetWorldCamera();
		if (worldCamera == null)
		{
			FLog.Warning("FVideoPlayer.StartVideoFromResources: World camera is null!");
			return;
		}
		VideoClip videoClip = Resources.Load<VideoClip>("Video/" + name);
		if (videoClip == null)
		{
			FLog.Warning("FVideoPlayer.StartVideoFromResources: VideoClip is null!");
			return;
		}
		mCurrentVideoName = name;
		mCurrentPlayer = worldCamera.gameObject.GetOrCreateComponent<VideoPlayer>();
		mCurrentPlayer.playOnAwake = false;
		mCurrentPlayer.clip = videoClip;
		mCurrentPlayer.targetCamera = worldCamera;
		mCurrentPlayer.renderMode = VideoRenderMode.CameraFarPlane;
		AudioSource target = worldCamera.gameObject.GetOrCreateComponent<AudioSource>();
		mCurrentPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
		mCurrentPlayer.SetTargetAudioSource(0, target);
		mCurrentPlayer.EnableAudioTrack(0, true);
		mCurrentPlayer.isLooping = true;
		mCurrentPlayer.loopPointReached += LoopPointReached;
		mCurrentPlayer.prepareCompleted += PrepareComplete;
		mCurrentPlayer.Prepare();
		VideoStarted();
		FLog.Log("FVideoPlayer.StartVideoFromResources: Preparing video.");
	}

	private static void VideoStarted()
	{
		mBeforeSoundVolume = FApp.SoundManager.EffectVolume;
		mBeforeMusicVolume = FApp.SoundManager.MusicVolume;
		FApp.SoundManager.EffectVolume = 0f;
		FApp.SoundManager.MusicVolume = 0f;
	}

	private static void PrepareComplete(VideoPlayer vPlayer)
	{
		FLog.Log("FVideoPlayer.PrepareComplete: Prepare complete, starting video.");
		vPlayer.Play();
		FGameUtil.StartCoroutine(EnsureVideoPlays());
	}

	private static IEnumerator EnsureVideoPlays()
	{
		yield return new WaitForSeconds(0.25f);
		long startFrame = mCurrentPlayer.frame;
		yield return new WaitForSeconds(0.25f);
		int counter = 0;
		while (startFrame == mCurrentPlayer.frame)
		{
			if (counter >= 5)
			{
				FLog.Log("FVideoPlayer.EnsureVideoPlays: Video wasn't playing after 5 attempts. Calling loop point.");
				LoopPointReached(null);
				break;
			}
			yield return new WaitForSeconds(0.5f);
			mCurrentPlayer.Play();
			counter++;
			FLog.Log("FVideoPlayer.EnsureVideoPlays: Video wasn't playing. Trying to play.");
		}
	}

	private static void LoopPointReached(VideoPlayer vPlayer)
	{
		FLog.Log("FVideoPlayer.LoopPointReached: Shutting down video player.");
		FApp.SoundManager.EffectVolume = mBeforeSoundVolume;
		FApp.SoundManager.MusicVolume = mBeforeMusicVolume;
		if (m_mLoopPointReachedAction != null)
		{
			m_mLoopPointReachedAction();
		}
		FGameUtil.DelayExecute(0.5f, CleanupVideoPlayer);
	}

	private static void CleanupVideoPlayer()
	{
		UnityEngine.Object.Destroy(mCurrentPlayer);
		mCurrentPlayer = null;
	}

	public static void EndVideo()
	{
		if (!(mCurrentPlayer == null))
		{
			LoopPointReached(null);
		}
	}
}
