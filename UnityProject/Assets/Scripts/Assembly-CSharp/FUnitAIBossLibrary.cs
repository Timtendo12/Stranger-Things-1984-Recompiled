using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBossLibrary @ 0x173b450..0x173bc8c + iterator to 0x173c4d8, Ghidra vaddr).
public class FUnitAIBossLibrary : FUnitAIBase
{
	protected List<FUnit> mEnemyUnits = new List<FUnit>();

	protected int mTotalHealth;

	protected int mCurrHealth;

	public FUnitAIBossLibrary(FUnit owner)
		: base(owner)
	{
	}

	protected override void Init()
	{
		if (mOwner.Room.State != FRoom.RoomStates.Current)
		{
			return;
		}
		mInitialized = true;
		for (int i = 0; i < mOwner.Room.Units.Count; i++)
		{
			FUnit unit = mOwner.Room.Units[i];
			if (unit.IsTag("EnemyCharacter"))
			{
				mEnemyUnits.Add(unit);
			}
		}
		FGameUtil.DelayExecute(1f, Enter);
		mOwner.World.InputEnabled = false;
	}

	protected void Enter()
	{
		mOwner.World.InputEnabled = false;
		StartEncounter();
	}

	protected void StartEncounter()
	{
		FGameUtil.StartCoroutine(DoStartEncounter());
	}

	protected IEnumerator DoStartEncounter()
	{
		if (mEnemyUnits.Count == 0)
		{
			StartLeave();
			yield break;
		}
		foreach (FUnit boss in mEnemyUnits.Where((FUnit u) => u.IsTag("Boss")))
		{
			mOwner.World.ZoomInToPosition(boss.Position);
			yield return new WaitForSeconds(1.5f);
		}
		mOwner.World.ZoomOut();
		mOwner.World.BeginBossEncounter(mOwner);
		mTotalHealth = mEnemyUnits.Aggregate(mTotalHealth, (int i, FUnit unit) => unit.HP + i);
		mCurrHealth = mTotalHealth;
		FApp.HUD.PageLegendWorld.SetBossHealth(mTotalHealth, mTotalHealth);
		yield return new WaitForSeconds(1f);
		mOwner.World.InputEnabled = true;
	}

	protected void StartLeave()
	{
		FGameUtil.DelayExecute(1f, delegate
		{
			mOwner.World.EndBossEncounter(true);
		});
		FGameUtil.DelayExecute(3f, FinishLeave);
	}

	protected void FinishLeave()
	{
		mOwner.World.ZoomInToPosition(mOwner.Position);
		mOwner.World.RemoveUnit(mOwner);
		mOwner.Dying = true;
		mOwner.DoLoot();
		FGameUtil.DelayExecute(3f, delegate
		{
			mOwner.World.ZoomOut();
			mOwner.World.InputEnabled = true;
		});
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		int num = mEnemyUnits.Count;
		if (num - 1 < 0)
		{
			return;
		}
		int prevHealth = mCurrHealth;
		do
		{
			num--;
			FUnit fUnit = mEnemyUnits[num];
			if (fUnit == null || !fUnit.AliveAndWell)
			{
				mCurrHealth--;
				mEnemyUnits.RemoveAt(num);
			}
		}
		while (0 < num);
		if (prevHealth != mCurrHealth)
		{
			FApp.HUD.PageLegendWorld.SetBossHealth(mCurrHealth, mTotalHealth);
			FApp.HUD.PageLegendWorld.FlashBossHealth();
			if (mEnemyUnits.Count == 0)
			{
				StartLeave();
			}
		}
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (m.SenderID >= 0)
		{
			mOwner.World.GetUnitByID(m.SenderID);
		}
		return false;
	}
}
