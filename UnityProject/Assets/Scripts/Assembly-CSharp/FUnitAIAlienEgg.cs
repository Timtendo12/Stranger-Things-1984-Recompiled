using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIAlienEgg @ 0x172c2a0..0x172cf70, Ghidra vaddr).
public class FUnitAIAlienEgg : FUnitAIBase
{
	protected const int cHitsNeeded = 3;

	protected int mHitsNeeded = 3;

	public FUnitAIAlienEgg(FUnit owner)
		: base(owner)
	{
		mResetTime = 1f;
	}

	public override void Update(float deltaTime)
	{
		if (mActions.Count == 0 && mHitsNeeded == 0)
		{
			mHitsNeeded = -1;
			RegisterInteraction();
		}
		base.Update(deltaTime);
	}

	protected override void Reset()
	{
		List<FUnit> units = new List<FUnit>();
		Vector3 ownerPos = mOwner.Position;
		int count = mOwner.World.GetUnitsByCircleCast(new Vector2(ownerPos.x, ownerPos.y), 1.5f, units);
		List<string> destructibles = new List<string>();
		for (int i = 0; i < count; i++)
		{
			FUnit unit = units[i];
			if (unit == mOwner)
			{
				continue;
			}
			if (!unit.IsTag("Destructible"))
			{
				Vector3 pos = unit.Position;
				Vector3 me = mOwner.Position;
				unit.OnAttacked(mOwner, new Vector2(pos.x - me.x, pos.y - me.y));
			}
			else
			{
				if (!destructibles.Contains(unit.Name))
				{
					destructibles.Add(unit.Name);
				}
				unit.PlayAnim("Stunned_Start", false);
				unit.QueueAnim("Stunned_Idle", false);
				unit.QueueAnim("Stunned_End", false);
			}
		}
		bool allDone;
		if (destructibles.Count < 1)
		{
			allDone = false;
		}
		else
		{
			allDone = true;
			for (int j = 0; j < destructibles.Count; j++)
			{
				FUnit[] named = mOwner.World.GetUnitsByName(destructibles[j]);
				if (!named.All((FUnit u) => !u.IsPlayingAnim("Idle")))
				{
					allDone = false;
				}
				else
				{
					for (int k = 0; k < named.Length; k++)
					{
						mOwner.SendMessage(named[k], "DeadFoLife");
					}
				}
			}
		}
		mOwner.PlayAnim("Explosion", false);
		if (allDone && !mOwner.Room.IsBossRoom())
		{
			mOwner.World.PlaySecretGet();
		}
		FGameUtil.DelayExecute(mOwner.GetCurrentAnimLength(), DoReset);
	}

	protected void DoReset()
	{
		base.Reset();
		mOwner.PlayAnim("Spawn", false);
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (m.SenderID >= 0 && m.Message == "Attacked" && mHitsNeeded >= 0)
		{
			mHitsNeeded = ((mHitsNeeded != 0) ? (mHitsNeeded - 1) : 0);
		}
		return false;
	}

	public override string GetAnimName(string anim)
	{
		int hits = mHitsNeeded;
		if (hits < 2)
		{
			hits = 1;
		}
		hits = 4 - hits;
		if (anim == "Idle")
		{
			anim = anim + "0" + hits.ToString();
		}
		return anim;
	}
}
