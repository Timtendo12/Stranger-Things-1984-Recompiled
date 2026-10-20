using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIShieldGuard @ 0x174f034..0x174f98c, Ghidra vaddr).
public class FUnitAIShieldGuard : FUnitAIEnemy
{
	protected int mShieldHealth = 1;

	public FUnitAIShieldGuard(FUnit owner)
		: base(owner)
	{
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		bool handled = base.ProcessMessage(m);
		if (!handled)
		{
			if (m.SenderID < 0)
			{
				return handled;
			}
			FUnit attacker = mOwner.World.GetUnitByID(m.SenderID);
			if (attacker == null || m.Message != "Attacked")
			{
				return handled;
			}
			if (mShieldHealth < 1)
			{
				mOwner.MaxDamageTakenPerHit = -1;
				return handled;
			}
			mOwner.MaxDamageTakenPerHit = 0;
			mOwner.World.SpawnFlyOffText(FLoc.GetString("Blocked"), mOwner.Position, new Vector2(50f, 0f));
			if (FApp.PlayerStats.CurrentCharacter == "Nancy")
			{
				mShieldHealth--;
			}
			if (mShieldHealth > 0)
			{
				return handled;
			}
			mOwner.World.SpawnEffect("PoofEffect", mOwner.Position, 200);
			mOwner.PlayAnim("Hit", false);
			BounceAwayUnit("ShieldGuardShield");
		}
		return handled;
	}

	protected void BounceAwayUnit(string unitName)
	{
		int newID = -1;
		if (mOwner.World.SpawnUnitAtPosition(unitName, mOwner.TilePosition, out newID) != FRoom.SpawnUnitResult.Success || newID < 0)
		{
			return;
		}
		FUnit unit = mOwner.World.GetUnitByID(newID);
		if (unit == null)
		{
			return;
		}
		unit.GO.transform.position = mOwner.Position + Vector3.up * 0.5f;
		float sideways = (FRandom.Generic.RandBool() ? (-2f) : 2f);
		Go.to(unit.GO.transform, 1f, new GoTweenConfig().vector3XProp("localPosition", sideways, true).setEaseType(GoEaseType.QuadOut));
		Go.to(unit.GO.transform, 1f, new GoTweenConfig().vector3YProp("position", -0.5f, true).setEaseType(GoEaseType.BounceOut).onComplete(delegate
		{
			PoofAwayUnit(unit);
		}));
	}

	protected void PoofAwayUnit(FUnit unit)
	{
		unit.SyncTilePosition();
		mOwner.World.SpawnEffect("PoofEffect", unit.Position, 200);
		unit.Die();
	}

	public override string GetAnimName(string anim)
	{
		int suffix = ((mShieldHealth >= 1) ? 1 : 2);
		switch (anim)
		{
		case "Walk":
		case "ChargeWindUp":
		case "Idle":
		case "Stun":
		case "Attack":
		case "Hit":
		case "Charge":
		case "Transition":
			return anim + "0" + suffix.ToString();
		default:
			return anim;
		}
	}
}
