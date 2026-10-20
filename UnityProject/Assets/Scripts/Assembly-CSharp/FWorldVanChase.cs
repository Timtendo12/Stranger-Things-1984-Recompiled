using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FWorldVanChase @ 0x176a4e4..0x176e5e0 + iterators to 0x1770540, Ghidra vaddr).
public class FWorldVanChase : FWorld
{
	protected const float cTimeBetweenLaneChecksAfterPlayerMove = 1f;

	protected const float cDefaultTimeBetweenLaneChecks = 0.5f;

	protected const float cDefaultMoveSpeed = 7f;

	protected const float cDefaultTimeBetweenVanEvents = 5f;

	protected float mTimeBetweenLaneChecks = 0.5f;

	protected List<Transform> mWorldSprites = new List<Transform>();

	protected Vector3[] mLanes = new Vector3[3];

	protected FAnimationController[] mLaneWarningBubbles = new FAnimationController[3];

	protected List<FUnit> mObstacles = new List<FUnit>();

	protected float mCameraHalfWidth;

	protected float mCameraHalfHeight;

	protected bool mSwitchedThisDrag;

	protected string[] mTopObstaclesProtoUnits = new string[0];

	protected string[] mMiddleObstaclesProtoUnits = new string[0];

	protected string[] mBottomObstaclesProtoUnits = new string[0];

	protected float mNextObstacleSpawn;

	protected int mTimeLimit;

	protected float mTimeRemaining;

	protected float mNextVanEventCheck;

	protected List<string> mPossibleVanEvents = new List<string>();

	protected Coroutine mVanEventRoutine;

	protected bool mRaceComplete;

	protected FBikeContestant mPlayerContestant;

	protected FBikeContestant mVanContestant;

	protected FBikeContestant[] mContestants;

	public new static IEnumerator Init(string worldName, FPlayerStartParams startParams = null)
	{
		if (FApp.WorldManager.ProtoWorlds == null)
		{
			yield break;
		}
		FProtoWorld worldToUse = null;
		foreach (KeyValuePair<string, FProtoWorld> protoWorld in FApp.WorldManager.ProtoWorlds)
		{
			if (protoWorld.Key.Equals(worldName, StringComparison.OrdinalIgnoreCase))
			{
				worldToUse = protoWorld.Value;
				break;
			}
		}
		if (worldToUse == null)
		{
			yield break;
		}
		GameObject gameObject = new GameObject("LegendWorld_VanChase");
		FWorldVanChase world = gameObject.AddComponent<FWorldVanChase>();
		FApp.World = world;
		world.ProtoWorld = worldToUse;
		world.mPather = new FPather(world);
		world.mPather.SetStopRange(0);
		world.LoadMap(startParams);
		yield return world.StartCoroutine(world.PostSceneLoad());
	}

	public override void Start()
	{
		mFlashShader = Shader.Find("GUI/Text Shader");
		mDefaultShader = Shader.Find("Sprites/Default");
		Pause(true, false);
		mNextObstacleSpawn = -1f;
	}

	protected override void LoadSpecialUnits(FPoint2 playerstart)
	{
		base.LoadSpecialUnits(playerstart);
		for (int i = 0; i < mLanes.Length; i++)
		{
			mLanes[i] = FWorld.Player.Position + Vector3.down * ((float)i * 1.3f);
		}
		for (int j = 0; j < mLaneWarningBubbles.Length; j++)
		{
			FAnimationController fAnimationController = SpawnDiscreteAnimation("WarningBubble", 200, "Play");
			mLaneWarningBubbles[j] = fAnimationController;
			fAnimationController.GO.SetActive(false);
			fAnimationController.GO.transform.position = mLanes[j];
		}
		mTopObstaclesProtoUnits = FApp.ProtoUnitManager.GetProtoUnitsWithTag("BikeRaceObstacle_Top").Select((FProtoUnit p) => p.Name).ToArray();
		mMiddleObstaclesProtoUnits = FApp.ProtoUnitManager.GetProtoUnitsWithTag("BikeRaceObstacle_Middle").Select((FProtoUnit p) => p.Name).ToArray();
		mBottomObstaclesProtoUnits = FApp.ProtoUnitManager.GetProtoUnitsWithTag("BikeRaceObstacle_Bottom").Select((FProtoUnit p) => p.Name).ToArray();
		if (ProtoWorld.LoadData.ContainsKey("PlayerCharacter"))
		{
			FWorld.Player.ChangeProto(FApp.ProtoUnitManager.GetProtoUnit(ProtoWorld.LoadData["PlayerCharacter"], true), true);
		}
		FWorld.Player.SetFacing(FAnimEntry.AnimFacing.RIGHT);
		FWorld.Player.SetIdle("Walk", true, true);
		mPlayerContestant = new FBikeContestant();
		mPlayerContestant.Unit = FWorld.Player;
		mPlayerContestant.MoveSpeed = 7.2f;
		string protoUnit = "BikerDude";
		if (ProtoWorld.LoadData.ContainsKey("OpponentProtoUnit"))
		{
			protoUnit = ProtoWorld.LoadData["OpponentProtoUnit"];
		}
		FProtoUnitStartData fProtoUnitStartData = new FProtoUnitStartData();
		fProtoUnitStartData.Name = "Opponent";
		fProtoUnitStartData.ProtoUnit = protoUnit;
		fProtoUnitStartData.Position = FPoint2.zero;
		fProtoUnitStartData.Facing = FAnimEntry.AnimFacing.RIGHT;
		int newID = -1;
		if (mCurrentRoom.SpawnUnit(fProtoUnitStartData, out newID) == FRoom.SpawnUnitResult.Success)
		{
			FUnit unitByID = GetUnitByID(newID);
			unitByID.SetFacing(FAnimEntry.AnimFacing.RIGHT);
			unitByID.SetSortingOrder(110);
			mVanContestant = new FBikeContestant();
			mVanContestant.Unit = unitByID;
			mVanContestant.MoveSpeed = 7f;
			Transform transform = mVanContestant.Unit.GO.transform.Find("Vis");
			transform.localPosition = (Vector3)(Vector2.left * 2f);
		}
	}

	protected override IEnumerator PostSceneLoad(bool revealWorld = true)
	{
		yield return StartCoroutine(base.PostSceneLoad(true));
		GameObject gameObject = new GameObject("WorldSprite_1");
		gameObject.transform.parent = mCurrentRoom.transform;
		gameObject.transform.position = mCurrentRoom.transform.position;
		gameObject.GetOrCreateComponent<SpriteRenderer>().sprite = mCurrentRoom.RoomSprite.sprite;
		mWorldSprites.Add(gameObject.transform);
		GameObject gameObject2 = new GameObject("WorldSprite_2");
		gameObject2.transform.parent = mCurrentRoom.transform;
		gameObject2.transform.position = mCurrentRoom.transform.position + Vector3.right * (float)mCurrentRoom.Width;
		gameObject2.GetOrCreateComponent<SpriteRenderer>().sprite = mCurrentRoom.RoomSprite.sprite;
		mWorldSprites.Add(gameObject2.transform);
		FApp.HUD.PageLegendWorld.EnableCharacterSwitching(false);
		FPageLegendWorld.TouchHoldInterval = 0f;
		mCamera.orthographicSize = 3f;
		float orthographicSize = mCamera.orthographicSize;
		float num = orthographicSize * 2f;
		float num2 = num * Futile.Screen.Width / Futile.Screen.Height * mCamera.rect.width;
		mCameraHalfHeight = num * 0.5f;
		mCameraHalfWidth = num2 * 0.5f;
		Vector3 camPos = mCamera.transform.position;
		float camX = num2 + mCameraHalfWidth - 0.5f;
		float camY = mCameraHalfHeight - 0.5f;
		mCamera.transform.position = new Vector3(camX, camY, camPos.z);
		Vector3 playerPos = mPlayerContestant.Unit.Position;
		float left = camX - mCameraHalfWidth;
		mPlayerContestant.Unit.GO.transform.position = new Vector3(left + (camX - left) * 0.8f, playerPos.y, playerPos.z);
		int lane = GetCurrentLane(FWorld.Player);
		mPlayerContestant.CurrentLane = lane;
		Vector3 lanePos = mLanes[lane];
		mVanContestant.Unit.GO.transform.position = new Vector3(left + (camX - left) * 0.1f, lanePos.y, lanePos.z);
		mVanContestant.CurrentLane = lane;
		mTimeBetweenLaneChecks = 0f;
		mInputEnabled = false;
		FGameUtil.StartCoroutine(DoChaseIntro());
	}

	protected IEnumerator DoChaseIntro()
	{
		Transform vanVis = mVanContestant.Unit.GO.transform.Find("Vis");
		yield return new WaitForSeconds(0.75f);
		FApp.HUD.PageLegendWorld.DimWorld(0f, 0.5f);
		FApp.SoundManager.PlaySoundSet("BossVan_Rev");
		Go.to(vanVis, 0.6f, new GoTweenConfig().localPosition(Vector3.zero).setEaseType(GoEaseType.BackOut));
		yield return new WaitForSeconds(1.35f);
		FApp.SoundManager.PlaySoundSet("BossVan_Horn");
		BeginBossEncounter(mVanContestant.Unit);
		yield return new WaitForSeconds(0.75f);
		mNextObstacleSpawn = mGametime + 1f;
		UnPause();
		mInputEnabled = true;
	}

	public override void FixedUpdate()
	{
		if (Paused)
		{
			return;
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		mCurrentRoom.DoUpdate(fixedDeltaTime);
		UpdateLanesAndContestants(fixedDeltaTime, mState != GameState.LOGIC_PAUSED, mState != GameState.ANIM_PAUSED);
		if (mState != GameState.LOGIC_PAUSED)
		{
			mGametime += fixedDeltaTime;
			Go.instance.HandleFWorldUpdate(fixedDeltaTime);
			if (mNextObstacleSpawn > 0f && mNextObstacleSpawn < mGametime)
			{
				SpawnObstacle();
			}
			for (int num = mObstacles.Count - 1; num >= 0; num--)
			{
				FUnit fUnit = mObstacles[num];
				if (fUnit != null && !(fUnit.GO == null) && fUnit.GO.transform.position.x < mCamera.transform.position.x - mCameraHalfWidth - 1f)
				{
					RemoveObstacle(fUnit, null);
				}
			}
			CheckEndConditions();
		}
		FApp.PlayerStats.TotalGameTime += fixedDeltaTime;
		mTimeRemaining -= fixedDeltaTime;
		if (mState != GameState.ANIM_PAUSED)
		{
			UpdateEffects(fixedDeltaTime);
			if (!mCameraTweening)
			{
				mCamera.transform.Translate(Vector3.right * fixedDeltaTime * 7f);
			}
		}
	}

	protected void UpdateLanesAndContestants(float deltaTime, bool updateLogic, bool updateAnims)
	{
		bool[] obstaclesOffScreen = new bool[mLanes.Length];
		UpdateContestant(mPlayerContestant, deltaTime, updateLogic, updateAnims, ref obstaclesOffScreen);
		UpdateContestant(mVanContestant, deltaTime, updateLogic, updateAnims, ref obstaclesOffScreen);
		if (!updateAnims)
		{
			return;
		}
		foreach (Transform mWorldSprite in mWorldSprites)
		{
			if (mWorldSprite.position.x - (mCamera.transform.position.x - mCameraHalfWidth) < (float)(-mCurrentRoom.Width))
			{
				mWorldSprite.Translate(Vector3.right * (float)mCurrentRoom.Width * 2f);
			}
		}
		UpdateLaneWarningBubbles(deltaTime, obstaclesOffScreen);
	}

	protected void UpdateContestant(FBikeContestant contestant, float deltaTime, bool updateLogic, bool updateAnims, ref bool[] obstaclesOffScreen)
	{
		if (updateAnims)
		{
			contestant.Unit.Update(deltaTime);
		}
		if (contestant.Unit.AliveAndWell)
		{
			FBikeContestant contestantInDirection = GetContestantInDirection(contestant.Unit, Vector2.right, 0.1f);
			float speed = contestant.MoveSpeed;
			float maxX = mCamera.transform.position.x;
			if (contestantInDirection != null && !contestant.Unit.ProtoUnit.ImmuneToKnockback)
			{
				speed = Mathf.Min(speed, contestantInDirection.MoveSpeed);
				maxX = contestantInDirection.Unit.Position.x - 1f;
			}
			Vector3 position = contestant.Unit.Position;
			float num = speed * deltaTime;
			float x = Mathf.Min(num + position.x, maxX);
			Vector3 position2 = contestant.Unit.Position;
			contestant.Unit.GO.transform.position = new Vector3(x, position2.y, position2.z);
		}
		if (!updateLogic || mRaceComplete)
		{
			return;
		}
		for (int num2 = mObstacles.Count - 1; num2 >= 0; num2--)
		{
			FUnit fUnit = mObstacles[num2];
			int currentLane = GetCurrentLane(fUnit);
			if (currentLane == contestant.CurrentLane && contestant.Unit.DistanceToUnitEdge(fUnit, false) < FUnitAIBase.cMoveTolerance)
			{
				HitContestant(contestant);
				RemoveObstacle(fUnit, contestant.Unit);
			}
			if (currentLane >= 0 && fUnit.GO != null && mCamera.transform.position.x + mCameraHalfWidth + 1f < fUnit.GO.transform.position.x)
			{
				obstaclesOffScreen[currentLane] = true;
			}
		}
		if (contestant.InputEnabled && contestant.State == FBikeContestant.ContestantStates.IDLE && !contestant.Unit.IsPlayer && contestant.LastLaneCheck + mTimeBetweenLaneChecks < mGametime)
		{
			contestant.LastLaneCheck = mGametime;
			mTimeBetweenLaneChecks = 0.5f;
			int num3 = mPlayerContestant.CurrentLane - contestant.CurrentLane;
			if (num3 != 0 && SwitchLanes(contestant, num3))
			{
				FApp.SoundManager.PlaySoundSet("BossVan_Rev");
				contestant.Unit.PlayAnim("Swerve", false);
			}
		}
		if (contestant.ShakeUntilTime > mGametime)
		{
			Transform vis = contestant.Unit.GO.transform.Find("Vis");
			vis.localPosition = (Vector3)(UnityEngine.Random.insideUnitCircle * 0.1f);
		}
		else if (contestant.ShakeUntilTime > 0f)
		{
			Transform vis2 = contestant.Unit.GO.transform.Find("Vis");
			vis2.localPosition = Vector3.zero;
			contestant.ShakeUntilTime = -1f;
			contestant.InputEnabled = true;
		}
	}

	protected void HitContestant(FBikeContestant contestant, int damage = 1)
	{
		if (contestant == null || contestant.Unit == null)
		{
			return;
		}
		FlashUnit(contestant.Unit);
		if (contestant.State == FBikeContestant.ContestantStates.IDLE)
		{
			float num = 1f;
			if (contestant.Unit.IsPlayer)
			{
				num = 0.3f;
			}
			contestant.ShakeUntilTime = mGametime + num;
		}
		else if (contestant.ShakeUntilTime < 0f)
		{
			contestant.ShakeUntilTime = 1f;
		}
		contestant.Unit.SetHP(contestant.Unit.HP - damage);
		contestant.InputEnabled = false;
		FUnitAIBossVan fUnitAIBossVan = contestant.Unit.AI as FUnitAIBossVan;
		if (fUnitAIBossVan == null)
		{
			contestant.Unit.PlayAnim("Hit", false);
			return;
		}
		FApp.SoundManager.PlaySoundSet("BossVan_Hit");
		int hP = contestant.Unit.HP;
		bool queueTransition = false;
		if (hP < 8)
		{
			fUnitAIBossVan.AnimIndex = 2;
			if (hP == 7)
			{
				queueTransition = true;
			}
			else
			{
				queueTransition = hP == 4;
				if (hP <= 4)
				{
					fUnitAIBossVan.AnimIndex = 3;
				}
				else
				{
					queueTransition = false;
				}
			}
		}
		FApp.HUD.PageLegendWorld.SetBossHealth(contestant.Unit.HP - 1, contestant.Unit.ProtoUnit.Health - 1);
		FApp.HUD.PageLegendWorld.FlashBossHealth();
		contestant.Unit.PlayAnim("Hit", false);
		if (queueTransition)
		{
			contestant.Unit.QueueAnim("Transition", false);
		}
	}

	protected void CheckEndConditions()
	{
		if (mRaceComplete)
		{
			return;
		}
		int hP = mVanContestant.Unit.HP;
		if (hP < 2)
		{
			StartCoroutine(DoWin());
			mRaceComplete = true;
		}
		else
		{
			if (mGametime < mNextVanEventCheck)
			{
				return;
			}
			mPossibleVanEvents.Clear();
			float num = 5f;
			if (hP < 8)
			{
				mPossibleVanEvents.Add("Charge");
				num = 4f;
				if (mVanContestant.Unit.HP < 5)
				{
					mPossibleVanEvents.Add("Fire");
					num = 2.5f;
				}
			}
			if (mPossibleVanEvents.Count > 0)
			{
				string randomElement = FRandom.Generic.GetRandomElement(mPossibleVanEvents);
				if (randomElement == "Charge")
				{
					mVanEventRoutine = FGameUtil.StartCoroutine(DoVanCharge());
				}
				else if (randomElement == "Fire")
				{
					mVanEventRoutine = FGameUtil.StartCoroutine(DoVanFireGun());
				}
			}
			mNextVanEventCheck = num + mGametime + 2f;
			if (!mRaceComplete)
			{
				return;
			}
		}
		mNextObstacleSpawn = -1f;
		foreach (FUnit item in new List<FUnit>(mObstacles))
		{
			RemoveUnitImmediate(item);
		}
	}

	protected IEnumerator DoVanCharge()
	{
		int vanLane = mVanContestant.CurrentLane;
		if (!IsObstacleInLane(vanLane))
		{
			SpawnObstacleInLane(vanLane);
		}
		mVanContestant.Unit.ShowEmote("Anger", false, 0f, 0f);
		mVanContestant.State = FBikeContestant.ContestantStates.CHARGING;
		yield return new WaitForSeconds(1f);
		Transform vanVis = mVanContestant.Unit.GO.transform.Find("Vis");
		FApp.SoundManager.PlaySoundSet("BossVan_Rev");
		float chargeX = FWorld.Player.Position.x - vanVis.position.x + -1f;
		Go.to(vanVis, 0.6f, new GoTweenConfig().localPosition(new Vector3(chargeX, 0f, 0f)).setEaseType(GoEaseType.QuadIn));
		yield return new WaitForSeconds(0.6f);
		bool hitPlayer = false;
		if (vanLane == mPlayerContestant.CurrentLane)
		{
			HitContestant(mPlayerContestant, 2);
			hitPlayer = true;
		}
		else
		{
			mVanContestant.State = FBikeContestant.ContestantStates.STUNNED;
			mVanContestant.Unit.PlayAnim("Stun");
		}
		Go.to(vanVis, 0.6f, new GoTweenConfig().localPosition(Vector3.zero).setEaseType(GoEaseType.QuadOut));
		yield return new WaitForSeconds(0.6f);
		if (!hitPlayer)
		{
			yield return new WaitForSeconds(2f);
		}
		mVanContestant.State = FBikeContestant.ContestantStates.IDLE;
		mVanEventRoutine = null;
	}

	protected IEnumerator DoVanFireGun()
	{
		mVanContestant.Unit.ShowEmote("Surprise", false, 0f, 0f);
		mVanContestant.State = FBikeContestant.ContestantStates.FIRING;
		yield return new WaitForSeconds(1f);
		FUnitAIBase ai = mVanContestant.Unit.AI;
		ai.AddAction(new FUnitActionAttack(GetNextActionID(), mVanContestant.Unit, FWorld.Player, false, "Attack"));
		yield return new WaitForSeconds(1f);
		mVanContestant.State = FBikeContestant.ContestantStates.IDLE;
		mVanEventRoutine = null;
	}

	protected void UpdateLaneWarningBubbles(float deltaTime, bool[] obstaclesOffScreen)
	{
		for (int i = 0; i < obstaclesOffScreen.Length; i++)
		{
			bool activeInHierarchy = mLaneWarningBubbles[i].GO.activeInHierarchy;
			bool flag = obstaclesOffScreen[i];
			mLaneWarningBubbles[i].GO.SetActive(flag);
			if (flag && !activeInHierarchy)
			{
				mLaneWarningBubbles[i].PlayAnim(FAnimDefManager.GetAnimTypeID("Spawn"), false);
				mLaneWarningBubbles[i].QueueAnim(FAnimDefManager.GetAnimTypeID("Play"), true);
			}
		}
		for (int num = mLaneWarningBubbles.Length - 1; num >= 0; num--)
		{
			mLaneWarningBubbles[num].Update(deltaTime);
			Vector3 position = mLaneWarningBubbles[num].GO.transform.position;
			mLaneWarningBubbles[num].GO.transform.position = new Vector3(mCamera.transform.position.x + mCameraHalfWidth + -0.5f, position.y, position.z);
		}
	}

	protected void Win()
	{
		StartCoroutine(DoWin());
	}

	protected IEnumerator DoWin()
	{
		if (mVanEventRoutine != null)
		{
			FGameUtil.StopCoroutine(mVanEventRoutine);
		}
		FBikeContestant van = mVanContestant;
		mInputEnabled = false;
		van.ShakeUntilTime = mGametime + 200f;
		EndBossEncounter(true);
		van.Unit.PlayAnim("Hit");
		Go.killAllAbstractTweensWithTarget(van.Unit.GO.transform);
		Go.to(van.Unit.GO.transform, 7f, new GoTweenConfig().vector3YProp("position", mCamera.transform.position.y - mCameraHalfHeight - 1f).setUpdateType(GoUpdateType.FWorldUpdate));
		float blowUpTime = 5f;
		int index = 6;
		while (true)
		{
			float x = FRandom.Generic.RangeFloat(0f, 1f);
			float y = FRandom.Generic.RangeFloat(-0.5f, 0.5f);
			SpawnEffect("SmallExplosion", van.Unit.GO.transform, new Vector2(x, y), 200, true);
			yield return new WaitForSeconds(0.15f);
			if (index > 5)
			{
				FApp.SoundManager.PlaySoundSet("BossVan_Explode");
				index = 0;
			}
			index++;
			blowUpTime += -0.15f;
			if (blowUpTime <= 0f)
			{
				break;
			}
		}
		StartCoroutine(EndLevel());
	}

	protected void Lose()
	{
		if (mVanEventRoutine != null)
		{
			FGameUtil.StopCoroutine(mVanEventRoutine);
		}
		FWorld.Player.Die();
		EndBossEncounter(false);
	}

	public override void ReleaseStuff()
	{
		base.ReleaseStuff();
		if (mVanEventRoutine != null)
		{
			FGameUtil.StopCoroutine(mVanEventRoutine);
		}
	}

	protected IEnumerator EndLevel()
	{
		SetPlayerCharacter("Mike", true);
		if (!ProtoWorld.LoadData.ContainsKey("LeadsTo"))
		{
			FApp.DestroyWorld(true);
		}
		else
		{
			string text = ProtoWorld.LoadData["LeadsTo"];
			string[] array = text.Split('.');
			if (array.Length < 2)
			{
				FLog.UnityWarning("Bike race map has wrong formatting for LeadsTo property. Expected {World Name}.{Entry Name} received : " + text);
			}
			else
			{
				FPlayerStartParams fPlayerStartParams = new FPlayerStartParams();
				fPlayerStartParams.EntryPoint = array[1];
				TryWorldTransition(array[0], null, fPlayerStartParams);
			}
		}
		yield break;
	}

	public override void LateUpdate()
	{
	}

	public override bool HandleWorldTap(Vector3 screenPos, bool force = false)
	{
		if (mInputEnabled && !Paused && FWorld.Player.Alive && !mSwitchedThisDrag)
		{
			int currentLane = mPlayerContestant.CurrentLane;
			int num = 0;
			float num2 = 20000f;
			for (int i = 0; i < mLanes.Length; i++)
			{
				float y = mCamera.WorldToScreenPoint(mLanes[i]).y;
				if (!(Mathf.Abs(num2 - screenPos.y) <= Mathf.Abs(y - screenPos.y)))
				{
					num = i;
					num2 = y;
				}
			}
			int num3 = num - currentLane;
			if (num3 != 0)
			{
				SwitchLanes(mPlayerContestant, (num3 >> 31) | 1);
				if (mVanContestant.CurrentLane == currentLane)
				{
					mTimeBetweenLaneChecks = 1f;
					mVanContestant.LastLaneCheck = mGametime;
				}
				mSwitchedThisDrag = false;
			}
		}
		return false;
	}

	public override bool HandleWorldHold(Vector3 screenPos)
	{
		return false;
	}

	protected bool SwitchLanes(FBikeContestant contestant, int direction, bool checkUnits = true)
	{
		int currentLane = contestant.CurrentLane;
		if (currentLane == -1)
		{
			return false;
		}
		FUnit unit = contestant.Unit;
		if (unit.IsPlayer)
		{
			mSwitchedThisDrag = true;
		}
		if (checkUnits)
		{
			Vector2 dir = ((direction == 1) ? Vector2.down : Vector2.up);
			if (GetContestantInDirection(unit, dir, 1f) != null)
			{
				return false;
			}
		}
		int num = Mathf.Clamp(currentLane + direction, 0, mLanes.Length - 1);
		contestant.CurrentLane = num;
		float y = mLanes[num].y;
		Go.killAllAbstractTweensWithTarget(unit.GO.transform);
		Go.to(unit.GO.transform, 0.25f, new GoTweenConfig().vector3YProp("position", y).setEaseType(GoEaseType.BackOut).setUpdateType(GoUpdateType.FWorldUpdate));
		return true;
	}

	protected int GetCurrentLane(FUnit unit)
	{
		if (unit != null && unit.GO != null)
		{
			for (int i = 0; i < mLanes.Length; i++)
			{
				if (Mathf.Approximately(mLanes[i].y, unit.GO.transform.position.y))
				{
					return i;
				}
			}
		}
		return -1;
	}

	protected void SpawnObstacle()
	{
		List<int> list = Enumerable.Range(0, mLanes.Length).ToList();
		int num = FRandom.Generic.RangeInt(1, 2);
		if (num > 0)
		{
			do
			{
				int index = FRandom.Generic.RangeInt(0, list.Count - 1);
				int lane = list[index];
				list.RemoveAt(index);
				SpawnObstacleInLane(lane);
				num--;
			}
			while (num != 0);
		}
		FApp.SoundManager.PlaySoundSet("BossVan_ObstacleAhead");
		mNextObstacleSpawn = mGametime + FRandom.Generic.RangeFloat(2.5f, 5f);
	}

	protected void SpawnObstacleInLane(int lane)
	{
		float x = mCamera.transform.position.x;
		float halfWidth = mCameraHalfWidth;
		float y = mLanes[lane].y;
		float z = mLanes[lane].z;
		string[] array;
		switch (lane)
		{
		case 2:
			array = mBottomObstaclesProtoUnits;
			break;
		case 1:
			array = mMiddleObstaclesProtoUnits;
			break;
		case 0:
			array = mTopObstaclesProtoUnits;
			break;
		default:
			return;
		}
		if (array == null)
		{
			return;
		}
		string unitName = array[FRandom.Generic.RangeInt(0, array.Length - 1)];
		int newID = -1;
		if (SpawnUnitAtPosition(unitName, FPoint2.zero, out newID, true) == FRoom.SpawnUnitResult.Success)
		{
			FUnit unitByID = GetUnitByID(newID);
			unitByID.GO.transform.position = new Vector3(x + halfWidth + 10f, y, z);
			mObstacles.Add(unitByID);
		}
	}

	protected void RemoveObstacle(FUnit unit, FUnit hitBy)
	{
		if (!mObstacles.Contains(unit))
		{
			return;
		}
		mObstacles.Remove(unit);
		if (hitBy == null)
		{
			RemoveUnitImmediate(unit);
			return;
		}
		unit.GO.transform.parent = mCamera.transform;
		List<Vector3> list = new List<Vector3>();
		Vector3 position = unit.Position;
		list.Add(position);
		Vector3 vertical = (FRandom.Generic.RandBool() ? Vector3.up : Vector3.down);
		Vector3 second = position + Vector3.right + Vector3.right + vertical;
		list.Add(second);
		list.Add(second + Vector3.left * (mCameraHalfHeight + 2f));
		GoSpline path = new GoSpline(list);
		Go.to(unit.GO.transform, 0.5f, new GoTweenConfig().positionPath(path).setUpdateType(GoUpdateType.FWorldUpdate));
		Go.to(unit.GO.transform, 0.5f, new GoTweenConfig().localRotation(new Vector3(0f, 0f, 180f), true).setEaseType(GoEaseType.BackIn).onComplete(delegate
		{
			RemoveUnitImmediate(unit);
		}));
		string animDefName = "PoofEffect";
		if (FWorld.Player != hitBy)
		{
			animDefName = "SmallExplosion";
		}
		SpawnEffect(animDefName, unit.Position, 200);
	}

	protected void RemoveUnitImmediate(FUnit unit)
	{
		Go.killAllAbstractTweensWithTarget(unit.GO.transform);
		RemoveUnit(unit);
	}

	protected bool IsObstacleInLane(int lane)
	{
		foreach (FUnit mObstacle in mObstacles)
		{
			if (GetCurrentLane(mObstacle) == lane)
			{
				return true;
			}
		}
		return false;
	}

	protected FBikeContestant GetContestantInDirection(FUnit unit, Vector2 dir, float distance)
	{
		if (mPlayerContestant == null || mVanContestant == null)
		{
			return null;
		}
		if (mContestants == null)
		{
			mContestants = new FBikeContestant[2] { mPlayerContestant, mVanContestant };
		}
		Vector3 position = unit.Position;
		RaycastHit2D[] array = Physics2D.CircleCastAll(new Vector2(position.x, position.y), 0.5f, dir, distance);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit2D hit = array[i];
			if (hit.collider == null || hit.collider.gameObject == null || hit.collider.isTrigger)
			{
				continue;
			}
			Vector3 delta = hit.transform.position - unit.Position;
			if (0f <= dir.x * delta.x + dir.y * delta.y)
			{
				FBikeContestant fBikeContestant = mContestants.FirstOrDefault((FBikeContestant c) => c.Unit.GO == hit.transform.gameObject);
				if (fBikeContestant != null && fBikeContestant.Unit != unit && fBikeContestant.Unit.Obstructs)
				{
					return fBikeContestant;
				}
			}
		}
		return null;
	}
}
