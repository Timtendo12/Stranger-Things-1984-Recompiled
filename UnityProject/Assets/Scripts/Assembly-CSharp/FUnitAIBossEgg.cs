using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBossEgg @ 0x17320cc..0x17330c8, Ghidra vaddr).
public class FUnitAIBossEgg : FUnitAIBase
{
	public enum EggTypes
	{
		EXPLOSIVE = 0,
		SPAWN_TENTACLE = 1,
		SPAWN_SLUG = 2
	}

	protected const float cFallTime = 1f;

	protected const int cMaxSlugs = 5;

	protected const int cMaxTentacles = 10;

	public EggTypes EggType;

	public FUnitAIBossEgg(FUnit owner)
		: base(owner)
	{
		mResetTime = 1f;
	}

	protected override void Init()
	{
		mInitialized = true;
		mOwner.SetObstructs(false);
		Transform vis = mOwner.GO.transform.Find("Vis");
		if (vis != null)
		{
			Go.killAllAbstractTweensWithTarget(vis);
			vis.localPosition = vis.localPosition + Vector3.up * 8f;
			Go.to(vis, 1f, new GoTweenConfig().localPosition(Vector3.zero).setEaseType(GoEaseType.Linear).onComplete(delegate
			{
				DropComplete();
			}).setUpdateType(GoUpdateType.FWorldUpdate));
		}
		Transform shadow = mOwner.GO.transform.Find("Shadow");
		if (shadow != null)
		{
			Go.killAllAbstractTweensWithTarget(shadow);
			shadow.localScale = Vector3.zero;
			Go.to(shadow, 1f, new GoTweenConfig().scale(1f).setEaseType(GoEaseType.CircOut).setUpdateType(GoUpdateType.FWorldUpdate));
		}
	}

	protected void PickEggType(int bossStage)
	{
		// 0x1732524 (32 bytes): the shipped method ignores bossStage and always picks the explosive egg (EggType = 0).
		EggType = EggTypes.EXPLOSIVE;
		mOwner.SetIdle();
	}

	protected override void Reset()
	{
		Explode();
	}

	protected void DropComplete()
	{
		FApp.SoundManager.PlaySoundSet("BossTentacles_EggFall");
		mOwner.SetObstructs(true);
		mOwner.SyncTilePosition();
		switch (EggType)
		{
		case EggTypes.SPAWN_SLUG:
			SpawnUnit("Slug");
			break;
		case EggTypes.SPAWN_TENTACLE:
			SpawnUnit("Tentacle");
			break;
		case EggTypes.EXPLOSIVE:
			RegisterInteraction();
			break;
		}
	}

	protected void SpawnUnit(string unitName)
	{
		mOwner.SetObstructs(false);
		FProtoUnit proto = FApp.ProtoUnitManager.GetProtoUnit(unitName);
		if (proto == null)
		{
			return;
		}
		int newID = -1;
		if (mOwner.World.SpawnUnitAtPosition(unitName, mOwner.TilePosition, out newID) == FRoom.SpawnUnitResult.Success)
		{
			mOwner.World.SpawnEffect("PoofEffect", mOwner.Position, 200);
			FUnit spawned = mOwner.World.GetUnitByID(newID);
			if (spawned != null)
			{
				spawned.Rename(unitName + System.Guid.NewGuid().ToString());
				if (spawned.AI is FUnitAIEnemy enemyAI)
				{
					enemyAI.SetState(FUnitAIEnemy.EnemyState.AGGROD);
				}
			}
		}
		mOwner.World.RemoveUnit(mOwner);
	}

	protected void Explode()
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
		for (int j = 0; j < destructibles.Count; j++)
		{
			FUnit[] named = mOwner.World.GetUnitsByName(destructibles[j]);
			if (named.All((FUnit u) => !u.IsPlayingAnim("Idle")))
			{
				for (int k = 0; k < named.Length; k++)
				{
					mOwner.SendMessage(named[k], "DeadFoLife");
				}
			}
		}
		mOwner.PlayAnim("Explosion", false);
		mOwner.DoLoot();
		mOwner.World.RemoveUnit(mOwner, mOwner.GetCurrentAnimLength());
	}

	protected void DoReset()
	{
		base.Reset();
		mOwner.PlayAnim("Spawn", false);
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (m.SenderID >= 0)
		{
			string message = m.Message;
			if (message != "Attacked" && message == "PickEggType")
			{
				int stage = -1;
				if (int.TryParse(m.Data, out stage))
				{
					PickEggType(stage);
				}
			}
		}
		return false;
	}

	public override string GetAnimName(string anim)
	{
		int index = ((EggType == EggTypes.EXPLOSIVE) ? 3 : 1);
		if (anim == "Idle")
		{
			anim = anim + "0" + index.ToString();
		}
		return anim;
	}
}
