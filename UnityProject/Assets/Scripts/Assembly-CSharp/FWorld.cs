using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FWorld @ 0x17520fc..0x1767b80, Ghidra vaddr).
public class FWorld : MonoBehaviour
{
	public enum GameState
	{
		RUNNING = 0,
		PAUSED = 1,
		ANIM_PAUSED = 2,
		LOGIC_PAUSED = 3
	}

	public const float cSecretGetInterval = 1f;

	public const float cRoomTransitionTime = 0.75f;

	public const float cDefaultCameraOrthographicSize = 4f;

	public const float cMaxCameraOrthographicSize = 10f;

	public static int cMaxPixelBlockSize = 80;

	protected float mGametime;

	protected FPather mPather;

	protected Dictionary<string, int> mInventory = new Dictionary<string, int>();

	protected Camera mCamera;

	protected Vector2 mCameraFactor = Vector2.zero;

	public static FUnit Player;

	protected int mNextSpriteID;

	protected float mCameraMinX;

	protected float mCameraMaxX;

	protected float mCameraMinY;

	protected float mCameraMaxY;

	protected int mNextUnitID;

	protected int mNextActionID;

	protected int mNextRoomID;

	protected FTile mPlayerCurrentMoveIndicator;

	protected FUnit mPlayerCurrentUnitIndicator;

	protected Coroutine mPlayerCurrentUnitFlashRoutine;

	protected bool mInputEnabled;

	protected List<FAnimationController> mActiveEffects = new List<FAnimationController>();

	protected List<FNode> mFlyoffTexts = new List<FNode>();

	protected Vector3 mLastFlyoffPosition = Vector3.zero;

	protected Color mBaseTileTint = Color.white;

	protected Vector2 mMapScrollVector = Vector2.zero;

	protected static GameObject mWorldMapGO;

	protected static WorldMapParams mWorldMapParams;

	protected static Transform mMapArrow;

	protected Transform mMapMarkerParent;

	protected Vector3 mCurrentCenterPoint = Vector3.zero;

	protected SpriteRenderer mCover;

	protected Rect mCameraRect;

	protected List<FRoom> mRoomsVisitedSinceLastSave = new List<FRoom>();

	protected GameState mState;

	[System.Runtime.CompilerServices.CompilerGenerated]
	private Action m_mPlayerMoveCompleteAction;

	protected Coroutine mDoorTransitionSafetyRoutine;

	protected bool mCameraTweening;

	protected FRoom mCurrentRoom;

	protected List<FRoom> mRooms = new List<FRoom>();

	protected Vector3 mLastCameraPos = Vector3.zero;

	protected FUnit mTileHighlight;

	protected Shader mFlashShader;

	protected Shader mDefaultShader;

	protected List<FUnitTriggerRevealDesc> mTriggerReveals = new List<FUnitTriggerRevealDesc>();

	protected Coroutine mTriggerRevealRoutine;

	protected bool mSilentMove;

	protected FPlayerStartParams mStartParams;

	protected bool mBossEncounter;

	protected float mLastSecretGet = -20f;

	public bool HundredPercent;

	protected FStage mRenderStage;

	protected float mLastCharacterCycle;

	private Collider2D[] mCollidersHit = new Collider2D[100];

	public float Gametime => mGametime;

	public FProtoWorld ProtoWorld { get; protected set; }

	public Dictionary<string, int> Inventory => mInventory;

	public Camera Camera => mCamera;

	public Vector2 CameraFactor => mCameraFactor;

	public Color BaseTileTint => mBaseTileTint;

	public float CameraOrthographicSize
	{
		get
		{
			if (mCamera == null)
			{
				return 4f;
			}
			return mCamera.orthographicSize;
		}
		set
		{
			if (!(mCamera == null))
			{
				mCamera.orthographicSize = value;
			}
		}
	}

	public float OrthographicBasedFNodeScale
	{
		get
		{
			if (mRenderStage != null)
			{
				return 4f / CameraOrthographicSize;
			}
			return 1f;
		}
	}

	public bool InputEnabled
	{
		get
		{
			return mInputEnabled;
		}
		set
		{
			mInputEnabled = value;
		}
	}

	public GameState State => mState;

	public bool Paused => mState == GameState.PAUSED;

	public FRoom CurrentRoom => mCurrentRoom;

	public List<FRoom> Rooms => mRooms;

	public bool InBossEncounter => mBossEncounter;

	public FStage RenderStage => mRenderStage;

	protected event Action mPlayerMoveCompleteAction
	{
		add
		{
			m_mPlayerMoveCompleteAction = (Action)Delegate.Combine(m_mPlayerMoveCompleteAction, value);
		}
		remove
		{
			m_mPlayerMoveCompleteAction = (Action)Delegate.Remove(m_mPlayerMoveCompleteAction, value);
		}
	}

	public event Action PlayerMoveCompleteAction
	{
		add
		{
			mPlayerMoveCompleteAction -= value;
			mPlayerMoveCompleteAction += value;
		}
		remove
		{
			mPlayerMoveCompleteAction -= value;
		}
	}

	public static IEnumerator Init(string worldName, FPlayerStartParams startParams = null)
	{
		FProtoWorld worldToUse = null;
		if (FApp.WorldManager.ProtoWorlds == null)
		{
			yield break;
		}
		foreach (KeyValuePair<string, FProtoWorld> pair in FApp.WorldManager.ProtoWorlds)
		{
			if (pair.Key.Equals(worldName, StringComparison.OrdinalIgnoreCase))
			{
				worldToUse = pair.Value;
				break;
			}
		}
		if (worldToUse == null)
		{
			yield break;
		}
		foreach (FProtoWorld item in FApp.WorldManager.ProtoWorlds.Values.Where((FProtoWorld w) => w.MapPack == worldToUse.MapPack))
		{
			item.Init();
		}
		if (!worldToUse.Initialized)
		{
			yield break;
		}
		GameObject gameObject = new GameObject("LegendWorld");
		Type type = Type.GetType("FWorld" + (worldToUse.SpecialMapType.Valid() ? worldToUse.SpecialMapType : string.Empty));
		if (type == null)
		{
			yield break;
		}
		FApp.HUD.PageLegendWorld.DimWorld(1f, 0f);
		FWorld world = (FWorld)gameObject.AddComponent(type);
		FApp.World = world;
		world.ProtoWorld = worldToUse;
		world.mPather = new FPather(world);
		world.LoadMap(startParams);
		yield return world.StartCoroutine(world.PostSceneLoad());
	}

	public virtual void ReleaseStuff()
	{
		// port guard: the player does not exist yet when the map failed to load
		if (FWorld.Player != null && FWorld.Player.AI != null)
		{
			FWorld.Player.AI.StopActions();
		}
		for (int i = 0; i < mRooms.Count; i++)
		{
			mRooms[i].Release();
		}
		StopAllCoroutines();
		if (mRenderStage != null)
		{
			FLog.Log("FWorld::ReleaseStuff: Removing World Stage.");
			Futile.RemoveStage(mRenderStage);
			mRenderStage = null;
		}
		else
		{
			FLog.Log("FWorld::ReleaseStuff: World Stage is already NULL.");
		}
	}

	public virtual void Start()
	{
		cMaxPixelBlockSize = Futile.Screen.IntHeight / 5;
	}

	public void LoadMap(FPlayerStartParams startParams = null)
	{
		if (ProtoWorld == null)
		{
			FLog.UnityError("Invalid ProtoWorld or Tilemap.");
			return;
		}
		mNextUnitID = 0;
		mStartParams = startParams;
		FPoint2 playerStart = ProtoWorld.Start;
		foreach (FProtoRoom protoRoom in ProtoWorld.ProtoRooms)
		{
			FRoom room = new GameObject(protoRoom.Name).AddComponent<FRoom>();
			room.Create(protoRoom, this, mNextRoomID++);
			mRooms.Add(room);
			if (mCurrentRoom == null && protoRoom.Name == ProtoWorld.StartRoom)
			{
				mCurrentRoom = room;
			}
			if (startParams == null || string.IsNullOrEmpty(startParams.EntryPoint))
			{
				if (FApp.PlayerStats.CurrentRoom.Valid() && FApp.PlayerStats.CurrentWorld == ProtoWorld.Name && protoRoom.Name == FApp.PlayerStats.CurrentRoom)
				{
					mCurrentRoom = room;
					playerStart = room.GetEntrance(room.EntranceUsed);
				}
			}
			else
			{
				FEntryData fEntryData = protoRoom.EntryDatas.FirstOrDefault((FEntryData e) => e.Name == startParams.EntryPoint);
				if (fEntryData != null)
				{
					mCurrentRoom = room;
					playerStart = fEntryData.Position;
				}
			}
		}
		mFlashShader = Shader.Find("GUI/Text Shader");
		mDefaultShader = Shader.Find("Sprites/Default");
		LoadSpecialUnits(playerStart);
	}

	public int GetNextRoomID()
	{
		return mNextRoomID++;
	}

	protected virtual void LoadSpecialUnits(FPoint2 playerstart)
	{
		FProtoUnitStartData fProtoUnitStartData = new FProtoUnitStartData();
		fProtoUnitStartData.Name = "Player";
		string currentCharacter = FApp.PlayerStats.CurrentCharacter;
		string serialized = FApp.PlayerStats.CurrentPlayerState.SerializedOutfits;
		if (!string.IsNullOrEmpty(serialized))
		{
			OutfitHandler.Deserialize(serialized);
		}
		fProtoUnitStartData.ProtoUnit = currentCharacter;
		fProtoUnitStartData.Position = playerstart;
		if (Player == null)
		{
			Player = new FUnit(mCurrentRoom, fProtoUnitStartData, mNextUnitID++);
			Player.IsPlayer = true;
		}
		else
		{
			mNextUnitID++;
			fProtoUnitStartData.Facing = Player.Facing;
			if (Player.ProtoUnit.DisplayName == fProtoUnitStartData.ProtoUnit)
			{
				fProtoUnitStartData.ProtoUnit = Player.ProtoUnit.Name;
			}
			Player.SetStartData(fProtoUnitStartData);
			Player.SetRoom(mCurrentRoom);
		}
		SetupPlayer();
		if (mTileHighlight == null)
		{
			FProtoUnitStartData fProtoUnitStartData2 = new FProtoUnitStartData();
			fProtoUnitStartData2.Name = "MoveIndicator";
			fProtoUnitStartData2.ProtoUnit = "MoveIndicator";
			fProtoUnitStartData2.Position = ProtoWorld.Start;
			mTileHighlight = new FUnit(mCurrentRoom, fProtoUnitStartData2, mNextUnitID++);
			mTileHighlight.Create(mCurrentRoom.gameObject, "");
			mTileHighlight.SetVisible(false);
		}
	}

	public int GetNextUnitID()
	{
		return mNextUnitID++;
	}

	protected void SetupPlayer()
	{
		string currentCharacter = FApp.PlayerStats.CurrentCharacter;
		Player.FullReset();
		if (Player.ProtoUnit.DisplayName == currentCharacter)
		{
			currentCharacter = Player.ProtoUnit.Name;
		}
		FProtoUnit protoUnit = FApp.ProtoUnitManager.GetProtoUnit(currentCharacter, true);
		SetPlayerCharacter(protoUnit);
		Player.HP = FApp.PlayerStats.Health;
		Player.SetAlive(true);
	}

	public void SetPlayerCharacter(string protoName, bool force = false)
	{
		FProtoUnit protoUnit = FApp.ProtoUnitManager.GetProtoUnit(protoName, true);
		if (protoUnit != null)
		{
			SetPlayerCharacter(protoUnit, force);
		}
	}

	public void SetPlayerCharacter(FProtoUnit proto, bool force = false)
	{
		Player.ChangeProto(proto, force);
		if (FApp.PlayerStats.CurrentCharacter != proto.DisplayName)
		{
			FApp.PlayerStats.CurrentCharacter = proto.Name;
		}
		FApp.HUD.PageLegendWorld.ResetSpecialWeaponButton();
	}

	public void CyclePlayerCharacter()
	{
		if (FApp.PlayerStats.LastCharacter.Valid() && mGametime - mLastCharacterCycle > 0.25f)
		{
			mLastCharacterCycle = mGametime;
			FProtoUnit protoUnit = FApp.ProtoUnitManager.GetProtoUnit(FApp.PlayerStats.LastCharacter, true);
			if (protoUnit != null)
			{
				SetPlayerCharacter(protoUnit, force: true);
				SpawnEffect("PoofEffect", base.transform, Player.Position, 200);
			}
		}
	}

	public void Dump()
	{
		if (mCurrentRoom == null)
		{
			FLog.Log("LegendWorld: No current room.");
			return;
		}
		FLog.Log("LegendWorld: DUMP:");
		Player.Dump("");
		for (int i = 0; i < mCurrentRoom.Units.Count; i++)
		{
			mCurrentRoom.Units[i].Dump("");
		}
	}

	protected virtual IEnumerator PostSceneLoad(bool revealWorld = true)
	{
		mState = GameState.LOGIC_PAUSED;
		mRenderStage = new FStage("World2");
		mRenderStage.SetShouldSortByZ(true);
		Futile.AddStageAtIndex(mRenderStage, 0);
		mRenderStage.MarkDirtyRecursive();
		FApp.HUD.RemoveAllUIPages();
		FApp.HUD.PushPage(FApp.HUD.PageLegendWorld, true, false);
		FApp.HUD.PageLegendWorld.ResetSpecialWeaponButton();
		InitCamera();
		BuildMap();
		StartCoroutine(LoadRoomSprites());
		yield return new WaitForSeconds(0.1f);
		FPageLegendWorld.TouchHoldInterval = 0.2f;
		if (mCover == null)
		{
			GameObject gameObject = new GameObject("CoverGO");
			gameObject.transform.parent = mCamera.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 7f);
			gameObject.transform.localScale = new Vector3(20f, 20f, 1f);
			mCover = gameObject.GetOrCreateComponent<SpriteRenderer>();
			mCover.sprite = Resources.Load<Sprite>("Prefabs/UI/BlackSquare");
			mCover.sortingLayerName = "Foreground";
			mCover.sortingOrder = 500;
		}
		SetCoverAlpha(0f);
		mBaseTileTint = (ProtoWorld.TintColor.Valid() ? FUtil.ColorFromHex(ProtoWorld.TintColor) : Color.white);
		SetCurrentRoom(mCurrentRoom, false);
		foreach (FRoom room in mRooms)
		{
			if (room != mCurrentRoom)
			{
				if (FApp.PlayerStats.HasVisitedRoom(room))
				{
					room.SetRoomState(FRoom.RoomStates.Visited, force: true, immediate: true);
				}
				else
				{
					room.SetRoomState(FRoom.RoomStates.Unvisited, force: true, immediate: true);
				}
			}
		}
		SetCameraPosition();
		Vector3 back = Player.Back();
		mCamera.transform.position = new Vector3(mCurrentCenterPoint.x + back.x * 2f, mCurrentCenterPoint.y + back.y * 2f, mCurrentCenterPoint.z);
		if (!ProtoWorld.UpsideDown)
		{
			mCamera.transform.Find("UD Particles").gameObject.SetActive(false);
			FApp.HUD.PageLegendWorld.EnableCharacterSwitching(true);
		}
		else
		{
			mCamera.transform.Find("UD Particles").gameObject.SetActive(true);
			FApp.HUD.PageLegendWorld.EnableCharacterSwitching(false);
		}
		if (mStartParams != null && mStartParams.ShowChapterTitle >= 0 && FApp.PlayerStats.CurrentChapter < mStartParams.ShowChapterTitle)
		{
			ShowChapterTitle(mStartParams.ShowChapterTitle);
		}
		else
		{
			if (revealWorld)
			{
				FApp.HUD.PageLegendWorld.DimWorld(0f, 0.5f);
			}
			yield return new WaitForSeconds(0.25f);
			Pixelate component = mCamera.GetComponent<Pixelate>();
			component.TweenToBlockSize(1);
			PlayLevelMusic();
			PlayStartSound();
			mInputEnabled = true;
			mState = GameState.RUNNING;
		}
		mCurrentRoom.PlayerEntranceComplete();
		CheckQuestReminders();
	}

	protected void StartWorld(float musicdelay = 0f)
	{
		PlayLevelMusic();
		PlayStartSound();
		mState = GameState.RUNNING;
		mInputEnabled = true;
	}

	protected void ShowChapterTitle(int index)
	{
		Transform transform = mCamera.transform.Find("ChapterTitle");
		if (transform == null || FApp.ChapterTitleCamera == null || FApp.ChapterTitle == null || FApp.ChapterSubTitle == null)
		{
			return;
		}
		ChapterTitleAlphaSetter alphaSetter = transform.GetComponent<ChapterTitleAlphaSetter>();
		if (alphaSetter == null)
		{
			return;
		}
		FApp.ChapterTitleCamera.SetActive(true);
		FApp.HUD.PageLegendWorld.DimWorld(1f, 0f);
		FApp.HUD.PageLegendWorld.DimWorld(0f, 0.5f);
		FApp.PlayerStats.CurrentChapter = index;
		FApp.ChapterTitle.text = FLoc.GetString("Chapter." + index.ToString() + ".Title");
		FApp.ChapterSubTitle.text = FLoc.GetString("Chapter." + index.ToString() + ".SubTitle");
		FApp.SoundManager.PlayMusic("FanfareTheme", 0.25f, 0.25f, true, 0.25f);
		mInputEnabled = false;
		transform.gameObject.SetActive(true);
		alphaSetter.Alpha = 1f;
		alphaSetter.WhiteAlpha = 1f;
		transform.localScale = new Vector3(800f, 400f, 1f);
		Go.to(alphaSetter, 1f, new GoTweenConfig().floatProp("WhiteAlpha", 0f).setDelay(1.5f));
		Go.to(transform, 4f, new GoTweenConfig().scale(new Vector3(15000f, 7000f, 20f)).setEaseType(GoEaseType.CircIn).setDelay(2f).onComplete(delegate
		{
			FinishTitle();
		}));
		Go.to(alphaSetter, 1f, new GoTweenConfig().floatProp("Alpha", 0f).setDelay(5f));
	}

	protected void FinishTitle()
	{
		PlayLevelMusic(0.5f);
		PlayStartSound();
		mState = GameState.RUNNING;
		mInputEnabled = true;
		if (FApp.ChapterTitleCamera != null)
		{
			FApp.ChapterTitleCamera.SetActive(false);
		}
		Transform transform = mCamera.transform.Find("ChapterTitle");
		if (!(transform == null))
		{
			transform.gameObject.SetActive(false);
		}
	}

	protected void CheckQuestReminders()
	{
		foreach (int currentQuest in FApp.PlayerStats.CurrentQuests)
		{
			FProtoQuest protoQuestByDBID = FApp.ProtoQuestManager.GetProtoQuestByDBID(currentQuest);
			if (protoQuestByDBID == null || protoQuestByDBID.Reminders == null)
			{
				continue;
			}
			foreach (QuestReminder reminder in protoQuestByDBID.Reminders)
			{
				if (reminder.CheckWaitTime() && reminder.Dialogue.Valid())
				{
					TryShowDialogue("", reminder.Dialogue);
					break;
				}
			}
		}
	}

	protected void PlayStartSound()
	{
		if (mStartParams != null && mStartParams.PlaySound.Valid())
		{
			FApp.SoundManager.PlaySoundSet(mStartParams.PlaySound);
		}
	}

	public void PlayLevelMusic(float delay = 0f)
	{
		string text = (string.IsNullOrEmpty(ProtoWorld.ThemeMusic) ? "InsideTheme" : ProtoWorld.ThemeMusic);
		if (text != "None")
		{
			FApp.SoundManager.PlayMusic(text, 0.25f, 0.25f, true, delay);
		}
		else if (FApp.SoundManager.IsMusicPlaying())
		{
			FApp.SoundManager.FadeOutMusic(0f);
		}
	}

	protected void InitCamera()
	{
		mCameraFactor = Vector2.one;
		if (!Futile.Screen.IsWidescreen)
		{
			mCameraFactor = new Vector2(1.125f, 0.889f);
		}
		else
		{
			mCameraFactor = new Vector2(0.84f, 1f);
		}
		GameObject gameObject = GameObject.Find("WorldCamera");
		if (gameObject == null)
		{
			GameObject gameObject2 = FUtil.ClonePrefab("Prefabs/System/WorldCamera");
			mCamera = gameObject2.GetComponent<Camera>();
			mCamera.name = "WorldCamera";
			mCamera.clearFlags = CameraClearFlags.SolidColor;
			mCamera.backgroundColor = new Color(0f, 0f, 0f, 1f);
			mCamera.depth = 50f;
			mCamera.rect = new Rect(0f, 0f, mCameraFactor.x, mCameraFactor.y);
			mCamera.orthographic = true;
			CameraOrthographicSize = 4f;
			mCamera.transform.localRotation = Quaternion.identity;
		}
		else
		{
			mCamera = gameObject.GetComponent<Camera>();
			Transform transform = mCamera.transform.Find("CoverGO");
			if (transform != null)
			{
				mCover = transform.GetComponent<SpriteRenderer>();
			}
		}
		Go.killAllAbstractTweensWithTarget(mCamera.transform);
		ResetCameraSize();
		Animator component = mCamera.GetComponent<Animator>();
		if (component != null && component.enabled)
		{
			component.enabled = false;
		}
		mCamera.cullingMask &= 0x3FFFFFFF;
	}

	public void ResetCameraSize()
	{
		CameraOrthographicSize = 4f;
		SyncCameraExtents();
		mInputEnabled = CameraOrthographicSize == 4f;
		FApp.HUD.PageLegendWorld.LayoutPage();
	}

	protected void SyncCameraExtents(float overrideOrthoSize = -1f)
	{
		if (mCurrentRoom == null)
		{
			return;
		}
		if (overrideOrthoSize <= 0f)
		{
			overrideOrthoSize = CameraOrthographicSize;
		}
		float a = Futile.Screen.Width;
		float b = Futile.Screen.Height;
		float aspect = (overrideOrthoSize * a) / b * mCamera.rect.width;
		float halfW = aspect;
		float width = halfW * 2f;
		float height = overrideOrthoSize * 2f;
		mCameraRect = new Rect(Vector2.zero, new Vector2(width, height));
		Rect rect = (mWorldMapParams != null || true) ? default(Rect) : default(Rect);
		// bounds of the room (the shipped code reads them via the proto room bounds Vector4 of the world map when non-zero)
		Vector4 bounds = Vector4.zero;
		if (mCurrentRoom != null && mCurrentRoom.ProtoRoom != null)
		{
			Rect r = mCurrentRoom.ProtoRoom.Bounds;
			bounds = new Vector4(r.x, r.y, r.width, r.height);
		}
		bool zero = bounds.x == 0f && bounds.y == 0f && bounds.z == 0f && bounds.w == 0f;
		if (zero)
		{
			mCameraMinX = halfW - 0.5f;
			mCameraMaxX = (float)ProtoWorld.Width - halfW + 2f;
			mCameraMinY = overrideOrthoSize - 0.5f;
			mCameraMaxY = (float)ProtoWorld.Height - overrideOrthoSize - 0.5f;
			return;
		}
		float minX = halfW + bounds.x - 0.5f;
		float maxX = bounds.x + bounds.z - halfW - 0.5f - 0.25f;
		mCameraMinX = minX;
		mCameraMaxX = Mathf.Max(minX, maxX);
		if (bounds.z < width)
		{
			float d = (width - bounds.z) * 0.5f;
			mCameraMinX = minX - d;
			mCameraMaxX = mCameraMaxX - d;
		}
		float minY = overrideOrthoSize + bounds.y - 0.5f;
		float maxY = bounds.y + bounds.w - overrideOrthoSize - 0.5f;
		mCameraMinY = minY;
		mCameraMaxY = Mathf.Max(minY, maxY);
		if (height > bounds.w)
		{
			float d2 = (height - bounds.w) * 0.5f;
			mCameraMinY = minY - d2;
			mCameraMaxY = mCameraMaxY - d2;
		}
	}

	protected void SetCurrentRoom(FRoom newRoom, bool shouldSave, bool animateCamera = false)
	{
		if (mCurrentRoom != null && newRoom.ID != mCurrentRoom.ID)
		{
			Player.RemoveFromTile();
			mCurrentRoom.PlayerLeftRoom();
		}
		if (shouldSave)
		{
			FApp.PlayerStats.SavePlayerState(mCurrentRoom);
			mRoomsVisitedSinceLastSave.Clear();
		}
		if (!mRoomsVisitedSinceLastSave.Contains(newRoom))
		{
			mRoomsVisitedSinceLastSave.Add(newRoom);
		}
		mCurrentRoom = newRoom;
		Player.SetRoom(mCurrentRoom);
		Player.SyncTilePosition();
		mTileHighlight.SetRoom(mCurrentRoom);
		mCurrentRoom.PlayerEnteredRoom(shouldSave);
		SyncCameraExtents();
		SetCameraPosition(animateCamera, RoomTransitionComplete);
		if (!animateCamera)
		{
			mCamera.transform.position = mCurrentCenterPoint;
		}
		FApp.HUD.PageLegendWorld.RoomChanged();
		TintWorld(mBaseTileTint);
	}

	protected void RoomTransitionComplete(AbstractGoTween tween)
	{
		CameraMoveComplete(tween);
		mCurrentRoom.PlayerEntranceComplete();
	}

	protected void SetInputFromOrthographicSize()
	{
		mInputEnabled = CameraOrthographicSize == 4f;
	}

	protected void SetCameraPosition(bool animate = false, Action<AbstractGoTween> onComplete = null)
	{
		if (mCamera == null)
		{
			return;
		}
		Vector3 clamped = GetClampedCameraPos(Player.Position);
		if (animate)
		{
			mCameraTweening = true;
			if (onComplete == null)
			{
				onComplete = CameraMoveComplete;
			}
			Go.killAllAbstractTweensWithTarget(mCamera.transform);
			Go.to(mCamera.transform, 0.75f, new GoTweenConfig().position(clamped).setEaseType(GoEaseType.QuadInOut).onComplete(onComplete));
		}
		else
		{
			Vector3 last = mLastCameraPos;
			mCurrentCenterPoint = clamped;
			if ((clamped - last).magnitude > 0.2f)
			{
				mLastCameraPos = clamped;
			}
		}
		if (mCurrentRoom != null)
		{
			mCurrentRoom.SyncUnitUINodes();
		}
		Vector3 pos = mCamera.transform.position;
		Vector3 prev = mLastFlyoffPosition;
		if ((pos - prev).sqrMagnitude < 1E-10f)
		{
			return;
		}
		if (FUtil.EpsilonEqual(pos, prev))
		{
			return;
		}
		foreach (FNode flyoff in mFlyoffTexts)
		{
			UpdateUINodePosition(flyoff, prev, mCamera.transform.position);
		}
		mLastFlyoffPosition = mCamera.transform.position;
	}

	protected Vector3 GetClampedCameraPos(Vector3 startPos)
	{
		Vector3 p = mCamera.transform.position;
		float x = Mathf.Clamp(startPos.x, mCameraMinX, mCameraMaxX);
		float y = Mathf.Clamp(startPos.y, mCameraMinY, mCameraMaxY);
		float t = Mathf.Clamp01((CameraOrthographicSize - 4f) / 6f);
		return new Vector3(x + (startPos.x - x) * t, y + (startPos.y - y) * t, p.z);
	}

	protected void CameraMoveComplete(AbstractGoTween tween)
	{
		if (!mInputEnabled && mCameraTweening && !mCurrentRoom.IsBossRoom())
		{
			mInputEnabled = true;
		}
		mCurrentCenterPoint = mCamera.transform.position;
		mCameraTweening = false;
		SyncCameraExtents();
	}

	public Vector3 GetScreenPosition(Vector3 position)
	{
		return Futile.WorldPositionToScreen(mCamera, position);
	}

	public FTile GetTile(FPoint2 position)
	{
		if (mCurrentRoom != null)
		{
			FTile tile = mCurrentRoom.GetTile(position);
			if (tile != null)
			{
				return tile;
			}
			for (int i = 0; i < mRooms.Count; i++)
			{
				if (!(mRooms[i] == null))
				{
					tile = mRooms[i].GetTile(position);
					if (tile != null)
					{
						return tile;
					}
				}
			}
		}
		return null;
	}

	public FTile GetTileFromGO(GameObject go)
	{
		FPoint2 fPoint = FPoint2.Parse(go.name);
		if (fPoint == FPoint2.invalid)
		{
			return null;
		}
		return GetTile(fPoint);
	}

	public FUnit GetUnitFromGO(GameObject go)
	{
		int result = -1;
		FUnit result2 = null;
		if (int.TryParse(go.name, out result))
		{
			result2 = GetUnitByID(result);
		}
		return result2;
	}

	public FUnit GetUnitByID(int id)
	{
		if (id < 0)
		{
			return null;
		}
		if (Player != null && Player.ID == id)
		{
			return Player;
		}
		if (mCurrentRoom != null)
		{
			FUnit unitByID = mCurrentRoom.GetUnitByID(id);
			if (unitByID != null)
			{
				return unitByID;
			}
		}
		for (int i = 0; i < mRooms.Count; i++)
		{
			if (!(mRooms[i] == null))
			{
				FUnit unitByID2 = mRooms[i].GetUnitByID(id);
				if (unitByID2 != null)
				{
					return unitByID2;
				}
			}
		}
		return null;
	}

	public FUnit GetUnitByName(string name)
	{
		if (Player != null && name == Player.Name)
		{
			return Player;
		}
		if (mCurrentRoom != null)
		{
			FUnit unitByName = mCurrentRoom.GetUnitByName(name);
			if (unitByName != null)
			{
				return unitByName;
			}
		}
		for (int i = 0; i < mRooms.Count; i++)
		{
			if (!(mRooms[i] == null))
			{
				FUnit unitByName2 = mRooms[i].GetUnitByName(name);
				if (unitByName2 != null)
				{
					return unitByName2;
				}
			}
		}
		return null;
	}

	public FUnit[] GetUnitsByName(string name)
	{
		List<FUnit> list = new List<FUnit>();
		if (Player != null && name == Player.Name)
		{
			list.Add(Player);
			return list.ToArray();
		}
		for (int i = 0; i < mRooms.Count; i++)
		{
			if (!(mRooms[i] == null))
			{
				list.AddRange(mRooms[i].GetUnitsByName(name));
			}
		}
		return list.ToArray();
	}

	public void RemoveUnit(FUnit unit, float delay = 0f)
	{
		if (Player == unit)
		{
			if (delay <= 0f)
			{
				delay = 3f;
			}
			StartCoroutine(ResetPlayer(delay));
		}
		else if (unit != null && mCurrentRoom != null)
		{
			FRoom fRoom = null;
			if (mCurrentRoom.GetUnitByID(unit.ID) != null)
			{
				fRoom = mCurrentRoom;
			}
			else
			{
				for (int i = 0; i < mRooms.Count; i++)
				{
					if (!(mRooms[i] == null) && mRooms[i].GetUnitByID(unit.ID) != null)
					{
						fRoom = mRooms[i];
						break;
					}
				}
			}
			if (fRoom != null)
			{
				fRoom.RemoveUnit(unit, delay);
			}
		}
	}

	public bool IsUnitOnScreen(FUnit unit)
	{
		Vector3 pos = mCamera.transform.position;
		mCameraRect.x = pos.x - mCameraRect.width * 0.5f;
		mCameraRect.y = pos.y - mCameraRect.height * 0.5f;
		if (unit == null)
		{
			throw new NullReferenceException();
		}
		Vector3 p = unit.Position;
		return !(p.x < mCameraRect.x) && !(mCameraRect.x + mCameraRect.width <= p.x) && !(p.y < mCameraRect.y) && p.y < mCameraRect.y + mCameraRect.height;
	}

	protected virtual IEnumerator ResetPlayer(float waitTime)
	{
		mInputEnabled = false;
		yield return new WaitForSeconds(waitTime);
		Pixelate component = mCamera.GetComponent<Pixelate>();
		component.TweenToBlockSize(cMaxPixelBlockSize);
		yield return new WaitForSeconds(0.25f);
		FApp.HUD.PageLegendWorld.DimWorld(1f, 0.5f);
		FApp.PlayerStats.Reset();
		yield return new WaitForSeconds(0.5f);
		if (mBossEncounter)
		{
			EndBossEncounter(false);
		}
		for (int i = 0; i < mRoomsVisitedSinceLastSave.Count; i++)
		{
			mRoomsVisitedSinceLastSave[i].ResetRoom();
		}
		FApp.DestroyWorld(false);
		FWorld.Player = null;
		FPluginRelay.Instance.StartCoroutine(Init(FApp.PlayerStats.CurrentWorld, new FPlayerStartParams()));
	}

	public void ResetPlayerNoSwitchRooms()
	{
		StartCoroutine(DoResetPlayerNoSwitchRooms());
	}

	protected IEnumerator DoResetPlayerNoSwitchRooms()
	{
		mInputEnabled = false;
		FApp.HUD.PageLegendWorld.ShowBossHealth(false);
		FApp.HUD.PageLegendWorld.DimWorld(1f, 0.5f);
		yield return new WaitForSeconds(0.5f);
		FApp.PlayerStats.HardSaveRoom(mCurrentRoom);
		FApp.PlayerStats.Reset();
		mCurrentRoom.ResetRoom();
		SetupPlayer();
		mCurrentRoom.ResetPlayerPos();
		FApp.HUD.PageLegendWorld.DimWorld(0f, 0.5f);
		yield return new WaitForSeconds(0.5f);
		mCurrentRoom.PlayerEntranceComplete();
		mInputEnabled = true;
		Player.EnableInput(true);
	}

	public void ForceChangeCurrentRoom(FRoom room, bool setPlayerPos = true)
	{
		SetCurrentRoom(room, true);
		SetPlayerCharacter(Player.ProtoUnit);
		SetupPlayer();
		if (setPlayerPos)
		{
			mCurrentRoom.ResetPlayerPos();
		}
	}

	public FRoom GetRoomByID(int id)
	{
		if (id < 0 || mRooms == null || mRooms.Count == 0)
		{
			return null;
		}
		return mRooms.FirstOrDefault((FRoom r) => r.ID == id);
	}

	public FRoom GetRoomByIndex(int index)
	{
		if (index >= 0 && mRooms != null && index < mRooms.Count)
		{
			return mRooms[index];
		}
		return null;
	}

	public int GetNextActionID()
	{
		return mNextActionID++;
	}

	public virtual void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		Player.Update(fixedDeltaTime);
		mTileHighlight.Update(fixedDeltaTime);
		mCurrentRoom.DoUpdate(fixedDeltaTime);
		if (mWorldMapGO != null && mWorldMapGO.activeInHierarchy && !FApp.HUD.PageWorldMap.TouchDown)
		{
			float t = Mathf.Clamp01(fixedDeltaTime * 2f);
			mMapScrollVector = Vector2.Lerp(mMapScrollVector, Vector2.zero, t);
			ScrollWorldMap(mMapScrollVector, countAsScroll: false);
		}
		if (mState != GameState.PAUSED)
		{
			if (mState != GameState.LOGIC_PAUSED)
			{
				mGametime += fixedDeltaTime;
				Go.instance.HandleFWorldUpdate(fixedDeltaTime);
				if (HundredPercent && mInputEnabled && Player.InputEnabled)
				{
					FApp.HUD.PageDungeonComplete.OverrideDungeonName = "100%";
					FApp.HUD.PushPage(FApp.HUD.PageDungeonComplete, true, false);
					FApp.SoundManager.PlaySoundSet("FanfareTheme");
					HundredPercent = false;
				}
				FApp.PlayerStats.TotalGameTime += fixedDeltaTime;
			}
			UpdateEffects(fixedDeltaTime);
		}
		if (!mCameraTweening)
		{
			Vector3 p = mCamera.transform.position;
			float t2 = Mathf.Clamp01(fixedDeltaTime * 7f);
			mCamera.transform.position = new Vector3(p.x + t2 * (mCurrentCenterPoint.x - p.x), p.y + t2 * (mCurrentCenterPoint.y - p.y), p.z + t2 * (mCurrentCenterPoint.z - p.z));
		}
		else if (mCurrentRoom != null)
		{
			mCurrentRoom.SyncUnitUINodes();
		}
	}

	public void ScrollWorldMap(Vector2 amt, bool countAsScroll = true)
	{
		if (countAsScroll)
		{
			mMapScrollVector = amt;
		}
		Vector3 local = mWorldMapGO.transform.localPosition;
		SetWorldMapPosition(new Vector2(amt.x + local.x, amt.y + local.y));
	}

	protected void UpdateEffects(float deltaTime)
	{
		for (int num = mActiveEffects.Count - 1; num >= 0; num--)
		{
			mActiveEffects[num].Update(deltaTime);
			if (!mActiveEffects[num].Animator.Playing)
			{
				UnityEngine.Object.Destroy(mActiveEffects[num].GO);
				mActiveEffects.RemoveAt(num);
			}
		}
	}

	public void TrySetCameraSizeDelta(float delta)
	{
		if (!mCameraTweening)
		{
			CameraOrthographicSize = Mathf.Clamp(CameraOrthographicSize + delta, 4f, 10f);
			SyncCameraExtents();
		}
	}

	public virtual void LateUpdate()
	{
		if (!mCameraTweening)
		{
			SetCameraPosition();
		}
	}

	public void Pause(bool logic = true, bool anims = true)
	{
		mState = (logic ? GameState.PAUSED : GameState.LOGIC_PAUSED);
		if (!anims)
		{
			mState = GameState.ANIM_PAUSED;
		}
	}

	public void UnPause()
	{
		mState = GameState.RUNNING;
	}

	public virtual bool HandleWorldTap(Vector3 screenPos, bool force = false)
	{
		if (mInputEnabled && mState != GameState.PAUSED && Player != null && Player.AliveAndWell && Player.AI != null)
		{
			mSilentMove = false;
			if (Player.AI.HandleWorldTap(screenPos, true, force) == FUnitAIBase.InputResult.Handled)
			{
				ClearMoveIndicator();
				return true;
			}
		}
		return false;
	}

	public void ClearMoveIndicator()
	{
		if (mPlayerCurrentMoveIndicator != null)
		{
			mPlayerCurrentMoveIndicator = null;
			mTileHighlight.SetVisible(false);
		}
		else
		{
			if (mPlayerCurrentUnitIndicator == null || !(mPlayerCurrentUnitIndicator.GO != null))
			{
				return;
			}
			if (mPlayerCurrentUnitFlashRoutine != null)
			{
				StopCoroutine(mPlayerCurrentUnitFlashRoutine);
				if (mPlayerCurrentUnitIndicator.GO != null)
				{
					mPlayerCurrentUnitIndicator.GO.GetComponentInChildren<SpriteRenderer>().material.shader = Shader.Find("Sprites/Default");
				}
				mPlayerCurrentUnitFlashRoutine = null;
			}
			mPlayerCurrentUnitIndicator = null;
		}
	}

	public virtual bool HandleWorldHold(Vector3 screenPos)
	{
		if (mInputEnabled && mState != GameState.PAUSED && Player != null && Player.AliveAndWell && Player.AI != null)
		{
			mSilentMove = true;
			if (Player.AI.HandleWorldTap(screenPos, false, false) == FUnitAIBase.InputResult.Handled)
			{
				ClearMoveIndicator();
				return true;
			}
		}
		return false;
	}

	// PC port (new): suppress the tap indicator for the next keyboard / gamepad driven move.
	public void PCSilentMove()
	{
		mSilentMove = true;
	}

	public bool SetMoveIndicator(FPoint2 point)
	{
		ClearMoveIndicator();
		if (mSilentMove)
		{
			mSilentMove = false;
			return false;
		}
		mPlayerCurrentMoveIndicator = GetTile(point);
		mTileHighlight.GO.transform.position = mPlayerCurrentMoveIndicator.Position.ToVector2();
		mTileHighlight.SyncTilePosition();
		mTileHighlight.SetVisible(true);
		mTileHighlight.PlayAnim("Spawn", loop: false, null, fireTags: true);
		return true;
	}

	public bool SetMoveIndicator(FUnit unit)
	{
		mPlayerCurrentUnitIndicator = unit;
		mPlayerCurrentUnitFlashRoutine = StartCoroutine(FlashCurrentUnit());
		return true;
	}

	protected IEnumerator FlashCurrentUnit()
	{
		yield return StartCoroutine(DoFlashUnit(mPlayerCurrentUnitIndicator));
	}

	public FRoom.SpawnUnitResult SpawnUnitAtPosition(string unitName, FPoint2 pos, out int unitID, bool force = false)
	{
		FProtoUnitStartData fProtoUnitStartData = new FProtoUnitStartData();
		fProtoUnitStartData.Position = pos;
		fProtoUnitStartData.Facing = FAnimEntry.AnimFacing.DOWN;
		fProtoUnitStartData.ProtoUnit = unitName;
		fProtoUnitStartData.Name = unitName;
		return mCurrentRoom.SpawnUnit(fProtoUnitStartData, out unitID, force);
	}

	public void FlashUnit(FUnit unit)
	{
		StartCoroutine(DoFlashUnit(unit));
	}

	protected IEnumerator DoFlashUnit(FUnit unit)
	{
		if (mState == GameState.PAUSED || mState == GameState.ANIM_PAUSED)
		{
			yield return 0f;
			yield break;
		}
		if (unit == null || unit.GO == null)
		{
			yield break;
		}
		Transform vis = unit.GO.transform.Find("Vis");
		if (vis == null)
		{
			yield break;
		}
		Material mat = vis.GetComponent<SpriteRenderer>().material;
		mat.shader = mFlashShader;
		yield return new WaitForSeconds(0.2f);
		mat.shader = mDefaultShader;
		yield return new WaitForSeconds(0.2f);
	}

	public void ResetUnitShader(FUnit unit)
	{
		if (unit != null && unit.GO != null)
		{
			Transform transform = unit.GO.transform.Find("Vis");
			if (!(transform == null))
			{
				transform.GetComponent<SpriteRenderer>().material.shader = mDefaultShader;
			}
		}
	}

	public void PlayerMoveComplete(bool playerInControl)
	{
		ClearMoveIndicator();
		if (m_mPlayerMoveCompleteAction != null)
		{
			m_mPlayerMoveCompleteAction();
		}
		if (mCameraTweening)
		{
			return;
		}
		FPoint2 playerPos = Player.TilePosition;
		FEntryData fEntryData = mCurrentRoom.ProtoRoom.EntryDatas.FirstOrDefault((FEntryData e) => e.Position == playerPos);
		if (fEntryData != null && fEntryData.LeadsTo.Valid())
		{
			string[] array = fEntryData.LeadsTo.Split('.');
			if (array.Length > 1)
			{
				FPlayerStartParams fPlayerStartParams = new FPlayerStartParams();
				fPlayerStartParams.EntryPoint = array[1];
				fPlayerStartParams.ShowChapterTitle = fEntryData.ShowChapterTitle;
				TryWorldTransition(array[0], null, fPlayerStartParams);
				return;
			}
			FLog.Warning("Entry " + fEntryData.Name + " has LeadsTo property with incorrect formatting: " + fEntryData.LeadsTo);
		}
		FRoom newRoom;
		FPoint2 newPlayerPos;
		if (CanTransitionToNewRoom(out newRoom, out newPlayerPos))
		{
			mInputEnabled = false;
			Player.AI.StopActions();
			SetCurrentRoom(newRoom, playerInControl, animateCamera: true);
			ForceMovePlayerToPoint(newPlayerPos);
		}
	}

	public bool CanTransitionToNewRoom(out FRoom newRoom, out FPoint2 newPlayerPos, FPoint2 playerPos = default(FPoint2), bool requirePassableTerrain = true)
	{
		int x = mCurrentRoom.X;
		int y = mCurrentRoom.Y;
		int height = mCurrentRoom.Height;
		int width = mCurrentRoom.Width;
		if (playerPos == default(FPoint2))
		{
			playerPos = Player.TilePosition;
		}
		newPlayerPos = FPoint2.zero;
		newRoom = null;
		FPoint2 delta = FPoint2.zero;
		if (x == playerPos.x)
		{
			delta = new FPoint2(-1, 0);
			newPlayerPos = playerPos + delta;
			newRoom = GetRoomFromPoint(newPlayerPos, requirePassableTerrain);
		}
		if (x + width - 1 == playerPos.x && newRoom == null)
		{
			delta = new FPoint2(1, 0);
			newPlayerPos = playerPos + delta;
			newRoom = GetRoomFromPoint(newPlayerPos, requirePassableTerrain);
		}
		if (y == playerPos.y && newRoom == null)
		{
			delta = new FPoint2(0, -1);
			newPlayerPos = playerPos + delta;
			newRoom = GetRoomFromPoint(newPlayerPos, requirePassableTerrain);
		}
		if (y + height - 1 == playerPos.y && newRoom == null)
		{
			delta = new FPoint2(0, 1);
			newPlayerPos = playerPos + delta;
			newRoom = GetRoomFromPoint(newPlayerPos, requirePassableTerrain);
		}
		newPlayerPos = newPlayerPos + delta;
		return newRoom != null;
	}

	public bool CanTransitionToNewRoom()
	{
		FRoom newRoom;
		FPoint2 newPlayerPos;
		return CanTransitionToNewRoom(out newRoom, out newPlayerPos);
	}

	public void ForceMovePlayerToPoint(FPoint2 point, bool checkCollision = true, Action moveCompleteAction = null)
	{
		if (!checkCollision)
		{
			Player.Obstructs = false;
		}
		Action<FUnit> done = delegate
		{
			if (!checkCollision)
			{
				Player.Obstructs = true;
			}
			if (moveCompleteAction != null)
			{
				moveCompleteAction();
			}
		};
		Player.AI.DoMoveToPoint(point, 0, false, checkCollision, checkCollision, Player.InputEnabled ? null : "FAKE_ANIMATION", -1f, null, done, done);
	}

	public void PlayerPositionChanged()
	{
	}

	protected FRoom GetRoomFromPoint(FPoint2 point, bool requirePassableTerrain = true)
	{
		for (int i = 0; i < mRooms.Count; i++)
		{
			if (mRooms[i] == null)
			{
				FLog.Error("Room is null in GetRoomFromPoint!");
			}
			else if (mRooms[i].ContainsFPoint(point))
			{
				FTile tile = mRooms[i].GetTile(point);
				if (tile.TerrainPassable || !requirePassableTerrain)
				{
					return mRooms[i];
				}
			}
		}
		return null;
	}

	public void TryDoorTransition(FUnit door)
	{
		if (door != null && Player != null && !door.Locked)
		{
			StartCoroutine(DoDoorTransition(door));
		}
	}

	protected IEnumerator DoDoorTransition(FUnit door)
	{
		mInputEnabled = false;
		yield return new WaitForSeconds(0.5f);
		mDoorTransitionSafetyRoutine = FGameUtil.StartCoroutine(DoDoorTransitionSafetyTimer());
		mPlayerMoveCompleteAction -= DoorTransitionFinished;
		mPlayerMoveCompleteAction += DoorTransitionFinished;
		ForceMovePlayerToPoint(door.TilePosition);
	}

	protected void DoorTransitionFinished()
	{
		mInputEnabled = true;
		mPlayerMoveCompleteAction -= DoorTransitionFinished;
		if (mDoorTransitionSafetyRoutine != null)
		{
			FGameUtil.StopCoroutine(mDoorTransitionSafetyRoutine);
			mDoorTransitionSafetyRoutine = null;
		}
	}

	protected IEnumerator DoDoorTransitionSafetyTimer()
	{
		yield return new WaitForSeconds(2f);
		DoorTransitionFinished();
	}

	public void BeginBossEncounter(FUnit boss)
	{
		FApp.HUD.PageLegendWorld.ShowBossHealth(true);
		if (boss != null)
		{
			FApp.HUD.PageLegendWorld.SetBossName(FLoc.GetString(boss.ProtoUnit.Name + ".Name"));
			FApp.HUD.PageLegendWorld.SetBossHealth(boss.HP - 1, boss.ProtoUnit.Health - 1);
		}
		FApp.SoundManager.PlayMusic("BossTheme", 0.25f, 0.25f, true, 0f);
		mBossEncounter = true;
	}

	public void EndBossEncounter(bool win)
	{
		FApp.HUD.PageLegendWorld.ShowBossHealth(false);
		if (!win)
		{
			PlayLevelMusic();
		}
		else
		{
			mCurrentRoom.CompleteEnemyEncounter();
			FApp.SoundManager.FadeOutMusic(0.25f);
			FApp.SoundManager.PlaySoundSet("FanfareTheme");
			FGameUtil.DelayExecute(6f, CompleteBossEncounterWin);
		}
		mBossEncounter = false;
	}

	protected void CompleteBossEncounterWin()
	{
		PlayLevelMusic();
	}

	public FPather.PathResult FindPath(FUnit unit, FPoint2 startPosition, FUnit targetUnit, FPoint2 goalPosition, int range, List<int> ignoreUnits, FPath resultPath, bool singleAxis, bool checkUnits = true)
	{
		FPoint2 minTile = mCurrentRoom.ClampPoint(startPosition - new FPoint2(12, 8));
		FPoint2 maxTile = mCurrentRoom.ClampPoint(startPosition + new FPoint2(12, 8));
		resultPath.Clear();
		mPather.CleanUp();
		if (ignoreUnits != null)
		{
			mPather.AddIgnoreUnits(ignoreUnits, false);
		}
		return mPather.FindPath(startPosition, goalPosition, unit, targetUnit, resultPath, range, false, minTile, maxTile, singleAxis, checkUnits);
	}

	public void TryWorldTransition(string mapName, FUnit door, FPlayerStartParams startParams = null)
	{
		if (FApp.WorldManager.ProtoWorlds.ContainsKey(mapName))
		{
			StartCoroutine(DoWorldTransition(mapName, door, startParams));
		}
	}

	protected IEnumerator DoWorldTransition(string mapName, FUnit door, FPlayerStartParams startParams = null)
	{
		FApp.PlayerStats.SavePlayerState(mCurrentRoom);
		mInputEnabled = false;
		if (door != null)
		{
			Player.AI.DoMoveToPoint(door, 0, false);
		}
		FApp.HUD.PageLegendWorld.DimWorld(1f, 0.5f);
		yield return new WaitForSeconds(0.5f);
		if (mapName != ProtoWorld.Name)
		{
			FApp.DestroyWorld(false);
			FPluginRelay.Instance.StartCoroutine(Init(mapName, startParams));
			yield break;
		}
		mStartParams = startParams;
		FRoom foundRoom = null;
		FPoint2 pos = FPoint2.invalid;
		for (int i = 0; i < mRooms.Count; i++)
		{
			FRoom room = mRooms[i];
			if (startParams == null || string.IsNullOrEmpty(startParams.EntryPoint))
			{
				if (FApp.PlayerStats.CurrentRoom.Valid() && FApp.PlayerStats.CurrentWorld == ProtoWorld.Name && room.ProtoRoom.Name == FApp.PlayerStats.CurrentRoom)
				{
					pos = room.GetEntrance(room.EntranceUsed);
					foundRoom = room;
				}
			}
			else
			{
				FEntryData fEntryData = room.ProtoRoom.EntryDatas.FirstOrDefault((FEntryData e) => e.Name == startParams.EntryPoint);
				if (fEntryData != null)
				{
					pos = fEntryData.Position;
					foundRoom = room;
				}
			}
		}
		if (foundRoom == null)
		{
			yield break;
		}
		if (foundRoom != mCurrentRoom)
		{
			ForceChangeCurrentRoom(foundRoom, false);
		}
		Player.SetTilePosition(pos, force: true);
		mCurrentRoom.PlayerEntranceComplete();
		PlayStartSound();
		FApp.HUD.PageLegendWorld.DimWorld(0f, 0.5f);
		Player.AI.StopActions();
		yield return new WaitForSeconds(0.5f);
		mInputEnabled = true;
	}

	public bool HasDialogue(FUnit unit)
	{
		return FApp.DialogManager.GetDialogLine(ProtoWorld.Name, unit.ProtoUnit.Name) != null;
	}

	public bool HasDialogue(string dialogueName)
	{
		return FApp.DialogManager.GetDialogLine(ProtoWorld.Name, dialogueName) != null;
	}

	public void TryShowDialogue(FUnit unit)
	{
		TryShowDialogue(ProtoWorld.Name, unit.ProtoUnit.Name, unit);
	}

	public void TryShowDialogue(string world, string unit, FUnit unitTarget = null)
	{
		FDialogueLine[] dialogLines = FApp.DialogManager.GetDialogLines(world, unit, true);
		if (dialogLines != null)
		{
			Vector3 vp = mCamera.WorldToViewportPoint(Player.Position);
			FPageLegendWorld.DialoguePlacement placement = (0.5f <= vp.y) ? FPageLegendWorld.DialoguePlacement.BOTTOM : FPageLegendWorld.DialoguePlacement.TOP;
			FApp.HUD.PageLegendWorld.DisplayDialogue(dialogLines, unitTarget ?? Player, placement);
		}
	}

	public void SpawnEffect(string animDefName, Transform parent, Vector2 position, int sortingOrder = 200, bool positionIsRelative = false)
	{
		FAnimDef animDef = FAnimDefManager.GetAnimDef(animDefName);
		if (animDef == null)
		{
			FLog.UnityWarning("No anim def for visual layer '" + animDefName + "' found!");
			return;
		}
		int animTypeID = FAnimDefManager.GetAnimTypeID("Play");
		if (animTypeID < 0)
		{
			return;
		}
		GameObject gameObject = new GameObject(ProtoWorld.Name + "_EffectAnim_" + animDefName);
		SpriteRenderer spriteRenderer = gameObject.GetOrCreateComponent<SpriteRenderer>();
		spriteRenderer.sortingLayerName = "Foreground";
		gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
		gameObject.transform.parent = parent;
		if (!positionIsRelative)
		{
			gameObject.transform.position = position;
		}
		else
		{
			gameObject.transform.localPosition = position;
		}
		gameObject.transform.localEulerAngles = Vector3.zero;
		gameObject.transform.SetAsFirstSibling();
		FAnimationController fAnimationController = new FAnimationController(animDefName, animDef, gameObject);
		fAnimationController.SetFacing(FAnimEntry.AnimFacing.DOWN);
		fAnimationController.PlayAnim(animTypeID, false);
		mActiveEffects.Add(fAnimationController);
	}

	public void SpawnEffect(string animDefName, Vector2 position, int sortingOrder = 200)
	{
		SpawnEffect(animDefName, base.transform, position, sortingOrder);
	}

	public void SpawnEffect(string animDefName, FUnit unit, Vector2 offset = default(Vector2), int sortingOrder = 200)
	{
		Vector2 position = offset + (Vector2)unit.Position;
		SpawnEffect(animDefName, unit.GO.transform, position, sortingOrder);
	}

	public FAnimationController SpawnDiscreteAnimation(string animDefName, int sortingOrder = 200, string animName = "Play")
	{
		FAnimDef animDef = FAnimDefManager.GetAnimDef(animDefName);
		if (animDef == null)
		{
			FLog.UnityWarning("No anim def for visual layer '" + animDefName + "' found!");
			return null;
		}
		int animTypeID = FAnimDefManager.GetAnimTypeID(animName);
		if (animTypeID < 0)
		{
			return null;
		}
		GameObject gameObject = new GameObject(ProtoWorld.Name + "_EffectAnim_" + animDefName);
		SpriteRenderer spriteRenderer = gameObject.GetOrCreateComponent<SpriteRenderer>();
		spriteRenderer.sortingLayerName = "Foreground";
		gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
		gameObject.transform.parent = mCurrentRoom.transform;
		gameObject.transform.localEulerAngles = Vector3.zero;
		gameObject.transform.SetAsFirstSibling();
		FAnimationController fAnimationController = new FAnimationController(animDefName, animDef, gameObject);
		fAnimationController.SetFacing(FAnimEntry.AnimFacing.DOWN);
		fAnimationController.PlayAnim(animTypeID, true);
		return fAnimationController;
	}

	public void TryTriggerReveal(FUnit door, string messageToSend = "Trigger")
	{
		if (door != null)
		{
			FUnitTriggerRevealDesc fUnitTriggerRevealDesc = new FUnitTriggerRevealDesc();
			fUnitTriggerRevealDesc.Unit = door;
			fUnitTriggerRevealDesc.Message = messageToSend;
			mTriggerReveals.Add(fUnitTriggerRevealDesc);
			if (mTriggerRevealRoutine == null)
			{
				mTriggerRevealRoutine = StartCoroutine(DoTriggerReveal());
			}
		}
	}

	protected IEnumerator DoTriggerReveal()
	{
		if (!Player.AliveAndWell)
		{
			yield break;
		}
		mInputEnabled = false;
		mCameraTweening = true;
		mState = GameState.LOGIC_PAUSED;
		PlaySecretGet();
		yield return new WaitForSeconds(0.75f);
		FRoom currentRoom = mCurrentRoom;
		IGrouping<FRoom, FUnitTriggerRevealDesc>[] triggerGroups = mTriggerReveals.GroupBy((FUnitTriggerRevealDesc r) => r.Unit.Room).ToArray();
		FUnitMessage unitMsg = new FUnitMessage();
		unitMsg.SenderID = 0;
		unitMsg.Message = "NoReveal";
		for (int i = 0; i < triggerGroups.Length; i++)
		{
			IGrouping<FRoom, FUnitTriggerRevealDesc> group = triggerGroups[i];
			Vector2 averagePos = group.Aggregate(Vector2.zero, (Vector2 acc, FUnitTriggerRevealDesc d) => acc + (Vector2)d.Unit.Position);
			averagePos /= (float)group.Count();
			if (group.Key != currentRoom)
			{
				FApp.HUD.PageLegendWorld.DimWorld(1f, 0.5f);
				yield return new WaitForSeconds(0.5f);
				mState = GameState.RUNNING;
				group.Key.DoUpdate(0f);
				mState = GameState.LOGIC_PAUSED;
				SetCurrentRoom(group.Key, false);
				FApp.HUD.PageLegendWorld.DimWorld(0f, 0.5f);
				yield return new WaitForSeconds(0.5f);
				MoveCameraToPosition(averagePos, 4f);
				yield return new WaitForSeconds(0.75f);
				foreach (FUnitTriggerRevealDesc reveal in group)
				{
					unitMsg.RecipientID = reveal.Unit.ID;
					unitMsg.Message = reveal.Message;
					reveal.Unit.ProcessMessage(unitMsg);
				}
				FApp.SoundManager.PlaySoundSet("UISuccess");
				yield return new WaitForSeconds(1.5f);
				group.Key.RemovePendingUnits();
				FApp.PlayerStats.AddPendingSaveRoom(group.Key);
			}
			else
			{
				MoveCameraToPosition(averagePos, 3f);
				yield return new WaitForSeconds(0.75f);
				foreach (FUnitTriggerRevealDesc reveal2 in group)
				{
					unitMsg.RecipientID = reveal2.Unit.ID;
					unitMsg.Message = reveal2.Message;
					reveal2.Unit.ProcessMessage(unitMsg);
				}
				FApp.SoundManager.PlaySoundSet("UISuccess");
				yield return new WaitForSeconds(0.75f);
			}
		}
		if (mCurrentRoom != currentRoom)
		{
			FApp.HUD.PageLegendWorld.DimWorld(1f, 0.5f);
			yield return new WaitForSeconds(0.5f);
			SetCurrentRoom(currentRoom, false);
			FApp.HUD.PageLegendWorld.DimWorld(0f, 0.5f);
			yield return new WaitForSeconds(0.5f);
		}
		MoveCameraToPosition(Player.Position, 4f, CameraMoveComplete);
		mTriggerReveals.Clear();
		mTriggerRevealRoutine = null;
		yield return new WaitForSeconds(0.75f);
		mState = GameState.RUNNING;
	}

	public void MoveCameraToPosition(Vector3 pos, float orthoSize = 4f, Action<AbstractGoTween> onComplete = null)
	{
		SyncCameraExtents(orthoSize);
		Vector3 clamped = GetClampedCameraPos(pos);
		mCurrentCenterPoint = clamped;
		Go.killAllAbstractTweensWithTarget(mCamera.transform);
		Go.to(mCamera.transform, 0.75f, new GoTweenConfig().position(mCurrentCenterPoint).setEaseType(GoEaseType.QuadInOut));
		Go.to(this, 0.75f, new GoTweenConfig().floatProp("CameraOrthographicSize", orthoSize).setEaseType(GoEaseType.QuadInOut).onComplete(onComplete));
	}

	public void ZoomInToPosition(Vector3 pos, Action<AbstractGoTween> onComplete = null)
	{
		mCameraTweening = true;
		MoveCameraToPosition(pos, 4f, onComplete);
	}

	public void ZoomOut()
	{
		MoveCameraToPosition(Player.Position, 4f, delegate
		{
			mCameraTweening = false;
		});
	}

	public void TryFanfareLootReveal(string protoName, Action fanfareCompleteAction = null)
	{
		FProtoUnit protoUnit = FApp.ProtoUnitManager.GetProtoUnit(protoName, true);
		if (protoUnit != null)
		{
			StartCoroutine(DoFanfareLootReveal(protoUnit, fanfareCompleteAction));
		}
	}

	protected IEnumerator DoFanfareLootReveal(FProtoUnit proto, Action fanfareCompleteAction = null)
	{
		mState = GameState.LOGIC_PAUSED;
		mInputEnabled = false;
		Player.AI.StopActions();
		Player.PlayAnim("Fanfare_Intro", loop: false, null, fireTags: true);
		Player.QueueAnim("Fanfare_Idle", loop: true);
		Player.QueueAnim("Fanfare_Outro", loop: false);
		Player.QueueAnim("Idle", loop: true);
		if (proto.IsTag("PlaySecretGet"))
		{
			PlaySecretGet(true);
		}
		else
		{
			FApp.SoundManager.PlaySoundSet("UISuccess");
		}
		mCameraTweening = true;
		MoveCameraToPosition(Player.Position);
		yield return new WaitForSeconds(0.5f);
		FProtoUnitStartData pusd = new FProtoUnitStartData();
		pusd.Name = "LootReveal";
		pusd.ProtoUnit = proto.Name;
		pusd.Position = Player.TilePosition;
		pusd.IsFanfareReveal = true;
		FUnit reveal = new FUnit(mCurrentRoom, pusd, mNextUnitID++);
		reveal.Create(mCurrentRoom.gameObject, "");
		reveal.SetAI("");
		mCurrentRoom.Units.Add(reveal);
		reveal.SetSortingOrder(120);
		reveal.GO.transform.position = Player.Position + Vector3.up * 1.25f;
		reveal.GO.transform.localScale = Vector3.zero;
		Go.to(reveal.GO.transform, 0.25f, new GoTweenConfig().scale(1f).setEaseType(GoEaseType.BackOut));
		yield return new WaitForSeconds(1.25f);
		if (proto.Name == "Eggo")
		{
			FApp.HUD.PushPage(FApp.HUD.PageDungeonComplete, true, false);
			while (mState == GameState.PAUSED)
			{
				yield return 0f;
			}
		}
		yield return new WaitForSeconds(1f);
		ZoomOut();
		float shortWait = 0.375f;
		Go.to(reveal.GO.transform, shortWait, new GoTweenConfig().scale(0f).setEaseType(GoEaseType.BackIn));
		yield return new WaitForSeconds(shortWait);
		RemoveUnit(reveal);
		Player.AnimController.EndCurrentLoop();
		yield return new WaitForSeconds(shortWait);
		if (!FApp.HUD.PageLegendWorld.DialogueOnScreen)
		{
			mState = GameState.RUNNING;
		}
		mInputEnabled = true;
		if (fanfareCompleteAction != null)
		{
			fanfareCompleteAction();
		}
	}

	public void HackWinPrototype()
	{
		StartCoroutine(DoHackWinPrototype());
	}

	protected IEnumerator DoHackWinPrototype()
	{
		float waitTime = 2f;
		while (waitTime > 0f && !mInputEnabled)
		{
			waitTime -= Time.deltaTime;
			yield return 0f;
		}
		Player.PlayAnim("Fanfare_Intro", loop: false, null, fireTags: true);
		Player.QueueAnim("Fanfare_Idle", loop: false);
		Player.QueueAnim("Fanfare_Outro", loop: false);
		FApp.SoundManager.PlaySoundSet("UISuccess");
	}

	public void HackDoEnding()
	{
		StartCoroutine(DoHackDoEnding());
	}

	protected IEnumerator DoHackDoEnding()
	{
		float waitTime = 2f;
		while (waitTime > 0f && !mInputEnabled)
		{
			waitTime -= Time.deltaTime;
			yield return 0f;
		}
		mInputEnabled = false;
		Player.EnableInput(false);
		mCameraTweening = true;
		FApp.SoundManager.PlaySoundSet("FanfareTheme");
		Go.to(mCamera.transform, 6f, new GoTweenConfig().position(mCamera.transform.position + Vector3.up * 6f).setEaseType(GoEaseType.QuadInOut));
		FApp.HUD.PageLegendWorld.DimWorld(1f, 4f);
		yield return new WaitForSeconds(6f);
		FApp.DestroyWorld(false);
		FPluginRelay.Instance.StartCoroutine(Init("BasementEnding"));
	}

	public int GetUnitsByCircleCast(Vector2 pos, float radius, List<FUnit> unitsHit)
	{
		int num = Physics2D.OverlapCircleNonAlloc(pos, radius, mCollidersHit);
		unitsHit.Clear();
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = mCollidersHit[i];
			if (!collider2D.isTrigger)
			{
				FUnit unitFromGO = FApp.World.GetUnitFromGO(collider2D.gameObject);
				if (unitFromGO != null)
				{
					unitsHit.Add(unitFromGO);
				}
			}
		}
		return unitsHit.Count;
	}

	public int GetTilesByCircleCast(Vector2 pos, float radius, List<FTile> tilesHit, bool onlyObstructing = true)
	{
		int num = Physics2D.OverlapCircleNonAlloc(pos, radius, mCollidersHit);
		tilesHit.Clear();
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = mCollidersHit[i];
			if (!collider2D.isTrigger || !onlyObstructing)
			{
				FTile tileFromGO = FApp.World.GetTileFromGO(collider2D.gameObject);
				if (tileFromGO != null)
				{
					tilesHit.Add(tileFromGO);
				}
			}
		}
		return tilesHit.Count;
	}

	public void DimWorld(bool dim)
	{
		if (!(mCurrentRoom == null))
		{
			SetCoverAlpha(dim ? 0.5f : 0f);
		}
	}

	public void SetCoverAlpha(float alpha)
	{
		if (!(mCover == null))
		{
			mCover.color = new Color(0f, 0f, 0f, alpha);
			mCover.gameObject.SetActive(0f < alpha);
			if (mRenderStage != null)
			{
				mRenderStage.Alpha = 1f - alpha;
			}
		}
	}

	public void TintWorld(Color color, bool force = false)
	{
		if (mTileHighlight != null)
		{
			mTileHighlight.SetColor(color);
			Player.SetColor(color);
			if (mCurrentRoom != null)
			{
				mCurrentRoom.SetAllTileColor(color);
				for (int i = 0; i < mCurrentRoom.Units.Count; i++)
				{
					mCurrentRoom.Units[i].SetColor(color, false, force);
				}
			}
		}
	}

	public void SpawnFlyOffText(string text, Vector3 position, Vector2? offset = null)
	{
		// Rewritten from libil2cpp.so 1.0.605 (FWorld.SpawnFlyOffText @ 0x175db6c).
		if (mRenderStage == null)
		{
			return;
		}
		Vector2 labelOffset = offset ?? Vector2.zero;
		FNode holderNode = new FNode("FlyoffText_" + mFlyoffTexts.Count.ToString());
		holderNode.SetParent(mRenderStage);
		FLabel label = new FLabel("DefaultStyle", text, 32f, false, true, false);
		label.Color = new Color(1f, 0f, 0f, 1f);
		label.SetParent(holderNode);
		Vector3 screen = GetScreenPosition(position);
		holderNode.DockInside(mRenderStage, InnerDock.Center, 96f, 64f, screen.x, screen.y);
		label.DockInside(holderNode, InnerDock.Center, 96f, 64f, labelOffset.x, labelOffset.y);
		FEntityAction[] spawned = new FEntityAction[3]
		{
			new FEntityActionTweenPropertyToFloat(1f, "y", label.y + 125f),
			new FEntityActionTweenPropertyToFloat(1f, "Alpha", 0f, GoEaseType.CircIn),
			new FEntityActionTweenPropertyToColor(0.25f, "Color", new Color(1f, 1f, 1f, 1f), GoEaseType.CircIn)
		};
		label.AddAction(new FEntityActionSequence(new FEntityActionSpawn(spawned), new FEntityActionCallMethod(delegate
		{
			mFlyoffTexts.Remove(holderNode);
			FNode.Cleanup(holderNode);
		})));
		mFlyoffTexts.Add(holderNode);
	}

	protected void UpdateUINodePosition(FNode node, Vector3 startPos, Vector3 currPos)
	{
		Vector3 a = GetScreenPosition(startPos);
		Vector3 b = GetScreenPosition(currPos);
		node.x += a.x - b.x;
		node.y += a.y - b.y;
	}

	// Map overlay (rewritten from libil2cpp.so 1.0.605: ZoomOut 0x175e204, SetWorldMapBounds 0x175e478, SetWorldMapPosition 0x175e5c4,
	// ZoomIn 0x175e688, ShowMap 0x175e8fc, PlaceMapmarker 0x175f8c4, SyncMapMarkers 0x175fc48, BuildMap 0x1760324).
	public bool WorldMapZoomOut()
	{
		if (mWorldMapGO == null || mWorldMapParams == null)
		{
			return false;
		}
		float current = mWorldMapGO.transform.localScale.x;
		float target = Mathf.Clamp(current - mWorldMapParams.ScaleInterval, mWorldMapParams.MinScale, mWorldMapParams.MaxScale);
		if (Mathf.Approximately(current, target))
		{
			return false;
		}
		mWorldMapGO.transform.localScale = Vector3.one * target;
		SetWorldMapBounds();
		SetWorldMapPosition(mWorldMapGO.transform.localPosition);
		return true;
	}

	public bool WorldMapZoomIn()
	{
		if (mWorldMapGO == null || mWorldMapParams == null)
		{
			return false;
		}
		float current = mWorldMapGO.transform.localScale.x;
		float target = Mathf.Clamp(current + mWorldMapParams.ScaleInterval, mWorldMapParams.MinScale, mWorldMapParams.MaxScale);
		if (Mathf.Approximately(current, target))
		{
			return false;
		}
		mWorldMapGO.transform.localScale = Vector3.one * target;
		SetWorldMapBounds();
		SetWorldMapPosition(mWorldMapGO.transform.localPosition);
		return true;
	}

	protected void SetWorldMapBounds()
	{
		if (mWorldMapParams == null)
		{
			return;
		}
		float width = mWorldMapParams.Width;
		float height = mWorldMapParams.Height;
		float ortho = CameraOrthographicSize;
		float halfViewW = ortho * (float)PCDisplay.Width / (float)PCDisplay.Height * mCamera.rect.width;
		float minX = halfViewW - width * 0.5f;
		float minY = ortho - height * 0.5f;
		float maxX = Mathf.Max(width * 0.5f - halfViewW, minX);
		float maxY = Mathf.Max(height * 0.5f - ortho, minY);
		float adjX = (width < halfViewW * 2f) ? ((halfViewW * 2f - width) * 0.5f) : 0f;
		float adjY = (height < ortho * 2f) ? ((ortho * 2f - height) * 0.5f) : 0f;
		float x = minX - adjX;
		float y = minY - adjY;
		mWorldMapParams.Bounds = new Rect(x, y, maxX - adjX - x, maxY - adjY - y);
	}

	// Port helper: FUtil.ClampToViewRect (0x1671f8c) is still a stub in the firstpass assembly, so the same math lives here.
	private static Vector2 ClampToViewRectLocal(Camera cam, Vector2 position, Vector2 size)
	{
		float ortho = cam.orthographicSize;
		float halfW = ortho * (float)PCDisplay.Width / (float)PCDisplay.Height * cam.rect.width;
		Vector3 camPos = cam.transform.position;
		float x = Mathf.Min(position.x, halfW + camPos.x - size.x * 0.5f);
		x = Mathf.Max(x, camPos.x - halfW + size.x * 0.5f);
		float y = Mathf.Min(position.y, ortho + camPos.y - size.y * 0.5f);
		y = Mathf.Max(y, camPos.y - ortho + size.y * 0.5f);
		return new Vector2(x, y);
	}

	// Clamps the arrow / marker icon to the camera view and dims it while it is off screen.
	private void ClampMapIcon(Transform icon, GameObject colorTarget)
	{
		Vector3 pos = icon.position;
		Vector2 clamped = ClampToViewRectLocal(mCamera, new Vector2(pos.x, pos.y), new Vector2(1.5f, 1.5f));
		icon.position = new Vector3(clamped.x, clamped.y, pos.z);
		float dx = pos.x - clamped.x;
		float dy = pos.y - clamped.y;
		float shade = (dx * dx + dy * dy >= 1E-10f) ? 0.6f : 1f;
		ColorAllSprites(colorTarget, new Color(shade, shade, shade, 1f));
	}

	public void ShowMap()
	{
		SetCoverAlpha(1f);
		mMapScrollVector = Vector2.zero;
		if (mWorldMapGO == null)
		{
			return;
		}
		mWorldMapGO.SetActive(true);
		Transform worldMapTransform = mWorldMapGO.transform;
		Transform worldNode = (ProtoWorld != null) ? worldMapTransform.Find(ProtoWorld.Name) : null;
		if (worldNode != null)
		{
			Go.killAllAbstractTweensWithTarget(mMapArrow);
			mMapArrow.parent = worldNode;
			Vector3 playerPos = Player.Position;
			mMapArrow.localPosition = new Vector3((playerPos.x + 0.5f) * 0.25f, (playerPos.y + 0.5f) * 0.25f, (playerPos.z + 0f) * 0.25f);
			Vector3 local = mCamera.transform.InverseTransformPoint(mMapArrow.position);
			ScrollWorldMap(new Vector2(0f - local.x, 0f - local.y), countAsScroll: false);
			mMapArrow.localScale = Vector3.zero;
			Go.to(mMapArrow, 0.5f, new GoTweenConfig().scale(1f).setEaseType(GoEaseType.BackOut));
			SpriteRenderer arrowRenderer = mMapArrow.GetComponentInChildren<SpriteRenderer>();
			arrowRenderer.sprite = Resources.Load<Sprite>(string.Format("Prefabs/UI/{0}_Head", FApp.PlayerStats.CurrentCharacter));
		}
		if (mMapMarkerParent == null)
		{
			mMapMarkerParent = mWorldMapGO.transform.Find("MapMarkersParent");
			if (mMapMarkerParent == null)
			{
				mMapMarkerParent = new GameObject("MapMarkersParent").transform;
				mMapMarkerParent.SetParent(mWorldMapGO.transform);
				mMapMarkerParent.localPosition = Vector3.zero;
			}
		}
		for (int num = mMapMarkerParent.childCount - 1; num >= 0; num--)
		{
			Transform child = mMapMarkerParent.GetChild(num);
			UnityEngine.Object.Destroy(child.gameObject);
			child.parent = null;
		}
		if (mWorldMapGO.transform.childCount == 2)
		{
			foreach (FRoom room in mRooms)
			{
				Transform roomNode = mWorldMapGO.transform.Find(ProtoWorld.Name + "/" + room.Name);
				if (roomNode != null)
				{
					roomNode.gameObject.SetActive(room.State != FRoom.RoomStates.Unvisited);
				}
			}
		}
		foreach (int questId in FApp.PlayerStats.CurrentQuests)
		{
			FProtoQuest protoQuest = FApp.ProtoQuestManager.GetProtoQuestByDBID(questId);
			if (protoQuest == null || protoQuest.Reminders == null)
			{
				continue;
			}
			foreach (QuestReminder reminder in protoQuest.Reminders)
			{
				if (reminder.MapMarker.Valid() && reminder.MapMarkerRevealed)
				{
					FMapMarker marker = FApp.MapMarkerManager.GetMapMarker(reminder.MapMarker);
					if (marker != null)
					{
						PlaceMapmarker(marker.Name, marker.Icon, marker.Map, marker.Position);
					}
				}
			}
		}
		List<FProtoWorld> mapWorlds = new List<FProtoWorld>();
		mapWorlds.AddRange(FApp.WorldManager.ProtoWorlds.Values.Where((FProtoWorld w) => w.MapPack == ProtoWorld.MapPack));
		bool isWorldMap = ProtoWorld.MapPack == "WorldMap";
		foreach (FProtoWorld mapWorld in mapWorlds)
		{
			foreach (FProtoUnitStartData blocker in mapWorld.GetStartDatasWithTag("MapBlocker"))
			{
				bool visited = FApp.PlayerStats.HasVisitedRoom(FApp.PlayerStats.GetRoomIdentifier(blocker.Room, mapWorld.Name));
				bool inCurrentRoom = mapWorld.Name == ProtoWorld.Name && blocker.Room == mCurrentRoom.Name;
				if (!(isWorldMap || inCurrentRoom || visited) || !blocker.GetProtoUnit.CheckSpawnRequirements())
				{
					continue;
				}
				bool alive;
				if (!inCurrentRoom)
				{
					alive = FApp.PlayerStats.IsUnitAlive(blocker, blocker.Room, mapWorld.Name);
				}
				else
				{
					FUnit existing = mCurrentRoom.GetUnitByUniqueID(FApp.PlayerStats.GetUnitIdentifier(blocker, mCurrentRoom));
					if (existing == null)
					{
						continue;
					}
					alive = existing.AliveAndWell;
				}
				if (alive)
				{
					PlaceMapmarker(Guid.NewGuid().ToString(), "Lock", mapWorld.Name, blocker.Position, shouldStayOnScreen: false);
				}
			}
		}
		SyncMapMarkers();
		mRenderStage.SetVisibleRecursive(false);
	}

	protected bool PlaceMapmarker(string name, string sprite, string map, FPoint2 position, bool shouldStayOnScreen = true)
	{
		if (position == FPoint2.invalid)
		{
			return false;
		}
		Transform mapNode = mWorldMapGO.transform.Find(map);
		if (mapNode == null)
		{
			return false;
		}
		Sprite markerSprite = Resources.Load<Sprite>(string.Format("Prefabs/UI/{0}", sprite));
		if (markerSprite == null)
		{
			return false;
		}
		string markerName = "MapMarker_" + name;
		Transform existing = mapNode.Find(markerName);
		GameObject markerGO;
		if (existing == null)
		{
			markerGO = FUtil.ClonePrefab("Prefabs/UI/MapArrow");
			markerGO.name = markerName;
			markerGO.transform.parent = mapNode;
		}
		else
		{
			markerGO = existing.gameObject;
		}
		SpriteRenderer renderer = markerGO.GetComponentInChildren<SpriteRenderer>();
		if (!shouldStayOnScreen)
		{
			markerGO.tag = "DontUpdate";
			renderer.sortingOrder -= 10;
		}
		renderer.sprite = markerSprite;
		markerGO.transform.localPosition = new Vector3(((float)position.x + 0.5f) * 0.25f, ((float)position.y + 0.5f) * 0.25f, 0f);
		markerGO.transform.parent = mMapMarkerParent;
		return true;
	}

	public void HideMap()
	{
		if (mWorldMapGO != null && mWorldMapGO.activeInHierarchy)
		{
			mWorldMapGO.SetActive(false);
			DimWorld(false);
			mRenderStage.SetVisibleRecursive(true);
		}
	}

	public void SyncMapMarkers()
	{
		if (mCamera == null || mMapArrow == null || Player == null)
		{
			return;
		}
		Vector3 playerPos = Player.Position;
		mMapArrow.localPosition = new Vector3(playerPos.x * 0.25f, playerPos.y * 0.25f, playerPos.z * 0.25f);
		ClampMapIcon(mMapArrow, mMapArrow.gameObject);
		if (mMapMarkerParent == null)
		{
			return;
		}
		for (int num = mMapMarkerParent.childCount - 1; num >= 0; num--)
		{
			Transform child = mMapMarkerParent.GetChild(num);
			if (!child.CompareTag("DontUpdate"))
			{
				Transform icon = child.GetChild(0);
				if (icon != null)
				{
					icon.localPosition = Vector2.zero;
					ClampMapIcon(icon, child.gameObject);
				}
			}
		}
	}

	public void PlaySecretGet(bool force = false)
	{
		if (FApp.SoundManager != null && (force || 1f < mGametime - mLastSecretGet))
		{
			mLastSecretGet = mGametime;
			FApp.SoundManager.PlaySoundSet("SecretGet");
		}
	}

	protected void ColorAllSprites(GameObject go, Color color)
	{
		SpriteRenderer[] componentsInChildren = go.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = color;
		}
	}

	protected void BuildMap()
	{
		if (mWorldMapGO != null)
		{
			if (mWorldMapGO.transform.Find(ProtoWorld.Name) != null)
			{
				return;
			}
			UnityEngine.Object.Destroy(mWorldMapGO);
		}
		string pack = ProtoWorld.MapPack;
		FProtoWorld[] packWorlds = FApp.WorldManager.ProtoWorlds.Values.Where((FProtoWorld world) => world.MapPack == pack).ToArray();
		FProtoWorld[][] columns;
		if (packWorlds.Count() < 2)
		{
			columns = new FProtoWorld[1][] { new FProtoWorld[1] { packWorlds[0] } };
		}
		else
		{
			columns = (from world in packWorlds
				orderby int.Parse(world.LoadData["Column"])
				group world by world.LoadData["Column"]).Select((IGrouping<string, FProtoWorld> g) => g.OrderBy((FProtoWorld world) => world.LoadData["Index"]).ToArray()).ToArray();
		}
		mWorldMapGO = new GameObject("MapHolder");
		mWorldMapGO.transform.parent = mCamera.transform;
		int totalWidth = 0;
		int maxHeight = 0;
		for (int i = 0; i < columns.Length; i++)
		{
			FProtoWorld[] column = columns[i];
			int y = 0;
			FProtoWorld last = null;
			for (int j = 0; j < column.Length; j++)
			{
				FProtoWorld world2 = column[j];
				if (world2 != null)
				{
					y -= world2.Height;
					LoadMapRooms(totalWidth, y, world2, mWorldMapGO.transform, 4f);
					last = world2;
				}
			}
			if (last != null)
			{
				totalWidth += last.Width;
			}
			maxHeight = Mathf.Max(Mathf.Abs(y), maxHeight);
		}
		float width = (float)totalWidth / 4f;
		float height = (float)maxHeight / 4f;
		for (int k = 0; k < mWorldMapGO.transform.childCount; k++)
		{
			Transform child = mWorldMapGO.transform.GetChild(k);
			Vector3 childPos = child.localPosition;
			child.localPosition = new Vector3(childPos.x - width * 0.5f, childPos.y + height * 0.5f, childPos.z + 0f);
		}
		float ortho = CameraOrthographicSize;
		float scale = 1f;
		float viewH = ortho * 2f * mCamera.rect.height;
		if (height < viewH)
		{
			float viewW = viewH * Futile.Screen.Width / Futile.Screen.Height * mCamera.rect.width;
			if (width < viewW)
			{
				scale = ((!(height > width)) ? (viewW / width) : (viewH / height));
			}
		}
		if (!Mathf.Approximately(1f, scale))
		{
			mWorldMapGO.transform.localScale = Vector3.one * scale;
		}
		mWorldMapParams = new WorldMapParams
		{
			Width = width * scale,
			Height = height * scale,
			MinScale = 1f,
			MaxScale = 1f,
			ScaleInterval = 0.2f
		};
		GameObject arrow = FUtil.ClonePrefab("Prefabs/UI/MapArrow");
		arrow.name = "MapArrow";
		mMapArrow = arrow.transform;
		mMapArrow.parent = mWorldMapGO.transform;
		SetWorldMapBounds();
		SetWorldMapPosition(Vector2.zero);
		mWorldMapGO.SetActive(false);
	}

	protected IEnumerator LoadRoomSprites()
	{
		FPoint2 currentRoomPoint = new FPoint2(mCurrentRoom.X, mCurrentRoom.Y);
		foreach (FRoom room in mRooms.OrderBy((FRoom r) => new FPoint2(r.X, r.Y).DistanceTo(currentRoomPoint)))
		{
			if (!(room == null))
			{
				Texture2D texture2D = Resources.Load<Texture2D>("Worlds/" + ProtoWorld.Name + "/" + room.Name);
				if (!(texture2D == null))
				{
					room.SetTexture(texture2D);
					yield return 0f;
				}
			}
		}
	}

	protected void LoadMapRooms(int xOffset, int yOffset, FProtoWorld world, Transform parent, float divideBy)
	{
		GameObject gameObject = new GameObject(world.Name);
		gameObject.transform.parent = parent;
		gameObject.transform.localPosition = new Vector3((float)xOffset / divideBy, (float)yOffset / divideBy, 0f);
		int num = 0;
		foreach (FProtoRoom protoRoom in world.ProtoRooms)
		{
			GameObject gameObject2 = new GameObject("Room_" + num.ToString());
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(protoRoom.Bounds.x / divideBy, protoRoom.Bounds.y / divideBy, 0f);
			SpriteRenderer spriteRenderer = gameObject2.GetOrCreateComponent<SpriteRenderer>();
			spriteRenderer.sortingLayerName = "Foreground";
			spriteRenderer.sortingOrder = 500;
			spriteRenderer.sprite = Resources.Load<Sprite>("WorldMaps/" + world.Name + "_Room_" + num.ToString());
			num++;
		}
	}

	protected void SetWorldMapPosition(Vector2 pos)
	{
		if (mWorldMapParams == null || mWorldMapGO == null)
		{
			return;
		}
		Rect bounds = mWorldMapParams.Bounds;
		float x = Mathf.Max(Mathf.Min(pos.x, bounds.x + bounds.width), bounds.x);
		float y = Mathf.Max(Mathf.Min(pos.y, bounds.y + bounds.height), bounds.y);
		mWorldMapGO.transform.localPosition = new Vector3(x, y, 5f);
		SyncMapMarkers();
	}

	public void MapScrollEnded()
	{
	}

	public void DelayExecute(float delaySeconds, Action function)
	{
		if (delaySeconds < 1E-05f)
		{
			function();
		}
		else
		{
			StartCoroutine(DoDelayExecute(delaySeconds, function));
		}
	}

	public IEnumerator DoDelayExecute(float delaySeconds, Action function)
	{
		float endTime = mGametime + delaySeconds;
		while (mGametime < endTime)
		{
			yield return 0f;
		}
		function();
	}

	public void WillResignActive()
	{
		if (FApp.HUD.TopPage == FApp.HUD.PageLegendWorld)
		{
			FApp.HUD.PageLegendWorld.ShowPauseMenu();
		}
	}

	public void FillPlayerHealth()
	{
		if (Player != null)
		{
			Player.SetHP(FApp.PlayerStats.MaxHealth);
			FApp.SoundManager.PlaySoundSet("HealthPickup");
			if (!(FApp.HUD == null) && FApp.HUD.PageLegendWorld != null)
			{
				FApp.HUD.PageLegendWorld.BounceHealth();
			}
		}
	}
}
