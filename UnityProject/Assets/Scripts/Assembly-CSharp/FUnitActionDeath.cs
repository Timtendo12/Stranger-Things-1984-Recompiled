using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitActionDeath @ 0x1723718..0x1729144, Ghidra vaddr).
public class FUnitActionDeath : FUnitActionWork
{
	protected bool mDed;

	public FUnitActionDeath(int newID, FUnit owner)
		: base(newID, owner, owner)
	{
	}

	public override void DoWork()
	{
		if (mDed)
		{
			return;
		}
		if (mOwner.ProtoUnit.Health != 0 && !mOwner.Alive)
		{
			return;
		}
		mOwner.Alive = false;
		float delay;
		if (!mOwner.PlayAnim("Death", false))
		{
			string effect = "PoofEffect";
			if (mOwner.ProtoUnit.IsTag("EnemyCharacter"))
			{
				effect = "DeathPoofEffect";
				FApp.SoundManager.PlaySoundSet("PistolGuard_Death");
			}
			mOwner.World.SpawnEffect(effect, mOwner.Position, 200);
			delay = 0f;
		}
		else
		{
			delay = mOwner.GetCurrentAnimLength();
			if (FApp.World.GetUnitByID(mOwner.ID) != null)
			{
				mOwner.SetObstructs(false);
				Transform shadow = mOwner.GO.transform.Find("Shadow");
				if (shadow != null)
				{
					shadow.gameObject.SetActive(false);
				}
			}
		}
		mOwner.World.RemoveUnit(mOwner, delay);
		mDed = true;
	}

	public override void Stop()
	{
		DoWork();
	}
}
