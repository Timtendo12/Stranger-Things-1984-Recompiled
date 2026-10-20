using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (PositionalExistSoundEntry @ 0x17cbd30..0x17cc2a4, Ghidra vaddr).
public class PositionalExistSoundEntry : ExistSoundEntry
{
	public List<Vector3> Positions = new List<Vector3>();

	public PositionalExistSoundEntry(string soundSetName)
		: base(soundSetName)
	{
	}

	public void AddRefAtPos(Vector3 pos)
	{
		if (mCurrentAudioHelper == null)
		{
			mCurrentAudioHelper = FApp.SoundManager.PlaySoundSetAtPoint(mSoundSetName, pos, 0f, 1f, looping: true, selfDestruct: false);
			mCurrentAudioHelper.mPositionalExist = this;
			if (mCurrentAudioHelper != null)
			{
				mCurrentAudioHelper.FadeIn(0.25f, mVolume * mSoundSetVolume * FApp.SoundManager.EffectVolume, 0f);
			}
		}
		Positions.Add(pos);
		mRefCount++;
	}

	public void ReleaseRefAtPos(Vector3 pos)
	{
		for (int i = 0; i < Positions.Count; i++)
		{
			if (FUtil.EpsilonEqual(Positions[i], pos))
			{
				Positions.RemoveAt(i);
				ReleaseRef();
				return;
			}
		}
		FLog.UnityError("PositionalExistSoundEntry.ReleaseRefAtPos: Position not found: " + pos.ToString());
	}
}
