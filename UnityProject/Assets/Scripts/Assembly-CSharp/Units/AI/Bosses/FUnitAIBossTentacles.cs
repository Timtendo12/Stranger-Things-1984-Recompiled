using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBossTentacles @ 0x173f8e8..0x1741fe0 + iterators to 0x1742b80, Ghidra vaddr).
public class FUnitAIBossTentacles : FUnitAIBase
{
	protected static readonly string[][] cSpawnSets = new string[4][]
	{
		new string[4] { "VibroGuard", "VibroGuard", "PistolGuard", "PistolGuard" },
		new string[4] { "Trenchcoat", "Trenchcoat", "Wrenchman", "Wrenchman" },
		new string[4] { "Trenchcoat", "VibroGuard2", "Trenchcoat", "VibroGuard2" },
		new string[3] { "Slug", "Slug", "Slug" }
	};

	protected const float cDefaultSpawnInterval = 6f;

	protected const float cSpawnIntervalMultiplier = 0.9f;

	protected const int cTotalStages = 5;

	protected int mBossStage;

	protected bool mEncounterStarted;

	protected float mSpawnInterval = 6f;

	protected float mLastSpawnTime;

	protected FUnit[] mBottomLeftTentacles;

	protected FUnit[] mBottomRightTentacles;

	protected bool mLeftTentaclesNeedReset;

	protected bool mRightTentaclesNeedReset;

	protected Vector3 mZoomPos;

	protected Vector3 mHitPos;

	public FUnitAIBossTentacles(FUnit owner)
		: base(owner)
	{
	}

	protected override void Init()
	{
		if (mOwner.Room.State != FRoom.RoomStates.Current)
		{
			return;
		}
		base.Init();
		mZoomPos = mOwner.Position + new Vector3(1f, 0f, 0f);
		CollectImportantUnits();
		mOwner.AddToTile(mOwner.TilePosition + new FPoint2(3, 0));
		FGameUtil.DelayExecute(1f, Enter);
		mOwner.World.InputEnabled = false;
		mOwner.PlayAnim("Dormant");
	}

	protected void CollectImportantUnits()
	{
		string text = mOwner.StartData.GetObjectProperty<string>("BottomLeft_Tentacle");
		if (text.Valid())
		{
			FUnit[] unitsByName = mOwner.Room.GetUnitsByName(text);
			if (unitsByName != null)
			{
				mBottomLeftTentacles = unitsByName;
			}
		}
		text = mOwner.StartData.GetObjectProperty<string>("BottomRight_Tentacle");
		if (text.Valid())
		{
			FUnit[] unitsByName2 = mOwner.Room.GetUnitsByName(text);
			if (unitsByName2 != null)
			{
				mBottomRightTentacles = unitsByName2;
			}
		}
	}

	protected void Enter()
	{
		mOwner.World.InputEnabled = false;
		FGameUtil.StartCoroutine(DoStartEncounter());
	}

	protected IEnumerator DoStartEncounter()
	{
		mOwner.World.ForceMovePlayerToPoint(mOwner.TilePosition + new FPoint2(0, -5), true, null);
		yield return new WaitForSeconds(2f);
		FWorld.Player.PlayAnim("Hit", false, null, false);
		FWorld.Player.ShowEmote("Surprise", false);
		yield return new WaitForSeconds(0.25f);
		FWorld.Player.SetIdle();
		mOwner.World.ZoomInToPosition(mZoomPos);
		yield return new WaitForSeconds(0.5f);
		mOwner.PlayAnim("Intro", false);
		yield return new WaitForSeconds(1.5f);
		mOwner.World.ZoomOut();
		mOwner.World.BeginBossEncounter(mOwner);
		yield return new WaitForSeconds(1f);
		mOwner.World.InputEnabled = true;
		mEncounterStarted = true;
	}

	protected void StartLeave()
	{
		foreach (FUnit item in new List<FUnit>(mOwner.Room.Units))
		{
			if (item != mOwner && item.IsTag("EnemyCharacter"))
			{
				mOwner.World.RemoveUnit(item);
				mOwner.World.SpawnEffect("PoofEffect", item.Position, 200);
			}
		}
		mOwner.World.EndBossEncounter(true);
		mOwner.PlayAnim("Dying", false);
		mOwner.QueueAnim("Dead", true);
		mOwner.World.UnPause();
		FGameUtil.DelayExecute(2f, FinishLeave);
	}

	protected void FinishLeave()
	{
		mOwner.World.ZoomOut();
		mOwner.DoLoot();
		mOwner.Dying = true;
		mOwner.SetNonInteractable();
		mOwner.World.InputEnabled = true;
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (!mEncounterStarted)
		{
			return;
		}
		if (mLastSpawnTime + mSpawnInterval <= mOwner.World.Gametime)
		{
			SpawnSomethingBad();
		}
		if (mRightTentaclesNeedReset && mBottomRightTentacles != null && mBottomRightTentacles.Length != 0 && mBottomRightTentacles.All((FUnit t) => FWorld.Player.TilePosition.y < t.TilePosition.y))
		{
			for (int i = 0; i < mBottomRightTentacles.Length; i++)
			{
				FUnit fUnit = mBottomRightTentacles[i];
				fUnit.FullReset();
				fUnit.PlayAnim("Stunned_End", false);
			}
			mRightTentaclesNeedReset = false;
		}
		if (mLeftTentaclesNeedReset && mBottomLeftTentacles != null && mBottomLeftTentacles.Length != 0 && mBottomLeftTentacles.All((FUnit t) => FWorld.Player.TilePosition.y < t.TilePosition.y))
		{
			for (int j = 0; j < mBottomLeftTentacles.Length; j++)
			{
				FUnit fUnit2 = mBottomLeftTentacles[j];
				fUnit2.FullReset();
				fUnit2.PlayAnim("Stunned_End", false);
			}
			mLeftTentaclesNeedReset = false;
		}
	}

	protected bool AllSwitchesOn()
	{
		string[] triggerUnits = mOwner.StartData.GetTriggerUnits();
		if (triggerUnits.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < triggerUnits.Length; i++)
		{
			FUnit unitByName = mOwner.World.GetUnitByName(triggerUnits[i]);
			if (unitByName != null && unitByName.AI is FUnitAITrigger fUnitAITrigger && !fUnitAITrigger.On)
			{
				return false;
			}
		}
		return true;
	}

	protected void ActivateAllSwitches()
	{
		string[] triggerUnits = mOwner.StartData.GetTriggerUnits();
		for (int i = 0; i < triggerUnits.Length; i++)
		{
			string triggerUnit = triggerUnits[i];
			FUnit unitByName = mOwner.World.GetUnitByName(triggerUnit);
			if (unitByName == null)
			{
				FProtoUnitStartData fProtoUnitStartData = mOwner.Room.ProtoRoom.ProtoUnitStartDatas.FirstOrDefault((FProtoUnitStartData d) => d.Name == triggerUnit);
				if (fProtoUnitStartData == null)
				{
					FProtoDoorStartData fProtoDoorStartData = mOwner.Room.ProtoRoom.ProtoDoorStartDatas.FirstOrDefault((FProtoDoorStartData d) => d.Name == triggerUnit);
					if (fProtoDoorStartData != null)
					{
						fProtoDoorStartData.CloseOnSpawn = true;
						mOwner.Room.AddDoor(fProtoDoorStartData);
					}
				}
				else
				{
					mOwner.Room.AddUnit(fProtoUnitStartData);
				}
			}
			else if (unitByName.AI is FUnitAITrigger fUnitAITrigger && !fUnitAITrigger.On)
			{
				fUnitAITrigger.DoTrigger();
			}
		}
	}

	protected void SpawnSomethingBad()
	{
		FPoint2 point = mOwner.TilePosition + new FPoint2(FRandom.Generic.RangeInt(-5, 5), FRandom.Generic.RangeInt(-8, -2));
		point = mOwner.Room.ClampPoint(point);
		FTile closestFullyOpenTile = mOwner.Room.GetClosestFullyOpenTile(point);
		if (closestFullyOpenTile == null)
		{
			return;
		}
		int newID = -1;
		if (mOwner.World.SpawnUnitAtPosition("BossEgg", closestFullyOpenTile.Position, out newID) == FRoom.SpawnUnitResult.Success)
		{
			FUnit unitByID = mOwner.World.GetUnitByID(newID);
			if (unitByID != null)
			{
				mOwner.SendMessage(unitByID, "PickEggType", mBossStage.ToString());
			}
		}
		mLastSpawnTime = mOwner.World.Gametime;
		mOwner.PlayAnim("Attack", false);
	}

	protected void SpawnUnit(string unitName, FPoint2 position)
	{
		if (FApp.ProtoUnitManager.GetProtoUnit(unitName) == null)
		{
			return;
		}
		int newID = -1;
		if (mOwner.World.SpawnUnitAtPosition(unitName, position, out newID) != FRoom.SpawnUnitResult.Success)
		{
			return;
		}
		FUnit unitByID = mOwner.World.GetUnitByID(newID);
		if (unitByID == null)
		{
			return;
		}
		mOwner.World.SpawnEffect("PoofEffect", unitByID.Position, 200);
		unitByID.Rename(unitName + Guid.NewGuid().ToString());
		if (unitByID.AI is FUnitAIEnemy fUnitAIEnemy)
		{
			unitByID.SetFacing(FAnimEntry.AnimFacing.UP);
			fUnitAIEnemy.SetState(FUnitAIEnemy.EnemyState.AGGROD);
			fUnitAIEnemy.CanLeash = false;
			// Faithful to the shipped code: the wait action is created for the boss, so AddAction on the enemy rejects it.
			fUnitAIEnemy.AddAction(new FUnitActionWait(mOwner.World.GetNextActionID(), mOwner, 2f));
		}
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (!mEncounterStarted)
		{
			return false;
		}
		if (m.SenderID < 0)
		{
			return false;
		}
		FUnit unitByID = mOwner.World.GetUnitByID(m.SenderID);
		if (unitByID == null)
		{
			return false;
		}
		string message = m.Message;
		if (message == "UnitAddedToTile")
		{
			if (!unitByID.IsTag("Emitted"))
			{
				return true;
			}
			NextBossStage();
			mHitPos = unitByID.Position;
			return true;
		}
		if (message != "Attacked")
		{
			return false;
		}
		mOwner.World.SpawnFlyOffText(FLoc.GetString("Immune"), mOwner.Position, new Vector2(0f, 50f));
		FApp.SoundManager.PlaySoundSet("BossTentacles_Hit");
		return true;
	}

	protected void InitBottomLeft(bool show)
	{
		mLeftTentaclesNeedReset = true;
	}

	protected void InitBottomRight(bool show)
	{
		mRightTentaclesNeedReset = true;
	}

	protected void SetupBossStage()
	{
		List<FUnit> list = new List<FUnit>(mOwner.Room.Units);
		for (int i = 0; i < list.Count; i++)
		{
			FUnit fUnit = list[i];
			string name = fUnit.ProtoUnit.Name;
			if (name == "AlienEgg")
			{
				fUnit.FullReset();
				mOwner.World.SpawnEffect("PoofEffect", fUnit.Position, 200);
			}
			else if (name == "PressurePlate")
			{
				FTile tile = mOwner.World.GetTile(fUnit.TilePosition);
				if (tile != null)
				{
					foreach (FUnit item in new List<FUnit>(tile.Units))
					{
						if (item.IsTag("EnemyCharacter"))
						{
							item.Die();
						}
					}
				}
			}
			else if (name == "BossEgg")
			{
				mOwner.World.RemoveUnit(fUnit);
				mOwner.World.SpawnEffect("PoofEffect", fUnit.Position, 200);
			}
		}
		mLeftTentaclesNeedReset = true;
		mRightTentaclesNeedReset = true;
		if (mBossStage >= 1 && mBossStage <= 4)
		{
			string[] set = cSpawnSets[mBossStage - 1];
			for (int j = 0; j < set.Length; j++)
			{
				FRoom room = mOwner.Room;
				FPoint2 point = new FPoint2(room.X + room.Width / 2, room.Y);
				FTile closestFullyOpenTile = room.GetClosestFullyOpenTile(point);
				SpawnUnit(set[j], closestFullyOpenTile.Position);
			}
			for (int k = 3; k > 0; k--)
			{
				FPoint2 randomPoint = mOwner.Room.GetRandomPoint();
				int y = mOwner.TilePosition.y;
				int num = y - 2;
				int num2 = y - 6;
				int num3 = randomPoint.y;
				if (num <= num3)
				{
					num3 = num;
				}
				if (num2 <= randomPoint.y)
				{
					num2 = num3;
				}
				FTile closestFullyOpenTile2 = mOwner.Room.GetClosestFullyOpenTile(new FPoint2(randomPoint.x, num2));
				SpawnUnit("Tentacle", closestFullyOpenTile2.Position);
			}
		}
		if ((mBossStage & 1) == 0)
		{
			mRightTentaclesNeedReset = true;
		}
		else
		{
			mLeftTentaclesNeedReset = true;
		}
		ActivateAllSwitches();
	}

	protected void NextBossStage()
	{
		mSpawnInterval *= 0.9f;
		mBossStage++;
		mOwner.World.InputEnabled = false;
		mEncounterStarted = false;
		FGameUtil.StartCoroutine(DoBossHit());
	}

	protected IEnumerator DoExplosions()
	{
		int index = 0;
		while (true)
		{
			float x = FRandom.Generic.RangeFloat(-0.5f, 0.5f);
			float y = FRandom.Generic.RangeFloat(-0.5f, 0.5f);
			mOwner.World.SpawnEffect("SmallExplosion", new Vector2(x + mHitPos.x, y + mHitPos.y), 200);
			yield return new WaitForSeconds(0.15f);
			if (index > 5)
			{
				FApp.SoundManager.PlaySoundSet("BossVan_Explode");
				index = 0;
			}
			index++;
		}
	}

	protected IEnumerator DoBossHit()
	{
		mOwner.World.ZoomInToPosition(mZoomPos);
		mOwner.PlayAnim("Hit");
		Coroutine explosionsRoutine = FGameUtil.StartCoroutine(DoExplosions());
		yield return new WaitForSeconds(0.1f);
		mOwner.World.Pause(true, false);
		int healthRemaining = 5 - mBossStage;
		FApp.HUD.PageLegendWorld.SetBossHealth(healthRemaining, 5);
		FApp.HUD.PageLegendWorld.FlashBossHealth();
		yield return new WaitForSeconds(2.9f);
		FGameUtil.StopCoroutine(explosionsRoutine);
		SetupBossStage();
		if (healthRemaining > 0)
		{
			FRoom room = mOwner.Room;
			mOwner.World.ZoomInToPosition(new Vector3(room.X + room.Width / 2, room.Y, 0f));
			yield return new WaitForSeconds(2f);
			if (FWorld.Player.TilePosition.y == mOwner.TilePosition.y)
			{
				FTile closestFullyOpenTile = mOwner.Room.GetClosestFullyOpenTile(FWorld.Player.TilePosition - new FPoint2(0, 1));
				FWorld.Player.SetTilePosition(closestFullyOpenTile.Position, true);
			}
			mOwner.World.ZoomOut();
			mOwner.SetIdle();
			mOwner.World.InputEnabled = true;
			mLastSpawnTime = mOwner.World.Gametime;
			mEncounterStarted = true;
			mOwner.World.UnPause();
		}
		else
		{
			StartLeave();
		}
	}

	public override string GetAnimName(string anim)
	{
		int num = mBossStage + 1;
		if (anim == "Intro" || anim == "Attack" || anim == "Idle" || anim == "Hit")
		{
			anim = anim + "0" + num;
		}
		return anim;
	}
}
