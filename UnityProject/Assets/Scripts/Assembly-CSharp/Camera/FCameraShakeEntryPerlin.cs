using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FCameraShakeEntryPerlin @ 0x16f21b4..0x16f23d0, Ghidra vaddr).
public class FCameraShakeEntryPerlin : FCameraShakeEntry
{
	protected float mDuration;

	protected float mFrequency = 1f;

	protected float mMagnitude;

	protected float mSeed;

	protected float mElapsedTime;

	public FCameraShakeEntryPerlin(float duration, float frequency, float magnitude)
	{
		mSeed = FRandom.Generic.RangeFloat(0f, 1f);
		mDuration = duration;
		mFrequency = frequency;
		mMagnitude = magnitude;
	}

	public override bool Update(Camera camera, float deltaTime, ref Vector3 position)
	{
		float duration = mDuration;
		mElapsedTime += deltaTime;
		if (duration < mElapsedTime)
		{
			return mElapsedTime <= duration;
		}
		float num = Mathf.Clamp01((duration - mElapsedTime) / duration) * mMagnitude;
		float num2 = Mathf.PerlinNoise(mElapsedTime * mFrequency, mSeed);
		float num3 = Mathf.PerlinNoise(mSeed, mElapsedTime * mFrequency);
		float num4 = num * (num2 * 2f - 1f);
		position += camera.transform.right * num4;
		float num5 = num * (num3 * 2f - 1f);
		position += camera.transform.up * num5;
		return mElapsedTime <= duration;
	}
}
