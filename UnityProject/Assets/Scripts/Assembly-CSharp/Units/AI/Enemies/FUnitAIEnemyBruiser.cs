using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIEnemyBruiser @ 0x17470dc..0x1747408, Ghidra vaddr).
public class FUnitAIEnemyBruiser : FUnitAIEnemy
{
	public FUnitAIEnemyBruiser(FUnit owner)
		: base(owner)
	{
	}

	public void DoTaunt()
	{
		mOwner.PlayAnim("Taunt");
		StopActions();
		AddAction(new FUnitActionWait(mOwner.World.GetNextActionID(), mOwner, 3f));
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.Message != "Attacked")
		{
			return false;
		}
		if (m.SenderID < 0)
		{
			return true;
		}
		FUnit attacker = mOwner.World.GetUnitByID(m.SenderID);
		if (attacker == null)
		{
			return true;
		}
		SetState(EnemyState.AGGROD);
		if (!GetHasActionOfType<FUnitActionStunned>())
		{
			DoTaunt();
		}
		if (attacker.IsPlayer)
		{
			mOwner.World.SpawnFlyOffText(FLoc.GetString("Immune"), mOwner.Position, new Vector2(50f, 0f));
		}
		if (attacker.Weapon != null && attacker.Weapon.Type == "Melee" && !attacker.ProtoUnit.ImmuneToKnockback)
		{
			attacker.Push(attacker.Back(), 1, true, true, false);
		}
		return true;
	}
}
