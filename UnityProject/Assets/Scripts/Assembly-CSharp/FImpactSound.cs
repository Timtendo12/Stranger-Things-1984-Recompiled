using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FImpactSound @ 0x1789628..0x1789a90, Ghidra vaddr).
public class FImpactSound : MonoBehaviour
{
	public List<FImpactSoundInfo> ImpactSoundInfos;

	protected const float cMinTimeBetweenSounds = 0.2f;

	protected float mLastPlayedTime = float.MinValue;

	private void Start()
	{
		ImpactSoundInfos.Sort((FImpactSoundInfo a, FImpactSoundInfo b) => a.MinImpactSpeed.CompareTo(b.MinImpactSpeed));
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (Time.realtimeSinceStartup - mLastPlayedTime < 0.2f)
		{
			return;
		}
		float magnitude = collision.relativeVelocity.magnitude;
		foreach (FImpactSoundInfo impactSoundInfo in ImpactSoundInfos)
		{
			if (!(magnitude < impactSoundInfo.MinImpactSpeed))
			{
				FApp.SoundManager.PlaySoundSetAtPoint(impactSoundInfo.SoundSetName, base.transform.position);
				mLastPlayedTime = Time.realtimeSinceStartup;
				break;
			}
		}
	}
}
