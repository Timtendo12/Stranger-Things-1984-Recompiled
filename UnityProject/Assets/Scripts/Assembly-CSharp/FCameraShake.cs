using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FCameraShake @ 0x16f23d0..0x16f2dbc, Ghidra vaddr).
public class FCameraShake : MonoBehaviour
{
	protected Vector3 mOffset = Vector3.zero;

	protected HashSet<FCameraShakeEntry> mEntries = new HashSet<FCameraShakeEntry>();

	protected List<FCameraShakeEntry> mEntriesToRemove = new List<FCameraShakeEntry>();

	protected Camera mCamera;

	public void Start()
	{
		mCamera = GetComponent<Camera>();
	}

	public void Update()
	{
		mOffset = Vector3.zero;
		float deltaTime = Time.deltaTime;
		foreach (FCameraShakeEntry mEntry in mEntries)
		{
			if (!mEntry.Update(mCamera, deltaTime, ref mOffset))
			{
				mEntriesToRemove.Add(mEntry);
			}
		}
		foreach (FCameraShakeEntry item in mEntriesToRemove)
		{
			mEntries.Remove(item);
		}
	}

	public void StartShake(Vector3 position, float duration, float frequency, float magnitude)
	{
		ComputeScreenSpaceFadePan(position, out var fade, out var _);
		if (!(fade < FMath.Epsilon))
		{
			mEntries.Add(new FCameraShakeEntryPerlin(duration, frequency, fade * magnitude));
		}
	}

	public void OnPreCull()
	{
		if (!(GetComponent<Camera>() == null))
		{
			GetComponent<Camera>().transform.position += mOffset;
		}
	}

	public void OnPostRender()
	{
		if (!(GetComponent<Camera>() == null))
		{
			GetComponent<Camera>().transform.position -= mOffset;
		}
	}

	protected void ComputeScreenSpaceFadePan(Vector3 pos, out float fade, out float pan)
	{
		fade = 1f;
		pan = 0f;
		Vector3 zero = Vector3.zero;
		if (FApp.World != null)
		{
			zero = FApp.World.Camera.WorldToCenteredViewportPoint(pos);
		}
		float a = Mathf.Abs(zero.x);
		float b = Mathf.Abs(zero.y);
		float num = Mathf.Max(a, b);
		fade = Mathf.Clamp01((1.25f - num) / 0.75f);
		pan = zero.x * 0.75f;
	}
}
