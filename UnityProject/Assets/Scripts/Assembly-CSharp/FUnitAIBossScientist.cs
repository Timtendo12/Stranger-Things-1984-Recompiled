using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBossScientist @ 0x173c520..0x173f3bc + iterators to 0x173f8a0, Ghidra vaddr).
public class FUnitAIBossScientist : FUnitAIEnemy
{
	protected int mBossStage;

	protected float mTimeSinceLastThrow;

	protected bool mEncounterStarted;

	protected float mPoolDeathTime = 1f;

	protected float mTimeBetweenThrows;

	protected int mLastHP;

	protected Coroutine mThrowVialRoutine;

	protected FPoint2 mTargetPoint = FPoint2.invalid;

	protected int mNumHitsWithLucas;

	protected float mSpawnY;

	public FUnitAIBossScientist(FUnit owner)
		: base(owner)
	{
		mShouldAggro = false;
		mLastHP = owner.HP;
		mSpawnY = owner.StartData.Position.y;
	}

	protected override void Init()
	{
		if (mOwner.Room.State == FRoom.RoomStates.Current)
		{
			base.Init();
			FGameUtil.DelayExecute(1f, StartEnter);
			FGameUtil.ExecuteAtEndOfFrame(delegate
			{
				mOwner.GO.transform.position = mOwner.GO.transform.position + Vector3.up * 5f;
			});
			mOwner.World.InputEnabled = false;
		}
	}

	protected void StartEnter()
	{
		mOwner.World.InputEnabled = false;
		mOwner.RemoveAnimationLayers();
		mOwner.SetObstructs(false);
		mOwner.PlayAnim("Jump_Loop");
		Go.killAllAbstractTweensWithTarget(mOwner.GO.transform);
		Vector3 position = mOwner.Position;
		FPoint2 tile = mOwner.TilePosition;
		Go.to(mOwner.GO.transform, 0.25f, new GoTweenConfig().position(new Vector3(tile.x, tile.y, position.z)).setEaseType(GoEaseType.Linear).onComplete(delegate
		{
			FinishEnter();
		}).setUpdateType(GoUpdateType.FWorldUpdate));
	}

	protected void FinishEnter()
	{
		mOwner.World.ZoomInToPosition(mOwner.Position);
		mOwner.SetObstructs(true);
		mOwner.PlayAnim("Jump_Land", false);
		mOwner.QueueAnim("Taunt", false);
		mOwner.QueueAnim("Remote_Start", false);
		mOwner.QueueAnim("Remote_Loop", false);
		mOwner.QueueAnim("Remote_End", false);
	}

	protected void StartLeave()
	{
		mOwner.SetSortingOrder(100);
		mOwner.PlayAnim("HeadShake", false);
		if (mOwner.ProtoUnit.Name == "BossScientist2")
		{
			mOwner.PlayAnim("Death", false);
			return;
		}
		mOwner.QueueAnim("Jump_Start", false);
		mOwner.QueueAnim("Jump_Loop", true);
	}

	protected void FinishLeave()
	{
		if (mOwner.ProtoUnit.Name == "BossScientist2")
		{
			mOwner.World.SpawnEffect("DeathPoofEffect", mOwner.Position, 200);
		}
		mOwner.World.ZoomOut();
		mOwner.DoLoot();
		mOwner.World.RemoveUnit(mOwner);
		mOwner.World.InputEnabled = true;
	}

	protected void StartEncounter()
	{
		FGameUtil.StartCoroutine(DoStartEncounter());
	}

	protected IEnumerator DoStartEncounter()
	{
		if (mBossStage > 0)
		{
			mOwner.World.SpawnEffect("ForcefieldHit", mOwner, Vector2.zero, 200);
			mOwner.AddAnimationLayer("BossScientist", "ForceField_Back", false, Vector2.zero, 1f, true);
			mOwner.AddAnimationLayer("BossScientist", "ForceField_Front", true, Vector2.zero, 1f, true);
			mOwner.QueueAnim("Enrage", true);
		}
		ActivateAllSwitches();
		FWorld.Player.SetObstructs(true);
		yield return new WaitForSeconds(1f);
		mOwner.World.ZoomOut();
		mOwner.World.BeginBossEncounter(mOwner);
		yield return new WaitForSeconds(1f);
		mOwner.World.InputEnabled = true;
		mEncounterStarted = true;
		mTimeSinceLastThrow = mTimeBetweenThrows;
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

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (mEncounterStarted)
		{
			Vector3 position = mOwner.Position;
			if (!Mathf.Approximately(mSpawnY, position.y))
			{
				Vector3 p = mOwner.GO.transform.position;
				mOwner.GO.transform.position = new Vector3(p.x, mSpawnY, p.z);
			}
			mTimeSinceLastThrow += deltaTime;
		}
	}

	protected override void AddIdleAction(float deltaTime)
	{
		if (!mEncounterStarted)
		{
			return;
		}
		if (mBossStage > 1 && mOwner.IsOnScreen && mTimeBetweenThrows <= mTimeSinceLastThrow && mOwner.CanSee(FWorld.Player, true))
		{
			mTimeSinceLastThrow = 0f;
			ThrowVial();
			return;
		}
		FPoint2 position = mOwner.StartData.Position;
		int dx = FRandom.Generic.RangeInt(-3, 4);
		DoMoveToPoint(new FPoint2(position.x + dx, position.y), 0, true, false, false, "Walk", -1f);
	}

	protected void ThrowVial()
	{
		mTargetPoint = FWorld.Player.TilePosition;
		StopActions();
		mOwner.PlayAnim("Throw_WindUp", false);
		mOwner.QueueAnim("Throw_WindUp_Loop", false);
		mOwner.QueueAnim("Throw", false);
		AddAction(new FUnitActionWait(mOwner.World.GetNextActionID(), mOwner, 0.7f));
	}

	protected void DoTaunt()
	{
		StopActions();
		mOwner.PlayAnim("Taunt", false);
		AddAction(new FUnitActionWait(mOwner.World.GetNextActionID(), mOwner, 1f));
	}

	protected void SpawnVial()
	{
		int newID = -1;
		if (mOwner.World.SpawnUnitAtPosition("ScienceVial", mOwner.TilePosition, out newID) == FRoom.SpawnUnitResult.Success && newID >= 0)
		{
			FUnit unitByID = mOwner.World.GetUnitByID(newID);
			if (unitByID != null)
			{
				unitByID.AI.DoMoveToPoint(mTargetPoint, 0, false, false, false, string.Empty, -1f, null, SpawnSciencePool);
			}
		}
	}

	protected void SpawnSciencePool(FUnit vial)
	{
		if (FWorld.Player.TilePosition == vial.TilePosition)
		{
			FWorld.Player.OnAttacked(vial, 1, Vector2.down, 1, true);
		}
		vial.PlayAnim("Impact");
		if (FRandom.Generic.RangeInt(0, 10) < 3)
		{
			vial.SetAlive(false);
		}
		else
		{
			FGameUtil.DelayExecute(vial.GetCurrentAnimLength() * 0.8f, delegate
			{
				SpawnSciencePoolImmediate(vial.TilePosition);
			});
		}
	}

	protected void SpawnSciencePoolImmediate(FPoint2 pos)
	{
		int newID = -1;
		if (mOwner.World.SpawnUnitAtPosition("SciencePool", pos, out newID) == FRoom.SpawnUnitResult.Success && newID >= 0)
		{
			FUnit pool = mOwner.World.GetUnitByID(newID);
			if (pool != null)
			{
				FGameUtil.DelayExecute(mPoolDeathTime, delegate
				{
					mOwner.World.RemoveUnit(pool);
				});
			}
		}
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		string message = m.Message;
		if (message == "Attacked")
		{
			if (!mEncounterStarted)
			{
				return false;
			}
			if (mBossStage < 2)
			{
				if (mThrowVialRoutine != null)
				{
					FGameUtil.StopCoroutine(mThrowVialRoutine);
				}
				mThrowVialRoutine = FGameUtil.StartCoroutine(DoThrowVial());
			}
			FUnit unitByID = mOwner.World.GetUnitByID(m.SenderID);
			bool result = false;
			if (unitByID != null && mBossStage > 0 && unitByID.Weapon.Type == "Ranged")
			{
				DoTaunt();
				DeflectProjectile(unitByID.Weapon);
				mNumHitsWithLucas++;
				if (mNumHitsWithLucas > 4)
				{
					mOwner.World.TryShowDialogue("", "SwitchToHopper");
					mNumHitsWithLucas = 0;
				}
				result = true;
			}
			FGameUtil.ExecuteAtEndOfFrame(CheckHP);
			return result;
		}
		if (message == "SpawnVial")
		{
			FGameUtil.ExecuteAtEndOfFrame(SpawnVial);
		}
		else if (message == "StartEncounter")
		{
			StartEncounter();
		}
		else if (message == "JumpOffscreen")
		{
			Transform transform = mOwner.GO.transform;
			Go.to(transform, 0.5f, new GoTweenConfig().position(mOwner.GO.transform.position + Vector3.up * 5f).setEaseType(GoEaseType.Linear).onComplete(delegate
			{
				FinishLeave();
			}).setUpdateType(GoUpdateType.FWorldUpdate));
			Transform shadow = mOwner.GO.transform.Find("Shadow");
			if (shadow != null)
			{
				shadow.parent = null;
				Go.to(shadow, 0.5f, new GoTweenConfig().position(shadow.position + Vector3.down * 0.4f).setEaseType(GoEaseType.CircIn).setUpdateType(GoUpdateType.FWorldUpdate));
				Go.to(shadow, 0.5f, new GoTweenConfig().scale(0f).setEaseType(GoEaseType.CircIn).onComplete(delegate
				{
					UnityEngine.Object.Destroy(shadow.gameObject);
				}).setUpdateType(GoUpdateType.FWorldUpdate));
			}
			return true;
		}
		else if (message == "DeathComplete")
		{
			FinishLeave();
		}
		return false;
	}

	protected void DeflectProjectile(FProtoWeapon weapon)
	{
		mOwner.World.SpawnEffect("ForcefieldHit", mOwner, Vector2.zero, 200);
		GameObject prefab = FUtil.LoadPrefab("Prefabs/Weapons/" + weapon.PrefabName);
		if (prefab == null)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		FWeaponObj component = gameObject.GetComponent<FWeaponObj>();
		component.Owner = mOwner;
		component.ProtoWeapon = weapon;
		gameObject.transform.position = mOwner.GO.transform.position;
		float x = (FRandom.Generic.RandBool() ? 1f : (-1f));
		float y = FRandom.Generic.RangeFloat(0f, 0.3f);
		Vector2 dir = new Vector2(x, y).normalized;
		component.MoveVector = dir * (float)weapon.Speed;
		component.transform.right = dir;
	}

	protected IEnumerator DoThrowVial()
	{
		yield return new WaitForSeconds((mBossStage != 0) ? 1f : 0.5f);
		ThrowVial();
		mThrowVialRoutine = null;
	}

	protected void CheckHP()
	{
		if (mLastHP == mOwner.HP)
		{
			mOwner.World.SpawnFlyOffText(FLoc.GetString("Deflect"), mOwner.Position, new Vector2(0f, 50f));
		}
		else
		{
			FApp.HUD.PageLegendWorld.SetBossHealth(mOwner.HP - 1, mOwner.ProtoUnit.Health - 1);
			FApp.HUD.PageLegendWorld.FlashBossHealth();
			mLastHP = mOwner.HP;
			mOwner.PlayAnim("Hit", false);
		}
		int hP = mOwner.HP;
		if (hP < 12 && (mBossStage == 0 || (hP < 7 && (mBossStage == 1 || (hP < 2 && mBossStage == 2)))))
		{
			NextBossStage();
		}
	}

	protected void NextBossStage()
	{
		mOwner.World.InputEnabled = false;
		mEncounterStarted = false;
		StopActions();
		if (mThrowVialRoutine != null)
		{
			FGameUtil.StopCoroutine(mThrowVialRoutine);
			mThrowVialRoutine = null;
		}
		mOwner.World.ZoomInToPosition(mOwner.Position);
		int bossStage = mBossStage;
		mBossStage = bossStage + 1;
		switch (bossStage)
		{
		case 2:
			mOwner.World.SpawnEffect("PoofEffect", mOwner.Position, 200);
			mOwner.World.SpawnEffect("ForcefieldHit", mOwner, Vector2.zero, 200);
			mOwner.RemoveAnimationLayers();
			mOwner.PlayAnim("BeatUp");
			mOwner.World.EndBossEncounter(true);
			FGameUtil.DelayExecute(3f, StartLeave);
			break;
		case 1:
			mOwner.World.SpawnEffect("PoofEffect", mOwner.Position, 200);
			mOwner.World.SpawnEffect("ForcefieldHit", mOwner, Vector2.zero, 200);
			mOwner.RemoveAnimationLayers();
			mPoolDeathTime = 3.5f;
			PlayStageSwitchAnims();
			break;
		case 0:
			PlayStageSwitchAnims(true);
			break;
		}
	}

	protected void PlayStageSwitchAnims(bool forceButtonPress = false)
	{
		if (mOwner.TilePosition.y - 1 <= FWorld.Player.TilePosition.y)
		{
			mOwner.PlayAnim(GetAnimName("Push"), false);
			FWorld.Player.SetObstructs(false);
			int numTiles = ~mOwner.Room.Y + FWorld.Player.TilePosition.y;
			FGameUtil.DelayExecute(0.5f, delegate
			{
				FWorld.Player.Push(Vector2.down, numTiles);
			});
		}
		if (AllSwitchesOn() && !forceButtonPress)
		{
			StartEncounter();
			return;
		}
		mOwner.QueueAnim("Remote_Start", false);
		mOwner.QueueAnim("Remote_Loop", false);
		mOwner.QueueAnim("Remote_End", false);
	}

	public override string GetAnimName(string anim)
	{
		switch (anim)
		{
		case "Throw":
		case "Walk":
		case "Remote_Loop":
		case "Remote_End":
		case "Remote_Start":
		case "Enrage":
		case "Throw_WindUp_Loop":
		case "Throw_WindUp":
		case "Push":
		case "Hit":
			return anim + "0" + mBossStage;
		default:
			return anim;
		}
	}
}
