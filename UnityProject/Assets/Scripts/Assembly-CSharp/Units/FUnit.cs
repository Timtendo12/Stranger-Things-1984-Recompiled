using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnit @ 0x17170cc..0x172821c, Ghidra vaddr).
public class FUnit : IAnimTagResponder
{
	public enum FUnitMoveResult
	{
		Success = 0,
		Fail_Generic = 1,
		Fail_UnitCollision = 2
	}

	public const int cLowHP_1 = 3;

	public const int cLowHP_2 = 1;

	public const float cTimeBetweenFootsteps = 0.2f;

	public const float cAttackedCooldown = 0.2f;

	public const float cDefaultUnitRadius = 0.5f;

	protected FPoint2 mTilePosition;

	protected FUnitAIBase mAI;

	protected bool mObstructs;

	protected bool mAlive;

	protected bool mDying;

	protected bool mLocked;

	protected bool mIsDroppedLoot;

	protected bool mSpawnComplete;

	protected bool mInteractable;

	protected float mLastTimeAttacked;

	protected FAnimationController mAnimController;

	protected int mPurchasePrice;

	public int MaxDamageTakenPerHit;

	protected CircleCollider2D mCollider;

	protected List<FFloatyLoots> mFloatyLoots;

	protected List<int> mTriggeringUnits;

	protected List<FTile> mTilesOccupying;

	protected FAnimationController mCurrentEmote;

	protected Vector2 mCurrentEmoteOffset;

	public FAnimEntry.AnimFacing Facing;

	protected int mHP;

	public FProtoWeapon Weapon;

	protected FLoopingSoundHelper mLoopingSoundHelper;

	protected FUnitAttackDesc mLastAttack;

	protected FUnitAttackDesc mMyLastAttack;

	protected Dictionary<string, FAnimationController> mAnimLayers;

	protected List<FAnimationController> mAnimLayersToRemove;

	protected int mBaseSortingOrder;

	protected FAnimationController mCountdownAnim;

	private int mNextAnimID;

	protected string mUniqueID;

	protected float mLastFootstepTime;

	protected string mCurrentTileType;

	protected bool mShowTileVisibility;

	protected FNode mUINode;

	protected Vector3 mLastUINodePosition;

	protected FLabelDropshadow mPurchasePriceLabel;

	protected string mIdleAnim;

	protected bool mCallSpawnCompleted;

	protected int[] mUnitsToForwardMessages;

	protected Vector3 mUINodeSize;

	private List<FUnit> mUnitsHit;

	private List<FTile> mTilesHit;

	protected bool mCheckingCanPush;

	private RaycastHit2D[] mRaycastHits;

	public int ID { get; protected set; }

	public string Name { get; protected set; }

	public string LogName => ID.ToString() + "_" + ProtoUnit.Name;

	public FWorld World { get; protected set; }

	public FRoom Room { get; protected set; }

	public bool IsPlayer { get; set; }

	public FProtoUnit ProtoUnit { get; protected set; }

	public GameObject GO { get; protected set; }

	public string ProtoCollectibleName { get; set; }

	public FPoint2 TilePosition => mTilePosition;

	public Vector3 Position
	{
		get
		{
			if (!(GO == null))
			{
				return GO.transform.position;
			}
			return new Vector3(mTilePosition.x, mTilePosition.y, 0f);
		}
	}

	public FUnitAIBase AI => mAI;

	public bool Obstructs
	{
		get
		{
			return mObstructs;
		}
		set
		{
			SetObstructs(value);
		}
	}

	public bool AliveAndWell
	{
		get
		{
			if (StartData is FProtoDoorStartData && !mObstructs && !ProtoUnit.IsTag("KeepAliveAfterOpen"))
			{
				return false;
			}
			if (ProtoUnit.Health != 0 && !mAlive)
			{
				return false;
			}
			return !mDying;
		}
	}

	public bool Alive
	{
		get
		{
			if (ProtoUnit.Health != 0)
			{
				return mAlive;
			}
			return true;
		}
		set
		{
			mAlive = value;
			if (value)
			{
				mDying = false;
			}
			else
			{
				DoLoot();
			}
		}
	}

	public bool Dying
	{
		get
		{
			return mDying;
		}
		set
		{
			mDying = value;
		}
	}

	public bool Locked
	{
		get
		{
			return mLocked;
		}
		set
		{
			mLocked = value;
			mIdleAnim = (value ? "Idle_Locked" : "Idle");
			SetIdle();
		}
	}

	protected bool IsDroppedLoot => mIsDroppedLoot;

	public bool SpawnComplete => mSpawnComplete;

	public bool Interactable
	{
		get
		{
			return mInteractable;
		}
		set
		{
			mInteractable = value;
		}
	}

	public FAnimationController AnimController => mAnimController;

	public bool Visible
	{
		get
		{
			if (GO != null)
			{
				return GO.activeInHierarchy;
			}
			return false;
		}
	}

	public int PurchasePrice
	{
		get
		{
			if (mPurchasePrice > 0)
			{
				return mPurchasePrice;
			}
			return ProtoUnit.PurchasePrice;
		}
	}

	public CircleCollider2D Collider => mCollider;

	public Vector3 CenterPoint
	{
		get
		{
			if (!(mCollider == null))
			{
				return mCollider.bounds.center;
			}
			return Vector3.zero;
		}
	}

	public bool InputEnabled
	{
		get
		{
			if (mAI != null)
			{
				return mAI.InputEnabled;
			}
			return false;
		}
	}

	public bool IsOnScreen
	{
		get
		{
			if (World == null)
			{
				return false;
			}
			return World.IsUnitOnScreen(this);
		}
	}

	public float Speed
	{
		get
		{
			if (ProtoUnit != null)
			{
				return ProtoUnit.Speed * ProtoUnit.SpeedModifier;
			}
			return 1f;
		}
	}

	public float DamageTakenModifier => ProtoUnit.DamageTakenModifier;

	public float SpeedModifier => ProtoUnit.SpeedModifier;

	public FPoint2 LastTilePosition { get; protected set; }

	public Vector2 LastTilePositionVector => new Vector2(LastTilePosition.x, LastTilePosition.y);

	public bool PlayerCanBeAttacked => mLastTimeAttacked + 0.2f < World.Gametime;

	public int HP
	{
		get
		{
			return mHP;
		}
		set
		{
			SetHP(value);
		}
	}

	public FProtoUnitStartData StartData { get; protected set; }

	public FUnit AttackTarget
	{
		get
		{
			if (mAI == null)
			{
				throw new System.NullReferenceException();
			}
			if (mAI.GetCurrentAction() is FUnitActionAttack attack)
			{
				return World.GetUnitByID(attack.TargetID);
			}
			return null;
		}
	}

	public FPoint2 AttackPosition
	{
		get
		{
			if (mAI == null)
			{
				throw new System.NullReferenceException();
			}
			if (mAI.GetCurrentAction() is FUnitActionAttack attack && AttackTarget == null)
			{
				return attack.TargetTile;
			}
			return FPoint2.invalid;
		}
	}

	public FLoopingSoundHelper LoopingSoundHelper => mLoopingSoundHelper;

	public FUnitAttackDesc LastAttack => mLastAttack;

	public string UniqueID
	{
		get
		{
			if (FApp.PlayerStats == null)
			{
				return string.Empty;
			}
			if (mUniqueID == null)
			{
				return FApp.PlayerStats.GetUnitIdentifier(this);
			}
			return mUniqueID;
		}
	}

	public FUnit()
	{
		InitCollections();
	}

	public FUnit(FRoom room, FProtoUnitStartData pusd, int newID)
	{
		InitCollections();
		Room = room;
		World = room.World;
		StartData = pusd;
		ID = newID;
		SetProperties();
	}

	// Shared field initialisation of both shipped constructors (field initialisers in the original).
	private void InitCollections()
	{
		mObstructs = true;
		mAlive = true;
		mInteractable = true;
		mTilePosition = FPoint2.zero;
		MaxDamageTakenPerHit = -1;
		mFloatyLoots = new List<FFloatyLoots>();
		mTriggeringUnits = new List<int>();
		mTilesOccupying = new List<FTile>();
		Facing = FAnimEntry.AnimFacing.LEFT;
		mAnimLayersToRemove = new List<FAnimationController>();
		mCurrentTileType = "Tile";
		mLastUINodePosition = Vector3.zero;
		mIdleAnim = "Idle";
		mUINodeSize = new Vector3(96f, 96f, 0f);
		mUnitsHit = new List<FUnit>();
		mTilesHit = new List<FTile>();
		mRaycastHits = new RaycastHit2D[20];
	}

	public void Dump(string indention)
	{
		FLog.Log(indention + "[" + ID.ToString("D04") + "]: Name=" + Name + " (" + ProtoUnit.Name + (IsPlayer ? " (PLAYER)" : string.Empty) + ", TilePos=" + mTilePosition.ToString() + ")");
		bool alive = ProtoUnit.Health == 0 || mAlive;
		FLog.Log(indention + indention + "Alive=" + alive.ToString() + ", Dying=" + mDying.ToString() + ", Locked=" + mLocked.ToString());
		string tags = string.Empty;
		int count = 0;
		foreach (int tag in ProtoUnit.Tags)
		{
			if (count > 0)
			{
				tags += ", ";
			}
			tags += FTagManager.GetTagName(tag);
			count++;
			if (count > 5)
			{
				FLog.Log(indention + indention + "Tags=" + tags);
				tags = string.Empty;
				count = 0;
			}
		}
		if (count > 0)
		{
			FLog.Log(indention + indention + "Tags=" + tags);
		}
		if (mAI == null)
		{
			FLog.Log(indention + "No AI.");
			return;
		}
		// TODO(port): separator literals of the AI dump line are not recoverable (0x1171bf8c); debug output only. [left: debug dump text]
		FLog.Log(indention + "AI: " + mAI.Actions.Count.ToString() + ", SpawnPoint=" + mAI.SpawnPoint.ToString() + ", PatrolPoints=" + mAI.PatrolPoints.Count.ToString() + ")");
	}

	public void Log(string text)
	{
		FLog.Log(LogName + ": " + text);
	}

	public void SetAI(string aiType)
	{
		if (mAI != null)
		{
			mAI.StopActions();
			mAI.ReleaseStuff();
			mAI = null;
		}
		System.Type type = System.Type.GetType("FUnitAI" + aiType);
		if (type != null)
		{
			mAI = System.Activator.CreateInstance(type, this) as FUnitAIBase;
		}
		else if (aiType == "None")
		{
			SetObstructs(false);
		}
		else
		{
			mAI = new FUnitAIBase(this);
		}
	}

	public bool IsTag(int id)
	{
		return ProtoUnit.IsTag(id);
	}

	public bool IsTag(string tag)
	{
		return ProtoUnit.IsTag(tag);
	}

	public void FullReset()
	{
		if (ProtoUnit != null && StartData != null && !(Room == null))
		{
			if (Room.gameObject == null)
			{
				return;
			}
			SetProperties();
			Create(Room.gameObject);
		}
	}

	public void SetStartData(FProtoUnitStartData pusd)
	{
		StartData = pusd;
		SetProperties();
	}

	protected void SetProperties()
	{
		Name = StartData.Name;
		ProtoUnit = StartData.GetProtoUnit;
		SetObstructs(ProtoUnit.Obstructs);
		ProtoCollectibleName = StartData.ProtoCollectible;
		mPurchasePrice = StartData.PurchasePrice;
		mIdleAnim = "Idle";
		mHP = ProtoUnit.Health;
		SyncWeaponWithProto();
		if (StartData.Facing == FAnimEntry.AnimFacing.NONE)
		{
			if (ProtoUnit.IsTag("Attackable"))
			{
				StartData.Facing = (FAnimEntry.AnimFacing)FRandom.Generic.RangeInt(1, 4);
			}
			else
			{
				StartData.Facing = FAnimEntry.AnimFacing.DOWN;
			}
		}
		Facing = StartData.Facing;
		mLoopingSoundHelper = new FLoopingSoundHelper();
		if (StartData is FProtoDoorStartData doorData)
		{
			SetDoorLocked(doorData.Locked);
		}
		if (StartData is FProtoDroppedLootStartData lootData)
		{
			mIsDroppedLoot = lootData.IsDroppedLoot;
		}
		SetTilePosition(StartData.Position);
		SetAI(ProtoUnit.AIType);
		if (mAI != null && StartData.PatrolPoints.Count > 0 && Room != null)
		{
			mAI.PatrolPoints.Add(StartData.Position);
			for (int i = 0; i < StartData.PatrolPoints.Count; i++)
			{
				FPointData pointData = Room.ProtoRoom.GetPointData(StartData.PatrolPoints[i]);
				if (pointData != null)
				{
					mAI.PatrolPoints.Add(pointData.Position);
				}
			}
		}
		StartData.World = World.ProtoWorld.Name;
		StartData.Room = ((Room != null) ? Room.Name : "RoomUnknown");
	}

	public void Create(GameObject roomGO, string Outfit = "")
	{
		if (ProtoUnit == null)
		{
			return;
		}
		if (GO == null)
		{
			GO = FApp.PrefabPool.GetObject(ProtoUnit.PrefabName);
			if (ProtoUnit.IsTag("NoShadow"))
			{
				Transform shadow = GO.transform.Find("Shadow");
				if (shadow != null)
				{
					Object.Destroy(shadow.gameObject);
				}
			}
		}
		GO.name = ID.ToString();
		GO.transform.position = new Vector3(mTilePosition.x, mTilePosition.y, 0f);
		GO.transform.localScale = Vector3.one;
		World.ResetUnitShader(this);
		Color baseTint = World.BaseTileTint;
		SetColor(baseTint, false, false, 0.2f);
		RemoveAnimationLayers();
		mMyLastAttack = null;
		mLastTimeAttacked = 0f;
		CircleCollider2D[] colliders = GO.GetComponents<CircleCollider2D>();
		for (int i = 0; i < colliders.Length; i++)
		{
			if (!colliders[i].isTrigger)
			{
				mCollider = colliders[i];
			}
		}
		FAnimDef animDef = FAnimDefManager.GetAnimDef(ProtoUnit.VisualName);
		if (animDef == null)
		{
			SetVisible(false);
			FLog.Warning("No anim def for ProtoUnit '" + ProtoUnit.Name + "' found, VisualName='" + ProtoUnit.VisualName + "'");
			return;
		}
		Transform vis = GO.transform.Find("Vis");
		if (vis == null)
		{
			return;
		}
		string unitName = ProtoUnit.VisualName + Outfit;
		if (mAnimController == null)
		{
			mAnimController = new FAnimationController(unitName, animDef, vis.gameObject);
			mAnimController.TagResponder = this;
		}
		else
		{
			mAnimController.GO = vis.gameObject;
			mAnimController.UnitName = unitName;
			mAnimController.AnimDef = animDef;
		}
		mAnimController.Facing = Facing;
		Transform shadowT = GO.transform.Find("Shadow");
		if (shadowT != null)
		{
			shadowT.gameObject.SetActive(true);
		}
		if (!mIsDroppedLoot)
		{
			mCallSpawnCompleted = true;
		}
		else
		{
			if (ProtoUnit.IsTag("SpawnPersists"))
			{
				if (!StartData.Name.Valid())
				{
					StartData.Name = System.Guid.NewGuid().ToString();
				}
				FApp.PlayerStats.AddSpawnedUnit(StartData);
			}
			PlaySoundSet(ProtoUnit.Name + "_Spawn");
			FPoint2 from = mTilePosition;
			FProtoDroppedLootStartData lootStart = StartData as FProtoDroppedLootStartData;
			bool jump = true;
			if (lootStart != null)
			{
				if ((!ProtoCollectibleName.Valid() || ProtoUnit.IsTag("DontSpawnOnPlayer")) && !lootStart.AutoAwardToPlayer)
				{
					if (lootStart.PositionSpawnedFrom != FPoint2.invalid)
					{
						from = lootStart.PositionSpawnedFrom;
					}
				}
				else
				{
					jump = false;
				}
			}
			if (jump)
			{
				GO.transform.localScale = Vector3.zero;
				Vector3 start = new Vector3(from.x, from.y, 0f);
				Vector3 end = Position;
				List<Vector3> nodes = new List<Vector3>();
				nodes.Add(start);
				nodes.Add(Vector3.Lerp(start, end, 0.5f) + Vector3.up);
				nodes.Add(end);
				GoSpline spline = new GoSpline(nodes);
				Go.to(GO.transform, 0.4f, new GoTweenConfig().positionPath(spline).onComplete(delegate
				{
					mCallSpawnCompleted = true;
				}).setUpdateType(GoUpdateType.FWorldUpdate));
				Go.to(GO.transform, 0.25f, new GoTweenConfig().scale(1f).setEaseType(GoEaseType.BackOut).setUpdateType(GoUpdateType.FWorldUpdate));
			}
			else
			{
				GO.transform.localScale = Vector3.zero;
				mCallSpawnCompleted = true;
				FWorld.Player.SendMessage(this, "UnitAddedToTile");
			}
			if (mAI is FUnitAIEnemy enemyAI)
			{
				enemyAI.SetState(FUnitAIEnemy.EnemyState.AGGROD);
			}
		}
		mTriggeringUnits.Clear();
		mInteractable = true;
		SetObstructs(mObstructs);
		SetVisible(true);
		SetIdle();
		if ((mPurchasePrice >= 1 || ProtoUnit.PurchasePrice >= 1) && !StartData.IsFanfareReveal)
		{
			CreateUINode();
		}
	}

	public void SyncWeaponWithProto()
	{
		Weapon = ProtoUnit.GetProtoWeapon();
	}

	public void SetTilePosition(FPoint2 newPosition, bool force = false)
	{
		if (!(newPosition == mTilePosition) || force)
		{
			RemoveFromTile();
			mTilePosition = newPosition;
			AddToTile();
			if (force)
			{
				GO.transform.position = new Vector3(mTilePosition.x, mTilePosition.y, GO.transform.position.z);
			}
			if (mShowTileVisibility)
			{
				Room.SyncTileVisibility();
			}
		}
	}

	public void Rename(string name)
	{
		Name = name;
	}

	public void ReleaseUI()
	{
		if (mUINode == null)
		{
			return;
		}
		if (!mUINode.OnStage)
		{
			FLog.Log("Unit.ReleaseUI: " + LogName + " has a node, but it's not on stage.");
		}
		else
		{
			FLog.Log("Unit.ReleaseUI: " + LogName + " has a node, it's on stage, Unparenting it.");
			mUINode.Unparent();
		}
		mUINode.Dispose();
		mUINode = null;
		mPurchasePriceLabel = null;
	}

	public void ShowUINode(bool show)
	{
		if (mUINode != null)
		{
			if (!show)
			{
				if (mUINode.OnStage)
				{
					mUINode.Unparent();
				}
			}
			else if (!mUINode.OnStage)
			{
				mUINode.SetParent(World.RenderStage);
			}
		}
	}

	public void Release()
	{
		ReleaseUI();
		if (mAI != null)
		{
			mAI.ReleaseStuff();
			mAI.StopActions();
		}
		if (mLoopingSoundHelper != null)
		{
			mLoopingSoundHelper.StopAllLoopingSounds();
		}
		FApp.PlayerStats.StatsUpdatedAction -= UpdatePlayerState;
		if (GO != null)
		{
			FApp.PrefabPool.ReleaseObject(GO);
		}
	}

	public void RemoveAnimationLayers()
	{
		if (mAnimLayers != null)
		{
			foreach (KeyValuePair<string, FAnimationController> layer in mAnimLayers)
			{
				Object.Destroy(layer.Value.GO);
			}
			mAnimLayers.Clear();
		}
	}

	public void SetVisible(bool visible)
	{
		if (GO != null)
		{
			GO.SetActive(visible);
		}
		if (visible)
		{
			AddToTile();
			return;
		}
		RemoveFromTile();
		mLoopingSoundHelper.StopAllLoopingSounds();
	}

	public bool PlaySoundSet(string soundSetName)
	{
		if (!soundSetName.Valid())
		{
			return false;
		}
		if (FApp.SoundManager == null)
		{
			return false;
		}
		if (FApp.World == null)
		{
			return false;
		}
		if (FWorld.Player == null)
		{
			return false;
		}
		if (FWorld.Player.GO == null)
		{
			return false;
		}
		// Sounds are always played at the player's position (units are never positional here).
		FApp.SoundManager.PlaySoundSetAtPoint(soundSetName, FWorld.Player.GO.transform.position);
		return true;
	}

	public bool SendMessage(FUnit recipient, string msg, string data = "")
	{
		FUnitMessage message = new FUnitMessage();
		message.SenderID = ID;
		message.RecipientID = recipient.ID;
		message.Message = msg;
		message.Data = data;
		return recipient.ProcessMessage(message);
	}

	public void SetIdle()
	{
		if (mAnimController != null && !mAnimController.HasAnimsInQueue() && (mInteractable || !PlayAnim("Idle_NoInteract")))
		{
			PlayAnim(mIdleAnim);
		}
	}

	protected void CreateUINode()
	{
		if (ProtoUnit == null)
		{
			return;
		}
		float screenW = Futile.Screen.Width;
		float screenH = Futile.Screen.Height;
		if (mUINode == null)
		{
			mUINode = new FNode("UINode" + ToString());
		}
		mUINode.SetParent(FApp.World.RenderStage);
		mUINode.DockInside(FApp.World.RenderStage, InnerDock.Center, mUINodeSize.x, mUINodeSize.y, 0f, 0f);
		if (mPurchasePrice >= 1 || ProtoUnit.PurchasePrice >= 1)
		{
			if (mPurchasePriceLabel == null)
			{
				mPurchasePriceLabel = new FLabelDropshadow(Name + ".PurchasePrice", "DefaultStyle", string.Empty, 48f, false, true, false);
			}
			mPurchasePriceLabel.Scale = 1.3333334f / (screenW / screenH);
			mPurchasePriceLabel.SetParent(mUINode);
			mPurchasePriceLabel.DockInside(mUINode, InnerDock.Center, mUINodeSize.x, mUINodeSize.y, 0f, 0f);
			int price = ((mPurchasePrice >= 1) ? mPurchasePrice : ProtoUnit.PurchasePrice);
			mPurchasePriceLabel.Text = price.ToString();
			FApp.PlayerStats.StatsUpdatedAction += UpdatePlayerState;
			UpdatePlayerState(FApp.PlayerStats.CurrentPlayerState);
		}
		Vector3 position = Position;
		if (mUINode != null && ProtoUnit != null)
		{
			UpdateUINodePosition(position, true);
		}
	}

	protected void UpdatePlayerState(FPlayerState state)
	{
		if (mPurchasePriceLabel != null)
		{
			int price = mPurchasePrice;
			if (price < 1)
			{
				price = ProtoUnit.PurchasePrice;
				if (price < 1)
				{
					return;
				}
			}
			// 0x171e788..0x171e7d4: text colour (coins < price ? 1 : 0, 0, 0, 1) = red when unaffordable, black otherwise; drop shadow = Color.gray
			Color textColor = new Color((state.Coins < price) ? 1f : 0f, 0f, 0f, 1f);
			mPurchasePriceLabel.SetTextColor(textColor, Color.gray);
		}
	}

	public void SyncUINode(bool force)
	{
		Vector3 position = Position;
		if (mUINode != null && ProtoUnit != null)
		{
			UpdateUINodePosition(position, force);
		}
	}

	public void SyncUINode(Vector3 position, bool force)
	{
		if (mUINode != null && ProtoUnit != null)
		{
			UpdateUINodePosition(position, force);
		}
	}

	protected void UpdateUINodePosition(Vector3 position, bool force)
	{
		if (mUINode == null)
		{
			return;
		}
		if (force)
		{
			SetUINodePosition(position);
			mUINode.MarkDirtyRecursive();
		}
		else if (!((position - mLastUINodePosition).sqrMagnitude < 1E-10f) && !FUtil.EpsilonEqual(position, mLastUINodePosition))
		{
			SetUINodePosition(position);
		}
	}

	protected void SetUINodePosition(Vector3 position)
	{
		if (ProtoUnit == null || mUINode == null || FApp.World == null)
		{
			return;
		}
		if (FApp.World.RenderStage == null)
		{
			return;
		}
		position += Vector3.up * 0.85f;
		mLastUINodePosition = position;
		Vector3 screen = FApp.World.GetScreenPosition(position);
		mUINode.DockInside(FApp.World.RenderStage, InnerDock.Center, mUINodeSize.x, mUINodeSize.y, screen.x, screen.y);
		if (World != null)
		{
			mUINode.Scale = World.OrthographicBasedFNodeScale;
		}
	}

	public bool PlayAnim(string animType, bool loop = true, int? startFrame = null, bool fireTags = true)
	{
		if (mAI != null)
		{
			animType = mAI.GetAnimName(animType);
		}
		int typeID = FAnimDefManager.GetAnimTypeID(animType);
		if (typeID >= 0 && mAnimController != null)
		{
			return mAnimController.PlayAnim(typeID, loop, startFrame, false, fireTags);
		}
		return false;
	}

	public void SetIdle(string idle, bool force = true, bool setAnimImmediate = true)
	{
		if (!(mIdleAnim == idle) || force)
		{
			mIdleAnim = idle;
			if (setAnimImmediate)
			{
				SetIdle();
			}
		}
	}

	protected virtual void SpawnCompleted()
	{
		mSpawnComplete = true;
	}

	public void SetRoom(FRoom room)
	{
		if (room == null)
		{
			return;
		}
		Room = room;
		World = room.World;
		if (GO != null)
		{
			GO.transform.parent = room.transform;
		}
	}

	protected bool CanChangeToProto(FProtoUnit protoUnit)
	{
		return protoUnit.CheckSpawnRequirements();
	}

	public void ChangeProto(FProtoUnit newProto, bool force = false)
	{
		if ((newProto == null || !newProto.CheckSpawnRequirements()) && !force)
		{
			FWorld.Player.World.SpawnFlyOffText(FLoc.GetString("NotAvailable"), FWorld.Player.Position, new Vector2(32f, 32f));
			if (newProto.DisplayName != newProto.Name)
			{
				newProto = FApp.ProtoUnitManager.GetProtoUnit(newProto.DisplayName, true);
				if (newProto == null)
				{
					return;
				}
			}
			if (!newProto.CheckSpawnRequirements())
			{
				return;
			}
		}
		FAnimator animator = mAnimController.Animator;
		string animType = FAnimDefManager.GetAnimTypeByID(animator.CurrentAnimTypeID);
		bool loop = animator.Loop;
		int frame = animator.CurrentFrame;
		bool fireTags = animator.FireTags;
		ProtoUnit = newProto;
		SyncWeaponWithProto();
		Create(Room.gameObject, OutfitHandler.GetCurrentOutfitForCharacter(ProtoUnit.Name));
		if (!PlayAnim(animType, loop, frame, fireTags))
		{
			SetIdle("Idle", true, true);
		}
	}

	public virtual void Update(float deltaTime)
	{
		if (World.State == FWorld.GameState.PAUSED)
		{
			return;
		}
		if (World.State != FWorld.GameState.LOGIC_PAUSED)
		{
			if (mCallSpawnCompleted)
			{
				SpawnCompleted();
				mCallSpawnCompleted = false;
			}
			if (mAI != null)
			{
				mAI.Update(deltaTime);
			}
			if (mFloatyLoots.Count > 0)
			{
				UpdateFloatyLoots(deltaTime);
			}
		}
		if (World.State == FWorld.GameState.ANIM_PAUSED)
		{
			return;
		}
		if (mAnimController != null)
		{
			mAnimController.Update(deltaTime);
		}
		if (mAnimLayers != null)
		{
			for (int i = 0; i < mAnimLayersToRemove.Count; i++)
			{
				RemoveAnimationLayer(mAnimLayersToRemove[i]);
			}
			mAnimLayersToRemove.Clear();
			foreach (FAnimationController layer in mAnimLayers.Values)
			{
				layer.Update(deltaTime);
				if (!layer.IsAnimating() && !layer.HasAnimsInQueue())
				{
					mAnimLayersToRemove.Add(layer);
				}
			}
		}
		SyncEmote();
	}

	// Floaty loot part of Update (inlined in the shipped method).
	private void UpdateFloatyLoots(float deltaTime)
	{
		Vector3 target = Position + Vector3.up * 0.75f;
		float t = Mathf.Clamp01(deltaTime * 10f);
		int index = 0;
		while (index < mFloatyLoots.Count)
		{
			FFloatyLoots current = mFloatyLoots[index];
			if (current.Unit == null || current.Unit.GO == null)
			{
				mFloatyLoots.RemoveAt(index);
				index--;
			}
			else
			{
				float gametime = World.Gametime;
				if (current.ShowUntil <= gametime)
				{
					if (!current.Despawning)
					{
						current.Despawning = true;
						Go.to(current.Unit.GO.transform, 0.25f, new GoTweenConfig().scale(0f).setEaseType(GoEaseType.BackIn).onComplete(delegate
						{
							World.RemoveUnit(current.Unit);
							mFloatyLoots.Remove(current);
						}));
					}
					else
					{
						Transform lootT = current.Unit.GO.transform;
						lootT.position = Vector3.Lerp(lootT.position, Position, t);
					}
				}
				else
				{
					FRandom.Generic.SetSeed(index);
					Transform lootT2 = current.Unit.GO.transform;
					lootT2.position = Vector3.Lerp(lootT2.position, new Vector3(target.x, target.y, 0f), t);
					// FRandom.RandomDirectionFixVector3().ToVector3() inlined (FixVector3 is not ported).
					Fix64 angle = FRandom.Generic.RangeFix64(Fix64.Zero, Fix64.TwoPi);
					Fix64 dirZ = FRandom.Generic.RangeFix64(Fix64.MinusOne, Fix64.One);
					Fix64 radius = Fix64.Sqrt(Fix64.One - dirZ * dirZ);
					Vector3 offset = new Vector3((float)(radius * Fix64.Cos(angle)), (float)(radius * Fix64.Sin(angle)), (float)dirZ);
					target = current.Unit.GO.transform.position + offset * 0.25f;
				}
			}
			index++;
		}
	}

	public bool UnitInFront(FUnit unit)
	{
		return DotToUnit(unit) > 0f;
	}

	public bool UnitBehind(FUnit unit)
	{
		return DotToUnit(unit) <= 0f;
	}

	public float DotToUnit(FUnit unit)
	{
		Vector2 dir = GetDirFromFacing(Facing);
		Vector3 other = unit.Position;
		Vector3 mine = Position;
		return dir.x * (other.x - mine.x) + dir.y * (other.y - mine.y);
	}

	public bool CanPath(Vector3 position)
	{
		return CanPath(new FPoint2((int)position.x, (int)position.y));
	}

	public bool CanPath(FPoint2 position, bool partialOk = false)
	{
		FPath path = new FPath();
		FPather.PathResult result = World.FindPath(this, mTilePosition, null, position, 0, null, path, false);
		bool ok = result == FPather.PathResult.Partial && partialOk;
		if (result == FPather.PathResult.Complete || result == FPather.PathResult.InRangeAtStart)
		{
			ok = true;
		}
		return ok;
	}

	public FUnitMoveResult MoveTowards(FPath path, int currentWaypointIndex, float movementSpeed, float deltaTime, bool setAnimation, out int newWaypointIndex)
	{
		newWaypointIndex = currentWaypointIndex;
		float actualDistance = 0f;
		if (path == null || GO == null)
		{
			return FUnitMoveResult.Fail_Generic;
		}
		if (currentWaypointIndex >= path.NumberWaypoints)
		{
			return FUnitMoveResult.Fail_Generic;
		}
		if (newWaypointIndex >= path.NumberWaypoints)
		{
			return FUnitMoveResult.Success;
		}
		float remaining = movementSpeed * deltaTime;
		if (!(remaining > 0f))
		{
			return FUnitMoveResult.Success;
		}
		while (true)
		{
			FPoint2 waypoint = path.GetWaypoint(newWaypointIndex);
			Vector3 pos = GO.transform.position;
			float dx = waypoint.x - pos.x;
			float dy = waypoint.y - pos.y;
			float dist = Mathf.Sqrt(dx * dx + dy * dy);
			if (FUnitAIBase.cMoveTolerance <= dist)
			{
				float step = Mathf.Min(dist, remaining);
				Vector3 dir = ((dist > 1E-05f) ? new Vector3(dx / dist, dy / dist, 0f) : Vector3.zero);
				actualDistance = 0f;
				bool canMove = CheckMove(dir, step, out actualDistance);
				if (!canMove)
				{
					GO.transform.Translate(dir * actualDistance);
					return FUnitMoveResult.Fail_UnitCollision;
				}
				GO.transform.Translate(dir * step);
				if (setAnimation)
				{
					SetFacing(GetFacingFromDir(new Vector2(dir.x, dir.y)));
				}
				Vector3 newPos = GO.transform.position;
				float newDist = Mathf.Sqrt((waypoint.x - newPos.x) * (waypoint.x - newPos.x) + (waypoint.y - newPos.y) * (waypoint.y - newPos.y));
				if (!(dist < newDist) && !(newDist < FUnitAIBase.cMoveTolerance))
				{
					if (World.Gametime - mLastFootstepTime >= 0.2f)
					{
						PlaySoundSet(ProtoUnit.Name + "_" + mCurrentTileType);
						mLastFootstepTime = World.Gametime;
					}
					SyncTilePosition();
					return FUnitMoveResult.Success;
				}
				GO.transform.position = new Vector3(waypoint.x, waypoint.y, 0f);
				SyncTilePosition();
				newWaypointIndex++;
				if (newWaypointIndex >= path.NumberWaypoints)
				{
					return FUnitMoveResult.Success;
				}
				remaining -= step;
				if (remaining <= 0f)
				{
					return FUnitMoveResult.Success;
				}
				continue;
			}
			GO.transform.position = new Vector3(waypoint.x, waypoint.y, 0f);
			SyncTilePosition();
			newWaypointIndex++;
			if (newWaypointIndex >= path.NumberWaypoints)
			{
				break;
			}
		}
		return FUnitMoveResult.Success;
	}

	public FUnitMoveResult MoveTowards(Vector2 targetPoint, float movementSpeed, float deltaTime, bool setAnimation)
	{
		float actualDistance = 0f;
		if (GO == null)
		{
			return FUnitMoveResult.Fail_Generic;
		}
		Vector3 pos = GO.transform.position;
		float dx = targetPoint.x - pos.x;
		float dy = targetPoint.y - pos.y;
		float dist = Mathf.Sqrt(dx * dx + dy * dy);
		float step = Mathf.Min(dist, movementSpeed * deltaTime);
		Vector2 dir = ((dist > 1E-05f) ? new Vector2(dx / dist, dy / dist) : Vector2.zero);
		actualDistance = 0f;
		if (!CheckMove(new Vector3(dir.x, dir.y, 0f), step, out actualDistance, true))
		{
			GO.transform.Translate(new Vector3(dir.x * actualDistance, dir.y * actualDistance, 0f));
			return FUnitMoveResult.Fail_UnitCollision;
		}
		GO.transform.Translate(new Vector3(dir.x * step, dir.y * step, 0f));
		if (setAnimation)
		{
			SetFacing(GetFacingFromDir(dir));
		}
		Vector3 newPos = GO.transform.position;
		float newDist = Mathf.Sqrt((targetPoint.x - newPos.x) * (targetPoint.x - newPos.x) + (targetPoint.y - newPos.y) * (targetPoint.y - newPos.y));
		if (dist < newDist || newDist < FUnitAIBase.cMoveTolerance)
		{
			GO.transform.position = new Vector3(targetPoint.x, targetPoint.y, 0f);
		}
		if (World.Gametime - mLastFootstepTime >= 0.2f)
		{
			PlaySoundSet(ProtoUnit.Name + "_" + mCurrentTileType);
			mLastFootstepTime = World.Gametime;
		}
		SyncTilePosition();
		return FUnitMoveResult.Success;
	}

	public void SyncTilePosition()
	{
		if (!(GO == null))
		{
			Vector3 pos = GO.transform.position;
			int x = (int)System.Math.Round((double)pos.x);
			pos = GO.transform.position;
			int y = (int)System.Math.Round((double)pos.y);
			SetTilePosition(new FPoint2(x, y));
		}
	}

	protected bool CheckMove(Vector3 direction, float desiredDistance, out float actualDistance, bool checkTiles = false)
	{
		actualDistance = desiredDistance;
		if (GO == null)
		{
			return false;
		}
		if (mObstructs)
		{
			Vector3 start = GO.transform.position;
			Vector2 point = new Vector2(direction.x * desiredDistance + start.x, direction.y * desiredDistance + start.y);
			int unitCount = World.GetUnitsByCircleCast(point, 0.5f, mUnitsHit);
			for (int i = 0; i < unitCount; i++)
			{
				FUnit other = mUnitsHit[i];
				if (other == this)
				{
					continue;
				}
				string[] ignore = ProtoUnit.IgnoreCollisionWithProtoUnits;
				if (ignore != null && ignore.Length != 0 && System.Linq.Enumerable.Contains(ignore, other.ProtoUnit.Name))
				{
					continue;
				}
				if (other.IsPlayer && World.Gametime <= other.mLastTimeAttacked + 0.2f)
				{
					continue;
				}
				Vector3 center = other.CenterPoint;
				if (direction.x * (center.x - point.x) + direction.y * (center.y - point.y) >= 0.5f)
				{
					actualDistance = DistanceToUnitEdge(other, false);
					if (!other.mObstructs)
					{
						return true;
					}
					if (mAI == null)
					{
						return false;
					}
					return !mAI.CollidedWithUnit(other);
				}
			}
			if (checkTiles)
			{
				int tileCount = World.GetTilesByCircleCast(point, 0.4f, mTilesHit);
				for (int j = 0; j < tileCount; j++)
				{
					Vector2 tilePos = mTilesHit[j].Position.ToVector2();
					float tdx = tilePos.x - point.x;
					float tdy = tilePos.y - point.y;
					if (direction.x * tdx + direction.y * tdy >= 0.5f)
					{
						actualDistance = Mathf.Sqrt(tdx * tdx + tdy * tdy) - 1f;
						return false;
					}
				}
			}
		}
		return true;
	}

	public bool CollidedWithUnit(FUnit otherUnit)
	{
		if (otherUnit != null && otherUnit != this)
		{
			if (!otherUnit.mObstructs)
			{
				return false;
			}
			if (mAI != null)
			{
				return mAI.CollidedWithUnit(otherUnit);
			}
			return true;
		}
		return false;
	}

	public float DistanceFromTileCenter()
	{
		Vector3 pos = Position;
		float dx = pos.x - mTilePosition.x;
		float dy = pos.y - mTilePosition.y;
		return Mathf.Sqrt(dx * dx + dy * dy);
	}

	public void SnapToTileCenter()
	{
		GO.transform.position = new Vector3(mTilePosition.x, mTilePosition.y, GO.transform.position.z);
	}

	public void RemoveFromTile()
	{
		LastTilePosition = mTilePosition;
		for (int i = 0; i < mTilesOccupying.Count; i++)
		{
			FTile tile = mTilesOccupying[i];
			if (tile != null)
			{
				tile.RemoveUnit(this);
			}
		}
		mTilesOccupying.Clear();
	}

	public void AddToTile(FPoint2 position = default(FPoint2))
	{
		if (position == default(FPoint2))
		{
			position = mTilePosition;
		}
		if (Room != null)
		{
			FTile tile = Room.GetTile(position);
			if (tile != null)
			{
				tile.AddUnit(this);
				mCurrentTileType = tile.ProtoTile.TileType;
				if (!mTilesOccupying.Contains(tile))
				{
					mTilesOccupying.Add(tile);
				}
			}
		}
	}

	public Vector3 GetPosition()
	{
		return Position;
	}

	public bool IsAdjacent(FUnit other)
	{
		return DistanceToUnitEdge(other, false) < FUnitAIBase.cMoveTolerance;
	}

	public float DistanceToUnitEdge(FUnit other, bool singleAxis)
	{
		if (other == null)
		{
			return 0f;
		}
		Vector3 pos = other.Position;
		Vector2 offset = other.mCollider.offset;
		return DistanceToUnitEdge(new Vector3(pos.x + offset.x, pos.y + offset.y, pos.z), singleAxis, other.mCollider.radius + 0.05f);
	}

	public float DistanceToUnitEdge(Vector3 pos, bool singleAxis, float radius = 0.5f)
	{
		Vector3 mine = Position;
		float distance = Mathf.Sqrt((pos.x - mine.x) * (pos.x - mine.x) + (pos.y - mine.y) * (pos.y - mine.y)) - radius - radius;
		float result = distance;
		if (singleAxis && distance > 0f && FUnitAIBase.cMoveTolerance < Mathf.Abs(pos.x - Position.x))
		{
			result = 1000f;
			if (Mathf.Abs(pos.y - Position.y) <= FUnitAIBase.cMoveTolerance)
			{
				result = distance;
			}
		}
		return result;
	}

	public void SetFacing(FAnimEntry.AnimFacing facing, bool force = false)
	{
		if (Facing == facing && !force)
		{
			return;
		}
		Facing = facing;
		if (mAnimController != null)
		{
			mAnimController.SetFacing(facing, force);
		}
		if (mAnimLayers != null)
		{
			foreach (FAnimationController layer in mAnimLayers.Values)
			{
				layer.SetFacing(Facing);
			}
		}
	}

	public void RefreshAnimation()
	{
		if (mAnimController != null && mAnimController.Animator != null)
		{
			string animType = FAnimDefManager.GetAnimTypeByID(mAnimController.Animator.CurrentAnimTypeID);
			if (animType.Valid())
			{
				if (animType.Contains("Idle"))
				{
					SetIdle();
					return;
				}
				FAnimator animator = mAnimController.Animator;
				PlayAnim(animType, animator.Loop, animator.CurrentFrame, animator.FireTags);
			}
		}
	}

	public void FacePosition(Vector2 position)
	{
		Vector3 pos = Position;
		SetFacing(GetFacingFromDir(new Vector2(position.x - pos.x, position.y - pos.y)));
	}

	public void FacePosition(FPoint2 tile)
	{
		FacePosition(new Vector2(tile.x, tile.y));
	}

	public bool PlayAnim(int type, bool loop = true, int? startFrame = null, bool fireTags = true)
	{
		if (mAnimController != null)
		{
			return mAnimController.PlayAnim(type, loop, startFrame, false, fireTags);
		}
		return false;
	}

	public bool IsPlayingAnim(string animType)
	{
		if (mAnimController.IsAnimating())
		{
			int typeID = FAnimDefManager.GetAnimTypeID(animType);
			if (typeID > -1)
			{
				return mAnimController.Animator.CurrentAnimTypeID == typeID;
			}
		}
		return false;
	}

	public bool QueueAnim(string animType, bool loop = true)
	{
		if (mAI != null)
		{
			animType = mAI.GetAnimName(animType);
		}
		int typeID = FAnimDefManager.GetAnimTypeID(animType);
		if (typeID > -1 && mAnimController != null)
		{
			return mAnimController.QueueAnim(typeID, loop);
		}
		return false;
	}

	public bool QueueAnim(int type, bool loop = true)
	{
		if (mAnimController != null)
		{
			return mAnimController.QueueAnim(type, loop);
		}
		return false;
	}

	public float GetCurrentAnimLength()
	{
		if (mAnimController != null && !(mAnimController.Animator == null) && mAnimController.IsAnimating())
		{
			return mAnimController.Animator.GetCurrentAnimLength();
		}
		return 0f;
	}

	public static FAnimEntry.AnimFacing GetFacingFromDir(Vector2 dir)
	{
		Vector2 n = dir.normalized;
		float dotRight = Vector3.Dot(n, Vector3.right);
		if (dotRight > 0.5f)
		{
			return FAnimEntry.AnimFacing.RIGHT;
		}
		if (dotRight < -0.5f)
		{
			return FAnimEntry.AnimFacing.LEFT;
		}
		if (Vector3.Dot(n, Vector3.up) > 0.5f)
		{
			return FAnimEntry.AnimFacing.UP;
		}
		return FAnimEntry.AnimFacing.DOWN;
	}

	public Vector2 GetDirFromFacing()
	{
		return GetDirFromFacing(Facing);
	}

	public static Vector2 GetDirFromFacing(FAnimEntry.AnimFacing facing)
	{
		switch (facing)
		{
		case FAnimEntry.AnimFacing.LEFT:
			return Vector2.left;
		case FAnimEntry.AnimFacing.UP:
			return Vector2.up;
		case FAnimEntry.AnimFacing.RIGHT:
			return Vector2.right;
		case FAnimEntry.AnimFacing.DOWN:
			return Vector2.down;
		default:
			return Vector2.zero;
		}
	}

	public Vector2 Left()
	{
		Vector2 dir = GetDirFromFacing(Facing);
		return new Vector2(0f - dir.y, dir.x);
	}

	public Vector2 Right()
	{
		Vector2 dir = GetDirFromFacing(Facing);
		return new Vector2(dir.y, 0f - dir.x);
	}

	public Vector2 Back()
	{
		return -GetDirFromFacing(Facing);
	}

	public Vector2 Forward()
	{
		return GetDirFromFacing(Facing);
	}

	public FPoint2 LeftFP2()
	{
		return Left().ToFPoint2();
	}

	public FPoint2 RightFP2()
	{
		return Right().ToFPoint2();
	}

	public FPoint2 BackFP2()
	{
		return Back().ToFPoint2();
	}

	public FPoint2 ForwardFP2()
	{
		return GetDirFromFacing(Facing).ToFPoint2();
	}

	public FPoint2 GetFPDirFromFacing()
	{
		return GetFPDirFromFacing(Facing);
	}

	public static FPoint2 GetFPDirFromFacing(FAnimEntry.AnimFacing facing)
	{
		Vector2 dir = GetDirFromFacing(facing);
		return new FPoint2((int)dir.x, (int)dir.y);
	}

	public Vector2 GetTileDirectionToUnit(FUnit other)
	{
		return new Vector2((float)other.mTilePosition.x - (float)mTilePosition.x, (float)other.mTilePosition.y - (float)mTilePosition.y).normalized;
	}

	public void SetColor(Color color, bool animate = false, bool force = false, float animTime = 0.2f)
	{
		if (GO == null)
		{
			return;
		}
		bool wasActive = GO.activeSelf;
		GO.SetActive(true);
		if (!force && ProtoUnit.NoWorldTint)
		{
			color = Color.white;
		}
		SpriteRenderer[] renderers = GO.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < renderers.Length; i++)
		{
			if (!animate)
			{
				renderers[i].material.color = color;
				continue;
			}
			Go.killAllAbstractTweensWithTarget(renderers[i]);
			Go.to(renderers[i], animTime, new GoTweenConfig().materialColor(color).setUpdateType(GoUpdateType.FWorldUpdate));
		}
		GO.SetActive(wasActive);
	}

	public void SetSortingOrder(int order)
	{
		if (GO == null)
		{
			return;
		}
		mBaseSortingOrder = order;
		SpriteRenderer[] renderers = GO.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < renderers.Length; i++)
		{
			if (renderers[i].gameObject.tag != "DontUpdate")
			{
				renderers[i].sortingOrder = order;
				order--;
			}
		}
	}

	public void ResetObstructs()
	{
		SetObstructs(ProtoUnit.Obstructs);
	}

	public void SetObstructs(bool v)
	{
		mObstructs = v;
		if (GO == null)
		{
			return;
		}
		Collider2D[] colliders = GO.GetComponentsInChildren<Collider2D>();
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = v;
		}
	}

	public void SetAlive(bool v)
	{
		mAlive = v;
		if (v)
		{
			mDying = false;
		}
		else
		{
			DoLoot();
		}
	}

	public bool CanAttackWithWeapon()
	{
		FProtoWeapon weapon = ProtoUnit.GetProtoWeapon();
		if (weapon == null)
		{
			return false;
		}
		if (weapon.HasAmmoResource && FApp.PlayerStats.GetAmmoAmount(weapon.AmmoResource) < 1)
		{
			return false;
		}
		return true;
	}

	public bool UseWeaponAmmo()
	{
		FProtoWeapon weapon = ProtoUnit.GetProtoWeapon();
		if (weapon == null || !weapon.HasAmmoResource)
		{
			return true;
		}
		return FApp.PlayerStats.UseAmmoAmount(weapon.AmmoResource, 1);
	}

	public FWeaponObj Attack(FUnit target = null, FPoint2 position = default(FPoint2))
	{
		if (Weapon == null)
		{
			return null;
		}
		if (target == null)
		{
			target = AttackTarget;
		}
		if (position == default(FPoint2))
		{
			position = AttackPosition;
		}
		if (target == null && position == FPoint2.invalid)
		{
			return null;
		}
		if (mMyLastAttack != null && World.Gametime - mMyLastAttack.Time < Weapon.Cooldown)
		{
			return null;
		}
		if (!UseWeaponAmmo())
		{
			return null;
		}
		FUnitAttackDesc attackDesc = new FUnitAttackDesc();
		attackDesc.ID = ((target != null) ? target.ID : 0);
		mMyLastAttack = attackDesc;
		attackDesc.Time = World.Gametime;
		GameObject prefab = FUtil.LoadPrefab("Prefabs/Weapons/" + Weapon.PrefabName);
		if (prefab == null)
		{
			return null;
		}
		float targetX;
		float targetY;
		if (target == null)
		{
			targetX = position.x;
			targetY = position.y;
		}
		else
		{
			Vector3 targetPos = target.Position;
			targetX = targetPos.x;
			targetY = targetPos.y;
		}
		Vector3 myPos = Position;
		Vector2 toTarget = new Vector2(targetX - myPos.x, targetY - myPos.y);
		Vector2 dir = toTarget.normalized;
		if (Weapon.SingleAxis && dir != Vector2.zero)
		{
			dir = GetDirFromFacing(Facing);
		}
		GameObject weaponGO = Object.Instantiate(prefab);
		FWeaponObj weaponObj = ((weaponGO != null) ? weaponGO.GetComponent<FWeaponObj>() : null);
		weaponObj.Owner = this;
		weaponObj.ProtoWeapon = Weapon;
		weaponObj.Target = target;
		weaponObj.StunTime = Weapon.StunTime;
		string weaponType = Weapon.Type;
		if (weaponType == "Melee")
		{
			Vector3 ownerPos = GO.transform.position;
			weaponGO.transform.position = new Vector3(dir.x + ownerPos.x, dir.y + ownerPos.y, 0f);
		}
		else if (weaponType == "Ranged")
		{
			weaponGO.transform.position = GO.transform.position;
			if (!Weapon.SingleAxis)
			{
				Vector3 ownerPos2 = GO.transform.position;
				dir = new Vector2(targetX - ownerPos2.x, targetY - ownerPos2.y).normalized;
			}
			int speed = Weapon.Speed;
			weaponObj.MoveVector = new Vector2(dir.x * (float)speed, dir.y * (float)speed);
			Vector2 land = new Vector2(targetX, targetY);
			if (!Weapon.CanTargetGround && position == FPoint2.invalid)
			{
				land = Vector2.zero;
			}
			weaponObj.LandPosition = land;
		}
		FGameUtil.ExecuteNextFrame(CleanupAfterAttack);
		return weaponObj;
	}

	protected void CleanupAfterAttack()
	{
		if (Weapon.SwitchToWeaponAfterUse.Valid())
		{
			FProtoWeapon next = FApp.ProtoWeaponManager.GetProtoWeapon(Weapon.SwitchToWeaponAfterUse);
			if (next != null)
			{
				ProtoUnit.SetWeapon(next.Name);
				SyncWeaponWithProto();
				if (FApp.HUD.PageLegendWorld != null)
				{
					FApp.HUD.PageLegendWorld.SyncSpecialWeaponButton();
				}
			}
		}
		if (mAI.GetCurrentAction() is FUnitActionAttack attack)
		{
			attack.SetState(FUnitActionWork.States.Success);
		}
	}

	public void StunAttackTarget()
	{
		FWeaponObj weaponObj = Attack();
		if (!(weaponObj == null))
		{
			weaponObj.StunTime = Weapon.StunTime;
		}
	}

	public bool OnStunned(FUnit attacker, float stunTime)
	{
		if (ProtoUnit.ImmuneToStun)
		{
			return false;
		}
		FUnitMessage message = new FUnitMessage();
		message.SenderID = attacker.ID;
		message.RecipientID = ID;
		message.Message = "Stunned";
		if (ProcessMessage(message))
		{
			return false;
		}
		mAI.StopActions();
		mAI.AddAction(new FUnitActionStunned(World.GetNextActionID(), this, stunTime));
		World.FlashUnit(this);
		return true;
	}

	public void SetLastAttackTarget(int targetID)
	{
		if (mAI != null)
		{
			mAI.SetLastAttackTarget(targetID);
		}
	}

	public void SetWeapon(FProtoWeapon weapon)
	{
		Weapon = weapon;
	}

	public bool OnAttacked(FUnit attacker, Vector2? pushDir = null, bool force = false, FProtoWeapon weaponToUse = null)
	{
		if (attacker == null)
		{
			return false;
		}
		if (!ProtoUnit.CheckInteractRequirements() && !force)
		{
			return false;
		}
		if (weaponToUse == null)
		{
			weaponToUse = attacker.Weapon;
		}
		int damage = weaponToUse.Damage;
		if (damage < 1)
		{
			return false;
		}
		FUnitMessage message = new FUnitMessage();
		message.SenderID = attacker.ID;
		message.RecipientID = ID;
		message.Message = "Attacked";
		if (ProcessMessage(message) && !force)
		{
			return false;
		}
		if (!pushDir.HasValue || (pushDir.Value - Vector2.zero).sqrMagnitude < 1E-10f)
		{
			Vector2 push;
			if (weaponToUse.Range < 2)
			{
				push = attacker.GetDirFromFacing();
			}
			else
			{
				Vector3 away = (Position - attacker.Position).normalized;
				push = new Vector2(away.x, away.y);
			}
			pushDir = push;
		}
		if (weaponToUse.CritChance > 0f)
		{
			int roll = FRandom.Generic.RangeInt(0, 100);
			if ((float)roll <= weaponToUse.CritChance)
			{
				int multiplier = weaponToUse.CritMultiplier;
				World.SpawnFlyOffText(FLoc.GetString("Crit"), Position, new Vector2(32f, 32f));
				damage *= multiplier;
			}
		}
		return OnAttacked(attacker, damage, pushDir.Value, weaponToUse.Knockback);
	}

	public bool OnAttacked(FUnit unit, int damage, Vector2 pushDir, int knockbackAmount = 0, bool forceKnockback = false)
	{
		if ((ProtoUnit.Health != 0 && !mAlive) || mDying)
		{
			return true;
		}
		if (!mInteractable || !ProtoUnit.IsTag("Attackable"))
		{
			return false;
		}
		FUnitAttackDesc attackDesc = new FUnitAttackDesc();
		attackDesc.ID = unit.ID;
		mLastAttack = attackDesc;
		attackDesc.Time = World.Gametime;
		bool applyDamage;
		if (!IsPlayer)
		{
			applyDamage = true;
		}
		else
		{
			float gametime = World.Gametime;
			if (mLastTimeAttacked + 0.2f < gametime)
			{
				mLastTimeAttacked = gametime;
				applyDamage = true;
			}
			else
			{
				applyDamage = false;
			}
		}
		if (IsPlayer && !applyDamage)
		{
			// PC port: a hit inside the 0.2 s invulnerability window does nothing at all (no knockback restart / hit anim / sound),
			// otherwise lasers and vial knockbacks re-trigger the hit every frame (sprite vibration, distorted audio).
			return mHP < 1;
		}
		float stunRemaining = 0f;
		if (mAI.GetCurrentAction() is FUnitActionStunned stunned)
		{
			stunRemaining = stunned.TimeRemaining;
		}
		if (knockbackAmount == 0)
		{
			knockbackAmount = (IsTag("AlwaysMovesOnKnockback") ? 1 : 0);
		}
		Push(pushDir, knockbackAmount, false, forceKnockback);
		if (MaxDamageTakenPerHit >= 0)
		{
			damage = Mathf.Min(damage, MaxDamageTakenPerHit);
		}
		if (damage > 0)
		{
			float modifier = ((ProtoUnit != null) ? ProtoUnit.DamageTakenModifier : 1f);
			damage = (int)(modifier * (float)damage);
		}
		if (applyDamage)
		{
			int newHP = mHP - damage;
			SetHP(newHP);
			SetHP(newHP);
		}
		if (mHP > 0 && stunRemaining > 0f)
		{
			mAI.AddAction(new FUnitActionStunned(World.GetNextActionID(), this, stunRemaining));
		}
		World.FlashUnit(this);
		return mHP < 1;
	}

	public void SetHP(int newHP)
	{
		bool noKill = ProtoUnit.IsTag("NoKill");
		if (newHP < 1 && noKill)
		{
			newHP = 1;
		}
		if (!IsPlayer)
		{
			mHP = newHP;
		}
		else
		{
			if (FApp.PlayerStats != null)
			{
				FApp.PlayerStats.Health = newHP;
				mHP = FApp.PlayerStats.Health;
			}
			FApp.HUD.PageLegendWorld.SetLowHP(mHP < 4, mHP < 2);
			newHP = mHP;
		}
		if (newHP <= 0)
		{
			Die();
		}
	}

	public void Die()
	{
		if (mDying)
		{
			return;
		}
		if (ProtoUnit.ResetOnDeath && World.CurrentRoom != null && World.CurrentRoom.ResetUnit(this))
		{
			World.SpawnEffect("PoofEffect", Position, 200);
			return;
		}
		ClearEmote();
		mHP = 0;
		mDying = true;
		if (mAI != null)
		{
			mAI.AddAction(new FUnitActionDeath(Room.World.GetNextActionID(), this));
		}
	}

	public void ClearEmote()
	{
		if (mCurrentEmote != null)
		{
			mCurrentEmote.ClearQueue();
			mCurrentEmote.EndCurrentLoop();
			mCurrentEmote = null;
		}
	}

	public void DoLoot()
	{
		if (IsPlayer)
		{
			return;
		}
		if (!string.IsNullOrEmpty(ProtoCollectibleName))
		{
			FApp.PlayerStats.AddCollectible(ProtoCollectibleName);
			TryAwardDungeonItem();
		}
		else if (ProtoUnit.IsTag("Collectible"))
		{
			TryEvaluateLootTable();
		}
		else
		{
			DropLoot();
		}
	}

	public void TryAwardDungeonItem()
	{
		FProtoCollectible collectible = FApp.ProtoCollectibleManager.GetProtoCollectible(ProtoCollectibleName);
		if (collectible == null || collectible.Type != FProtoCollectible.Types.DungeonItem)
		{
			return;
		}
		List<string> seen = new List<string>();
		string prefix = collectible.Name.Split('_')[0];
		int owned = 0;
		int index = 0;
		do
		{
			if (!seen.Contains(collectible.Name))
			{
				seen.Add(collectible.Name);
				owned += (FApp.PlayerStats.HasCollectible(collectible.DBID) ? 1 : 0);
			}
			collectible = FApp.ProtoCollectibleManager.GetProtoCollectible(string.Format(prefix + "_{0:D3}", index));
			index++;
		}
		while (collectible != null);
		World.SpawnFlyOffText(FLoc.Format("DungeonItem.Format", owned, seen.Count), Position, new Vector2(-32f, -32f));
		if (owned == seen.Count)
		{
			DropLoot(false, default(FPoint2), true);
		}
	}

	public bool TryEvaluateLootTable()
	{
		if (string.IsNullOrEmpty(ProtoUnit.LootTable))
		{
			return false;
		}
		FApp.PlayerStats.RollAndApplyLootTable(FApp.LootTableManager.GetLootTable(ProtoUnit.LootTable));
		return true;
	}

	public bool DropLoot(bool fullPassableCheck = false, FPoint2 positionOverride = default(FPoint2), bool autoAwardToPlayer = false)
	{
		ProtoUnit.IsTag("Openable");
		List<FProtoDroppedLootStartData> datas = GetLootStartDatas(fullPassableCheck);
		for (int i = 0; i < datas.Count; i++)
		{
			FProtoDroppedLootStartData data = datas[i];
			if (positionOverride != default(FPoint2))
			{
				data.Position = positionOverride;
			}
			if (autoAwardToPlayer)
			{
				data.AutoAwardToPlayer = true;
			}
			Room.AddUnit(data);
		}
		return false;
	}

	public List<FProtoDroppedLootStartData> GetLootStartDatas(bool fullPassableCheck = false)
	{
		List<FProtoDroppedLootStartData> result = new List<FProtoDroppedLootStartData>();
		string table = ProtoUnit.LootTable;
		if (StartData.LootTableOverride.Valid())
		{
			table = StartData.LootTableOverride;
		}
		if (!table.Valid())
		{
			return result;
		}
		FLootDesc[] descs = FApp.PlayerStats.RollLootTable(FApp.LootTableManager.GetLootTable(table));
		List<FPoint2> usedPoints = new List<FPoint2>();
		for (int i = 0; i < descs.Length; i++)
		{
			FLootDesc desc = descs[i];
			string protoName = desc.Name;
			if (!protoName.Valid())
			{
				continue;
			}
			if (FApp.ProtoUnitManager.GetProtoUnit(protoName) == null)
			{
				FApp.PlayerStats.EvaluateLoot(desc);
				continue;
			}
			FProtoDroppedLootStartData lootData = new FProtoDroppedLootStartData();
			lootData.Name = "Loot_" + Name + "_" + protoName;
			lootData.ProtoUnit = protoName;
			lootData.Position = mTilePosition;
			if (desc.DBID > -1)
			{
				FProtoCollectible collectible = FApp.ProtoCollectibleManager.GetProtoCollectibleByDBID(desc.DBID);
				lootData.ProtoCollectible = collectible.Name;
			}
			FUnit ignoreUnit;
			if (ProtoUnit.IsTag("Openable"))
			{
				ignoreUnit = null;
			}
			else
			{
				ignoreUnit = (AliveAndWell ? null : this);
			}
			FTile tile = Room.GetClosestFullyOpenTile(mTilePosition, ignoreUnit, usedPoints);
			if (tile != null)
			{
				lootData.Position = tile.Position;
				lootData.PositionSpawnedFrom = mTilePosition;
				usedPoints.Add(tile.Position);
			}
			lootData.IsDroppedLoot = true;
			result.Add(lootData);
		}
		return result;
	}

	public bool ToggleDoor()
	{
		if (!ProtoUnit.IsTag("Door"))
		{
			return false;
		}
		if (mObstructs)
		{
			return OpenDoor();
		}
		CloseDoor();
		return true;
	}

	public bool CanBeOpened()
	{
		if (IsPlayingAnim("Open"))
		{
			return false;
		}
		if (mLocked)
		{
			if (!ProtoUnit.KeycardUnlockable)
			{
				return false;
			}
			if (FApp.PlayerStats.KeyCards == 0)
			{
				return false;
			}
		}
		return true;
	}

	public bool OpenDoor(bool force = false)
	{
		if (!force)
		{
			if (!ProtoUnit.CheckInteractRequirements())
			{
				string currentCharacter = FApp.PlayerStats.CurrentCharacter;
				string requiredCharacter = ProtoUnit.GetInteractRequirementsRequiredCharacter();
				if (!string.IsNullOrEmpty(requiredCharacter) && requiredCharacter != FApp.PlayerStats.CurrentCharacter && (FApp.PlayerStats.HasCollectible(requiredCharacter) || requiredCharacter == "Hopper"))
				{
					World.SetPlayerCharacter(requiredCharacter);
					if (ProtoUnit.CheckInteractRequirements())
					{
						goto IL_Unlock;
					}
					World.SetPlayerCharacter(currentCharacter);
				}
				World.TryShowDialogue(this);
				return false;
			}
			goto IL_Unlock;
		}
		goto IL_Open;
		IL_Unlock:
		if (mLocked && !Room.UnlockDoor(this))
		{
			return false;
		}
		IL_Open:
		return SendMessage(this, "OpenDoor");
	}

	public bool CloseDoor()
	{
		return SendMessage(this, "CloseDoor");
	}

	protected void OpenDoorImmediate()
	{
		if (PlayAnim("Open", false))
		{
			SetIdle("Idle_Open", false, false);
			SetObstructs(false);
			World.TryDoorTransition(this);
			if (!ProtoUnit.IsTag("KeepAliveAfterOpen"))
			{
				World.RemoveUnit(this, GetCurrentAnimLength());
			}
		}
	}

	protected void CloseDoorImmediate()
	{
		SetObstructs(true);
		PlayAnim("Close");
	}

	public void SetDoorLocked(bool locked)
	{
		mLocked = locked;
		mIdleAnim = (locked ? "Idle_Locked" : "Idle");
		SetIdle();
	}

	public void Push(Vector2 dir, int numTiles, bool interrupt = true, bool forceKnockback = false, bool playHitAnim = true)
	{
		if (ProtoUnit.IsTag("Stationary"))
		{
			return;
		}
		mAI.StopActions();
		if (ProtoUnit.ImmuneToKnockback && !forceKnockback)
		{
			numTiles = 0;
		}
		FPoint2 endPoint;
		CanPush(this, dir, numTiles, out endPoint);
		mAI.AddAction(new FUnitActionKnockback(World.GetNextActionID(), this, endPoint, playHitAnim));
		if (ProtoUnit.IsTag("EnemyCharacter") || IsPlayer)
		{
			mAI.AddAction(new FUnitActionWait(World.GetNextActionID(), this, 0.5f));
		}
	}

	public bool CanPush(FUnit targetUnit, Vector2 dir, int numTiles, out FPoint2 endPoint)
	{
		FPoint2 step = new FPoint2((Mathf.Abs(dir.x) > 0.5f) ? ((dir.x >= 0f) ? 1 : (-1)) : 0, (Mathf.Abs(dir.y) > 0.5f) ? ((dir.y >= 0f) ? 1 : (-1)) : 0);
		FPoint2 current = default(FPoint2);
		endPoint = targetUnit.mTilePosition;
		if (!mCheckingCanPush && !targetUnit.ProtoUnit.IsTag("Stationary") && (targetUnit.ProtoUnit.IsTag("Pushable") || targetUnit.ProtoUnit.IsTag("Attackable")))
		{
			mCheckingCanPush = true;
			if (numTiles < 1)
			{
				mCheckingCanPush = false;
				return false;
			}
			bool anyFree = false;
			int distance = 1;
			while (true)
			{
				current = targetUnit.mTilePosition + step * distance;
				if (!Room.ContainsFPoint(current))
				{
					break;
				}
				FTile tile = World.CurrentRoom.GetTile(current);
				if (tile == null || !tile.TerrainPassable)
				{
					break;
				}
				int count = FApp.World.GetUnitsByCircleCast(new Vector2(current.x, current.y), 0.2f, mUnitsHit);
				FUnit blocker = null;
				for (int i = 0; i < count; i++)
				{
					FUnit other = mUnitsHit[i];
					if (other != this && other != targetUnit && other.mObstructs && !(other.mTilePosition == mTilePosition) && !CanPush(other, dir, numTiles, out current))
					{
						blocker = other;
					}
				}
				distance++;
				anyFree = blocker == null || anyFree;
				endPoint = current;
				if (numTiles < distance)
				{
					break;
				}
			}
			mCheckingCanPush = false;
			if (anyFree)
			{
				return endPoint != targetUnit.mTilePosition;
			}
			return false;
		}
		return false;
	}

	public bool CanPush(FUnit targetUnit, Vector2 dir, int numTiles)
	{
		FPoint2 endPoint;
		return CanPush(targetUnit, dir, numTiles, out endPoint);
	}

	public bool CanOpen(FUnit targetUnit)
	{
		return targetUnit.ProtoUnit.CheckInteractRequirements();
	}

	public bool CanSee(FUnit other, bool checkUnits)
	{
		Vector3 otherPos = other.Position;
		Vector3 myPos = Position;
		float dx = otherPos.x - myPos.x;
		float dy = otherPos.y - myPos.y;
		int count = Physics2D.CircleCastNonAlloc(new Vector2(myPos.x, myPos.y), 0.5f, new Vector2(dx, dy), mRaycastHits, Mathf.Sqrt(dx * dx + dy * dy));
		bool ranged = Weapon != null && Weapon.Type == "Ranged";
		for (int i = 0; i < count; i++)
		{
			RaycastHit2D hit = mRaycastHits[i];
			if (hit.collider == null || hit.collider.gameObject == null || hit.collider.isTrigger)
			{
				continue;
			}
			FUnit unit = FApp.World.GetUnitFromGO(hit.collider.gameObject);
			if (unit != null)
			{
				if (unit == other)
				{
					return true;
				}
				if (checkUnits)
				{
					if (unit.mObstructs && unit != this && unit.ProtoUnit.BlocksProjectiles)
					{
						return false;
					}
					continue;
				}
			}
			FTile tile = FApp.World.GetTileFromGO(hit.collider.gameObject);
			if (tile != null && !tile.TerrainPassable && tile.Position != other.mTilePosition && (!ranged || !tile.Hole))
			{
				return false;
			}
		}
		return true;
	}

	protected void AddFloatyLoot(FUnit unit)
	{
		unit.mInteractable = false;
		unit.SetAI("None");
		unit.SetSortingOrder(300);
		unit.RemoveAnimationLayers();
		unit.mDying = true;
		FFloatyLoots floaty = new FFloatyLoots();
		floaty.Unit = unit;
		floaty.ShowUntil = World.Gametime + 1f;
		mFloatyLoots.Add(floaty);
	}

	public void SetInteractable()
	{
		mInteractable = true;
	}

	public void SetNonInteractable()
	{
		mInteractable = false;
	}

	public virtual void Trigger(int unitID = -1)
	{
		FUnit triggerUnit = World.GetUnitByID(unitID);
		if (triggerUnit != null)
		{
			bool contained = mTriggeringUnits.Contains(unitID);
			if (contained)
			{
				mTriggeringUnits.Remove(unitID);
			}
			FUnitAIBase triggerAI = triggerUnit.mAI;
			if (triggerAI != null)
			{
				if (triggerAI is FUnitAITrigger triggerAITrigger && !triggerAITrigger.On)
				{
					return;
				}
				if (triggerAI is FUnitAIPressurePlate plate && !plate.Pressed)
				{
					return;
				}
			}
			if (!contained && !mTriggeringUnits.Contains(unitID))
			{
				mTriggeringUnits.Add(unitID);
			}
		}
		int needed = StartData.GetObjectProperty<int>("TriggersNeeded");
		if (needed < 2)
		{
			needed = 1;
		}
		if (mTriggeringUnits.Count < needed)
		{
			return;
		}
		World.PlaySecretGet();
		OpenDoor(true);
		if (ProtoUnit.IsTag("ShowHelperArrow"))
		{
			int arrowID;
			if (World.SpawnUnitAtPosition("HelperArrow", mTilePosition, out arrowID) == FRoom.SpawnUnitResult.Success)
			{
				FUnit arrow = World.GetUnitByID(arrowID);
				if (arrow != null)
				{
					FPoint2 lookAt = mTilePosition + BackFP2();
					arrow.FacePosition(lookAt);
					arrow.mDying = true;
				}
			}
		}
	}

	public void EnableInput(bool enable)
	{
		if (mAI != null)
		{
			mAI.InputEnabled = enable;
		}
	}


	public virtual bool ProcessMessage(FUnitMessage m)
	{
		if (mAI != null && mAI.ProcessMessage(m))
		{
			return true;
		}
		bool handled = false;
		switch (m.Message)
		{
		case "Unlock":
			SetDoorLocked(false);
			handled = true;
			break;
		case "OpenDoor":
			OpenDoorImmediate();
			handled = true;
			break;
		case "Trigger":
			if (Room != World.CurrentRoom && !mAnimController.Animator.SpriteRenderer.isVisible && !m.Data.Contains("NoReveal"))
			{
				World.TryTriggerReveal(this, "Trigger");
				handled = true;
				break;
			}
			Trigger(m.SenderID);
			handled = true;
			break;
		case "Interact":
			World.TryShowDialogue(this);
			handled = true;
			break;
		case "Lock":
			SetDoorLocked(true);
			handled = true;
			break;
		case "FakeOpen":
			PlayAnim("Idle_CrackedOpen", true, null, true);
			handled = true;
			break;
		case "FlashUIButton":
			if (m.Data.Valid())
			{
				FApp.HUD.PageLegendWorld.FlashButton(m.Data);
			}
			handled = true;
			break;
		case "Close":
		{
			string closeAnim = ((mAnimController.Animator.CurrentAnimTypeID == FAnimDefManager.GetAnimTypeID("Idle_CrackedOpen")) ? "Close_CrackedOpen" : "Close");
			PlayAnim(closeAnim, false, null, true);
			handled = true;
			break;
		}
		case "CloseDoor":
			CloseDoorImmediate();
			handled = true;
			break;
		case "Collected":
		{
			FUnit collected = World.GetUnitByID(m.SenderID);
			if (collected != null)
			{
				AddFloatyLoot(collected);
			}
			handled = true;
			break;
		}
		}
		if (!m.Data.Contains("Forwarded"))
		{
			TryForwardMessage(m);
		}
		return handled;
	}

	protected void TryForwardMessage(FUnitMessage m)
	{
		if ((mUnitsToForwardMessages == null || mUnitsToForwardMessages.Length == 0) && !TryGetForwardUnits())
		{
			return;
		}
		m.Data += "|Forwarded";
		for (int i = 0; i < mUnitsToForwardMessages.Length; i++)
		{
			FUnit unit = World.GetUnitByID(mUnitsToForwardMessages[i]);
			if (unit != null)
			{
				unit.ProcessMessage(m);
			}
		}
	}

	protected bool TryGetForwardUnits()
	{
		string forwardTo = StartData.GetObjectProperty<string>("ForwardMessagesTo");
		if (!forwardTo.Valid())
		{
			return false;
		}
		List<int> ids = new List<int>();
		string[] names = forwardTo.Split(new string[1] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < names.Length; i++)
		{
			FUnit[] units = World.GetUnitsByName(names[i].Trim());
			if (units != null && units.Length != 0)
			{
				ids.AddRange(System.Linq.Enumerable.Select(units, (FUnit u) => u.ID));
			}
		}
		if (ids.Count < 1)
		{
			mUnitsToForwardMessages = null;
		}
		else
		{
			mUnitsToForwardMessages = ids.ToArray();
		}
		return mUnitsToForwardMessages != null;
	}

	public void FireEndTag(int actionID)
	{
	}

	public void FireLoopTag(int actionID)
	{
	}

	public void FireTag(FAnimTag tag, int actionID)
	{
		switch (tag.TagType)
		{
		case AnimTagTypes.Attack:
			Attack();
			break;
		case AnimTagTypes.Sound:
		{
			FAnimTagSound soundTag = tag as FAnimTagSound;
			if (soundTag != null)
			{
				string soundName = (string.IsNullOrEmpty(soundTag.SoundSetNameVO) ? soundTag.SoundSetName : soundTag.SoundSetNameVO);
				if (!string.IsNullOrEmpty(soundName))
				{
					PlaySoundSet(soundName);
				}
			}
			break;
		}
		case AnimTagTypes.CallMethod:
		{
			FAnimTagCallMethod callTag = tag as FAnimTagCallMethod;
			if (callTag != null)
			{
				System.Action method = FCoreUtil.GetDelegate<System.Action>(this, callTag.MethodName);
				if (method != null)
				{
					method();
				}
			}
			break;
		}
		case AnimTagTypes.RemoveUnit:
			FApp.World.RemoveUnit(this);
			break;
		case AnimTagTypes.DropLoot:
			DropLoot(true);
			break;
		case AnimTagTypes.SetAlive:
		{
			FAnimTagSetAlive aliveTag = tag as FAnimTagSetAlive;
			if (aliveTag != null)
			{
				mAlive = aliveTag.Alive;
				if (!mAlive)
				{
					DoLoot();
				}
				else
				{
					mDying = false;
				}
			}
			break;
		}
		case AnimTagTypes.MoveToPatrolPoint:
		{
			FAnimTagMoveToPatrolPoint patrolTag = tag as FAnimTagMoveToPatrolPoint;
			if (mAI != null && patrolTag != null && patrolTag.Index >= 0)
			{
				if (mAI.PatrolPoints.Count > patrolTag.Index)
				{
					SetObstructs(false);
					mAI.StopActions();
					mAI.DoMoveToPoint(mAI.PatrolPoints[patrolTag.Index], 0, false, false, false, string.Empty, -1f);
				}
			}
			break;
		}
		case AnimTagTypes.GrantQuest:
		{
			FAnimTagGrantQuest grantTag = tag as FAnimTagGrantQuest;
			if (grantTag != null)
			{
				FApp.PlayerStats.StartQuest(grantTag.DBID);
			}
			break;
		}
		case AnimTagTypes.ProcessMessage:
		{
			FAnimTagProcessMessage messageTag = tag as FAnimTagProcessMessage;
			if (messageTag != null)
			{
				SendMessage(this, messageTag.Name, messageTag.Data);
			}
			break;
		}
		case AnimTagTypes.CompleteQuest:
		{
			FAnimTagCompleteQuest completeTag = tag as FAnimTagCompleteQuest;
			if (completeTag != null)
			{
				string lootTable = string.Empty;
				FApp.PlayerStats.CompleteQuest(completeTag.DBID, out lootTable);
				string oldOverride = StartData.LootTableOverride;
				StartData.LootTableOverride = lootTable;
				DropLoot(true);
				StartData.LootTableOverride = oldOverride;
			}
			break;
		}
		case AnimTagTypes.PlayAnimation:
		{
			FAnimTagPlayAnimation playTag = tag as FAnimTagPlayAnimation;
			if (playTag != null)
			{
				PlayAnim(playTag.Name, true, null, true);
			}
			break;
		}
		case AnimTagTypes.TriggerUnit:
		{
			FAnimTagTriggerUnit triggerTag = tag as FAnimTagTriggerUnit;
			if (triggerTag != null)
			{
				FUnit target = World.GetUnitByName(triggerTag.Name);
				if (target != null)
				{
					target.Trigger(ID);
				}
			}
			break;
		}
		case AnimTagTypes.SpawnEffect:
		{
			FAnimTagSpawnEffect effectTag = tag as FAnimTagSpawnEffect;
			if (effectTag != null)
			{
				World.SpawnEffect(effectTag.Name, Position, 200);
			}
			break;
		}
		case AnimTagTypes.FillPlayerHealth:
			if (tag is FAnimTagFillPlayerHealth)
			{
				World.FillPlayerHealth();
			}
			break;
		}
	}



	public void Show()
	{
		SetVisible(true);
	}

	public void Hide()
	{
		SetVisible(false);
	}

	public bool IsInCombat(float lastAttackWindow)
	{
		if (mAI != null && mAI.LastAttackTargetID > -1)
		{
			float gametime = World.Gametime;
			float lastAttack = mAI.LastAttackTime;
			FUnit target = World.GetUnitByID(mAI.LastAttackTargetID);
			if (target != null && (gametime - lastAttack < lastAttackWindow || (target.mAI != null && target.mAI.GetCurrentTargetID() == ID)))
			{
				return true;
			}
		}
		return false;
	}

	protected void HackWinPrototype()
	{
		FApp.HUD.PageLegendWorld.DialogueCompleteAction += World.HackWinPrototype;
	}

	protected void HackStartEnding()
	{
		FApp.HUD.PageLegendWorld.DialogueCompleteAction += World.HackDoEnding;
	}

	public FAnimationController AddAnimationLayer(string animDefName, string animName, bool topmost = true, Vector2 offset = default(Vector2), float scale = 1f, bool loop = true)
	{
		if (GO == null)
		{
			FLog.UnityWarning("Trying to add animlayer to null GO.");
			return null;
		}
		FAnimDef animDef = FAnimDefManager.GetAnimDef(animDefName);
		if (animDef == null)
		{
			FLog.UnityWarning("No anim def for visual layer '" + animDefName + "' found.");
			return null;
		}
		int animType = FAnimDefManager.GetAnimTypeID(animName);
		if (animType < 0)
		{
			return null;
		}
		if (mAnimLayers == null)
		{
			mAnimLayers = new Dictionary<string, FAnimationController>();
		}
		GameObject layerGO = new GameObject(Name + "_AnimationLayer_" + mAnimLayers.Count.ToString());
		FUtil.GetOrCreateComponent<SpriteRenderer>(layerGO).sortingLayerName = "Foreground";
		layerGO.transform.parent = GO.transform;
		layerGO.transform.localPosition = new Vector3(offset.x, offset.y, 0f);
		layerGO.transform.localEulerAngles = Vector3.zero;
		layerGO.transform.localScale = layerGO.transform.localScale * scale;
		string key = GetAnimLayerName(animDefName, animName) + "_" + (mNextAnimID++).ToString();
		mAnimLayers[key] = new FAnimationController(animDefName, animDef, layerGO);
		mAnimLayers[key].PlayAnim(animType, loop, null, false, true);
		mAnimLayers[key].SetFacing(Facing);
		if (topmost)
		{
			layerGO.transform.SetAsFirstSibling();
		}
		SetSortingOrder(mBaseSortingOrder);
		return mAnimLayers[key];
	}

	protected string GetAnimLayerName(string animDefName, string animName)
	{
		return animDefName + "_" + animName;
	}

	public int GetNextAnimID()
	{
		return mNextAnimID++;
	}

	public void RemoveAnimationLayer(string animDefName, string animName)
	{
		if (mAnimLayers == null)
		{
			return;
		}
		string prefix = GetAnimLayerName(animDefName, animName);
		int i = 0;
		while (i < System.Linq.Enumerable.ToList(System.Linq.Enumerable.Where(mAnimLayers, (KeyValuePair<string, FAnimationController> kvp) => kvp.Key.StartsWith(prefix))).Count)
		{
			RemoveAnimationLayer(System.Linq.Enumerable.ToList(System.Linq.Enumerable.Where(mAnimLayers, (KeyValuePair<string, FAnimationController> kvp) => kvp.Key.StartsWith(prefix)))[i].Value);
			i++;
		}
	}

	public bool HasAnimationLayer(string animDefName, string animName)
	{
		if (mAnimLayers == null)
		{
			return false;
		}
		return System.Linq.Enumerable.Any(mAnimLayers.Keys, (string k) => k.StartsWith(GetAnimLayerName(animDefName, animName)));
	}

	public void RemoveAnimationLayer(FAnimationController animLayer)
	{
		if (mAnimLayers == null)
		{
			return;
		}
		int i = 0;
		while (i < System.Linq.Enumerable.ToList(System.Linq.Enumerable.Where(mAnimLayers, (KeyValuePair<string, FAnimationController> kvp) => kvp.Value == animLayer)).Count)
		{
			KeyValuePair<string, FAnimationController> entry = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Where(mAnimLayers, (KeyValuePair<string, FAnimationController> kvp) => kvp.Value == animLayer))[i];
			entry.Value.GO.transform.parent = null;
			Object.Destroy(entry.Value.GO);
			mAnimLayers.Remove(entry.Key);
			i++;
		}
	}

	public void ShowEmote(string emote, bool loop, float offsetX = -0.5f, float offsetY = 0.5f)
	{
		if (mCurrentEmote != null)
		{
			ClearEmote();
		}
		int emoteType = FAnimDefManager.GetAnimTypeID(emote);
		if (emoteType > -1)
		{
			mCurrentEmoteOffset = new Vector2(offsetX, offsetY);
			mCurrentEmote = AddAnimationLayer("Emotes", "Spawn", true, new Vector2(offsetX, offsetY), 1f, false);
			if (mCurrentEmote != null)
			{
				mCurrentEmote.Animator.SpriteRenderer.sortingOrder = 200;
				mCurrentEmote.QueueAnim(emoteType, loop);
				mCurrentEmote.QueueAnim(FAnimDefManager.GetAnimTypeID("Die"), false);
			}
		}
	}

	protected void SyncEmote()
	{
		if (mCurrentEmote != null && !(mCurrentEmote.GO == null))
		{
			Transform emoteT = mCurrentEmote.GO.transform;
			emoteT.localPosition = new Vector3(mCurrentEmoteOffset.x, mCurrentEmoteOffset.y, 0f);
			Vector3 pos = emoteT.position;
			Vector2 clamped = World.Camera.ClampToViewRect(new Vector2(pos.x, pos.y), Vector2.one);
			emoteT.position = new Vector3(clamped.x, clamped.y, pos.z);
		}
	}

	public int GetPurchasePrice()
	{
		if (mPurchasePrice > 0)
		{
			return mPurchasePrice;
		}
		return ProtoUnit.PurchasePrice;
	}

	public void ShowCountdown(bool show, float resetTime = 1f)
	{
		if (mCountdownAnim != null)
		{
			RemoveAnimationLayer("CountdownTimer", "Play");
			mCountdownAnim = null;
		}
		if (show)
		{
			mCountdownAnim = AddAnimationLayer("CountdownTimer", "Play", true, new Vector2(0f, 0.5f), 0.75f, true);
			mCountdownAnim.Animator.SetAnimSpeed(1f / resetTime);
			if (mCountdownAnim.GO != null)
			{
				mCountdownAnim.GO.tag = "DontUpdate";
			}
			SpriteRenderer renderer = mCountdownAnim.Animator.SpriteRenderer;
			if (renderer != null)
			{
				renderer.sortingLayerName = "Foreground";
				renderer.sortingOrder += 300;
			}
		}
	}

	public string GetSavedUnitInfo(string key)
	{
		if (FApp.PlayerStats != null)
		{
			return FApp.PlayerStats.GetSavedUnitInfo(this, key);
		}
		return null;
	}

	public void SetSavedUnitInfo(string key, string value)
	{
		FApp.PlayerStats.SetSavedUnitInfo(this, key, value);
	}

	public void SetSavedUnitInfo(string key, object value)
	{
		SetSavedUnitInfo(key, value.ToString());
	}

	public bool HasSavedUnitInfo(string key)
	{
		return FApp.PlayerStats.GetSavedUnitInfo(this, key).Valid();
	}

	public T GetSavedUnitInfo<T>(string key)
	{
		object value = default(T);
		if (FApp.PlayerStats != null)
		{
			if (HasSavedUnitInfo(key))
			{
				value = FApp.PlayerStats.GetSavedUnitInfo(this, key);
			}
			if (!(value is T))
			{
				value = System.Convert.ChangeType(value, typeof(T));
			}
		}
		return (T)value;
	}

	public bool IsSecret()
	{
		return StartData.IsSecret();
	}

	public bool TrySwitchToRequiredCharacter(FProtoUnit targetUnit)
	{
		if (!IsPlayer)
		{
			return false;
		}
		string currentCharacter = FApp.PlayerStats.CurrentCharacter;
		string requiredCharacter = targetUnit.GetInteractRequirementsRequiredCharacter();
		if (string.IsNullOrEmpty(requiredCharacter))
		{
			return false;
		}
		if (!(requiredCharacter != FApp.PlayerStats.CurrentCharacter))
		{
			return false;
		}
		if (!FApp.PlayerStats.HasCollectible(requiredCharacter) && requiredCharacter != "Hopper")
		{
			return false;
		}
		World.SetPlayerCharacter(requiredCharacter);
		if (targetUnit.CheckInteractRequirements())
		{
			return true;
		}
		World.SetPlayerCharacter(currentCharacter);
		return false;
	}
}
