using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FFXTimer @ 0x1701d70..0x1702004, Ghidra vaddr).
public class FFXTimer : MonoBehaviour
{
	public enum StartStates
	{
		Active = 0,
		Inactive = 1,
		Random = 2
	}

	public float MinSecondsBetweenActivations = 1f;

	public float MaxSecondsBetweenActivations = 2f;

	public StartStates StartState;

	public float MinSecondsActive = 1f;

	public float MaxSecondsActive = 2f;

	protected bool mIsActive;

	protected float mTimeLeft;

	private void Start()
	{
		if (StartState != StartStates.Active)
		{
			if (StartState != StartStates.Random || !FRandom.Generic.RandBool())
			{
				Deactivate();
				return;
			}
			Activate();
		}
	}

	private void Update()
	{
		mTimeLeft -= Time.deltaTime;
		if (!(mTimeLeft > 0f))
		{
			if (mIsActive)
			{
				Deactivate();
			}
			else
			{
				Activate();
			}
		}
	}

	public void Deactivate()
	{
		base.gameObject.StopParticleSystemsAndTrailsRecursive(-1f);
		mIsActive = false;
		mTimeLeft = FRandom.Generic.RangeFloat(MinSecondsBetweenActivations, MaxSecondsBetweenActivations);
	}

	public void Activate()
	{
		base.gameObject.PlayParticleSystemsAndTrailsRecursive();
		mIsActive = true;
		mTimeLeft = FRandom.Generic.RangeFloat(MinSecondsActive, MaxSecondsActive);
	}
}
