using System.Collections;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAudioHelper @ 0x16eed38..0x16f00a4, Ghidra vaddr).
[RequireComponent(typeof(AudioSource))]
public class FAudioHelper : MonoBehaviour
{
	protected float mBaseVolume = 1f;

	protected bool mInWorld = true;

	protected float mStartFadePct = 1f;

	protected float mEndFadePct = 1.5f;

	public PositionalExistSoundEntry mPositionalExist;

	public GameObject TrackGameObject { get; set; }

	private void Start()
	{
	}

	protected IEnumerator DoFadeOut(float baseVolume, float fadeTime, bool releaseWhenDone)
	{
		float fadeTimeRemaining = fadeTime;
		while (fadeTimeRemaining > 0f)
		{
			float num = FMath.smoothstep(fadeTimeRemaining / fadeTime);
			GetComponent<AudioSource>().volume = num * baseVolume;
			fadeTimeRemaining -= Time.deltaTime;
			yield return null;
		}
		GetComponent<AudioSource>().Stop();
		if (releaseWhenDone)
		{
			Release();
		}
	}

	protected IEnumerator DoFadeIn(float targetVolume, float fadeTime, float startDelayTime)
	{
		float delayTimeRemaining = startDelayTime;
		if (startDelayTime > 0f)
		{
			GetComponent<AudioSource>().Stop();
		}
		while (delayTimeRemaining > 0f)
		{
			delayTimeRemaining -= Time.deltaTime;
			yield return null;
		}
		GetComponent<AudioSource>().Play();
		float fadeTimeRemaining = fadeTime;
		while (fadeTimeRemaining > 0f)
		{
			float num = FMath.smoothstep(fadeTimeRemaining / fadeTime);
			GetComponent<AudioSource>().volume = (1f - num) * targetVolume;
			fadeTimeRemaining -= Time.deltaTime;
			yield return null;
		}
	}

	public void OnReleaseObject()
	{
		AudioSource component = GetComponent<AudioSource>();
		component.Stop();
		component.clip = null;
		component.pitch = 1f;
		component.volume = 1f;
		component.loop = false;
		TrackGameObject = null;
		StopAllCoroutines();
	}

	public void PlayAtPoint(AudioClip clip, Vector3 position, float volume, float pitch, bool looping = false, bool selfDestruct = true, bool inWorld = false, float minAttenuateDistance = 1f, float maxAttenuateDistance = 1.5f)
	{
		base.transform.position = position;
		AudioSource component = GetComponent<AudioSource>();
		component.clip = clip;
		component.volume = volume;
		component.pitch = pitch;
		component.loop = looping;
		component.panStereo = 0f;
		mBaseVolume = volume;
		mInWorld = inWorld;
		mStartFadePct = minAttenuateDistance;
		mEndFadePct = maxAttenuateDistance;
		if (volume > FMath.Epsilon)
		{
			ComputeFadePan();
			GetComponent<AudioSource>().Play();
		}
		if (selfDestruct)
		{
			StartCoroutine(WaitThenRelease());
		}
	}

	protected void ComputeFadePan()
	{
		if (!mInWorld || mPositionalExist == null || mPositionalExist.Positions == null || mPositionalExist.Positions.Count < 1)
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < mPositionalExist.Positions.Count; i++)
		{
			ComputeScreenSpaceFadePan(mPositionalExist.Positions[i], out var fade, out var pan, out var distSS);
			if (distSS <= 0.01f)
			{
				distSS = 0.01f;
			}
			float num4 = 1f / distSS;
			num3 += num4;
			num += fade * num4;
			num2 += pan * num4;
		}
		mPositionalExist.SetVolume(num / num3);
		GetComponent<AudioSource>().panStereo = num2 / num3;
	}

	protected IEnumerator WaitThenRelease()
	{
		yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length + 0.1f);
		Release();
	}

	public void PlayAtGameObject(AudioClip clip, GameObject parentGO, float volume, float pitch, bool looping = false, bool selfDestruct = true, bool inWorld = false, float minAttenuateDistance = 1f, float maxAttenuateDistance = 1.5f)
	{
		TrackGameObject = parentGO;
		AudioSource component = GetComponent<AudioSource>();
		component.clip = clip;
		component.volume = volume;
		component.pitch = pitch;
		component.loop = looping;
		component.panStereo = 0f;
		mBaseVolume = volume;
		mInWorld = inWorld;
		mStartFadePct = minAttenuateDistance;
		mEndFadePct = maxAttenuateDistance;
		if (volume > FMath.Epsilon)
		{
			ComputeFadePan();
			GetComponent<AudioSource>().Play();
		}
		if (selfDestruct)
		{
			StartCoroutine(WaitThenRelease());
		}
	}

	public void SetVolume(float volume)
	{
		StopAllCoroutines();
		AudioSource component = GetComponent<AudioSource>();
		if (!component.isPlaying)
		{
			GetComponent<AudioSource>().Play();
		}
		GetComponent<AudioSource>().volume = volume;
	}

	public void FadeOut(float fadeTime, bool releaseWhenDone)
	{
		StopAllCoroutines();
		StartCoroutine(DoFadeOut(GetComponent<AudioSource>().volume, fadeTime, releaseWhenDone));
	}

	public void FadeIn(float fadeTime, float targetVolume, float startDelayTime)
	{
		StopAllCoroutines();
		if (fadeTime > 0f)
		{
			GetComponent<AudioSource>().Stop();
		}
		StartCoroutine(DoFadeIn(targetVolume, fadeTime, startDelayTime));
	}

	public void Release()
	{
		FApp.SoundManager.ReleaseHelper(this);
	}

	public void Update()
	{
		if (GetComponent<AudioSource>().isPlaying)
		{
			if (TrackGameObject != null)
			{
				base.transform.position = TrackGameObject.transform.position;
			}
			ComputeFadePan();
		}
	}

	protected void ComputeScreenSpaceFadePan(Vector3 pos, out float fade, out float pan, out float distSS)
	{
		fade = 1f;
		pan = 0f;
		distSS = 0f;
		if (!(FApp.World == null) && FApp.World.Camera != null)
		{
			// 0x16ef9fc..0x16efab0: the call returns (s0, s1, s2); x and y are used exactly as below (max(|x|,|y|), sqrt(x*x+y*y), x * 0.75)
			Vector3 vector = FApp.World.Camera.WorldToCenteredViewportPoint(pos);
			float num = Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
			distSS = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
			fade = Mathf.Clamp01((mEndFadePct - num) / (mEndFadePct - mStartFadePct));
			pan = vector.x * 0.75f;
		}
	}

	public void OnDestroy()
	{
		// Original asserts if destroyed before app shutdown; harmless on PC (also fires on scene teardown), so omitted.
	}
}
