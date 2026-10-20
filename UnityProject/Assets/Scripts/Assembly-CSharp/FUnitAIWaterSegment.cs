using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIWaterSegment @ 0x174fe90..0x1751d60, Ghidra vaddr).
public class FUnitAIWaterSegment : FUnitAIBase
{
	protected enum SewerWaterState
	{
		NORMAL = 0,
		ENTRANCE = 1,
		EXIT = 2,
		CORNER = 3
	}

	protected int mFlowDirection;

	protected bool mIsVert;

	protected int mStartFlowDirection;

	protected FUnitAIWaterSegment mConnectedSegment;

	protected bool mLeadsToSegment;

	protected FAnimEntry.AnimFacing mFlowFacing = FAnimEntry.AnimFacing.DOWN;

	protected string mHackFlowAnimOverride;

	protected SewerWaterState mState;

	[CompilerGenerated]
	private Action m_mFlowDirUpdated;

	public int FlowDirection => mFlowDirection;

	public bool IsVert => mIsVert;

	public int StartFlowDirection => mStartFlowDirection;

	protected event Action mFlowDirUpdated
	{
		add
		{
			m_mFlowDirUpdated = (Action)Delegate.Combine(m_mFlowDirUpdated, value);
		}
		remove
		{
			m_mFlowDirUpdated = (Action)Delegate.Remove(m_mFlowDirUpdated, value);
		}
	}

	public event Action FlowDirUpdatedAction
	{
		add
		{
			mFlowDirUpdated -= value;
			mFlowDirUpdated += value;
		}
		remove
		{
			mFlowDirUpdated -= value;
		}
	}

	public FUnitAIWaterSegment(FUnit owner)
		: base(owner)
	{
		string direction = mOwner.StartData.GetObjectProperty<string>("FlowDirection");
		mIsVert = direction != null && direction.ToUpperInvariant() == "VERT";
		mStartFlowDirection = mOwner.StartData.GetObjectProperty<int>("StartFlow");
		mHackFlowAnimOverride = mOwner.StartData.GetObjectProperty<string>("FlowAnimOverride");
		mFlowDirection = mStartFlowDirection;
		string saved = mOwner.GetSavedUnitInfo("WaterDirection");
		if (saved.Valid())
		{
			SetFlowDir(int.Parse(saved));
		}
		SyncPathingCost();
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
	}

	protected override void Init()
	{
		base.Init();
		CheckIfTurn();
		FUnitAIWaterSegment connected = mConnectedSegment;
		if (connected == null)
		{
			FGameUtil.ExecuteNextFrame(delegate
			{
				CheckIfSpecialEdgeTile();
				if (mState != SewerWaterState.NORMAL)
				{
					SyncAnimation();
				}
			});
		}
		else
		{
			connected.mFlowDirUpdated -= SyncAnimation;
			connected.mFlowDirUpdated += SyncAnimation;
		}
		SyncAnimation();
	}

	protected void CheckIfSpecialEdgeTile()
	{
		SewerWaterState oldState = mState;
		mState = SewerWaterState.EXIT;
		FPoint2 startDir = GetStartFlowDir();
		FPoint2 ahead = mOwner.TilePosition + startDir;
		if (!mOwner.Room.ContainsFPoint(ahead))
		{
			FTile tile = mOwner.World.GetTile(ahead);
			if (tile == null)
			{
				return;
			}
			if (tile.Units.All((FUnit u) => !(u.AI is FUnitAIWaterSegment)))
			{
				return;
			}
		}
		mState = SewerWaterState.ENTRANCE;
		FPoint2 behind = mOwner.TilePosition - startDir;
		if (!mOwner.Room.ContainsFPoint(behind))
		{
			FTile tile2 = mOwner.World.GetTile(behind);
			if (tile2 == null)
			{
				return;
			}
			if (tile2.Units.All((FUnit u) => !(u.AI is FUnitAIWaterSegment)))
			{
				return;
			}
		}
		mState = oldState;
	}

	protected void CheckIfTurn()
	{
		int outerOffset = 0;
		for (int x = -1; x != 2; x++)
		{
			for (int y = -1; y != 2; y++)
			{
				if (outerOffset + y != -1 && x == 0 && (y == 0 || !mIsVert))
				{
					FTile tile = mOwner.World.GetTile(new FPoint2(mOwner.TilePosition.x, y + mOwner.TilePosition.y));
					if (tile != null)
					{
						FUnit other = tile.Units.FirstOrDefault((FUnit u) => u.ProtoUnit == mOwner.ProtoUnit);
						if (other != null && other.AI is FUnitAIWaterSegment segment && (mIsVert != segment.mIsVert || segment.mFlowDirection != mFlowDirection))
						{
							mConnectedSegment = segment;
						}
					}
				}
			}
			outerOffset--;
		}
	}

	protected void DoHitUnit(FUnit unit)
	{
		if (mState == SewerWaterState.EXIT)
		{
			KillUnit(unit);
			return;
		}
		if (mFlowDirection == 0)
		{
			return;
		}
		FPoint2 flowDir = GetFlowDir();
		if (!unit.AliveAndWell)
		{
			return;
		}
		if (unit.InputEnabled)
		{
			FApp.SoundManager.PlaySoundSet("Water_Enter");
		}
		FUnitActionBase current = unit.AI.GetCurrentAction();
		FUnitActionKnockback knockback = current as FUnitActionKnockback;
		Vector2 back = mOwner.Back();
		if (knockback == null || knockback.Direction.x * back.x + knockback.Direction.y * back.y < 0.75f)
		{
			unit.AI.StopActions();
		}
		FPoint2 target = mOwner.TilePosition + flowDir;
		unit.AI.DoMoveToPoint(target, 0, true, false, false, "FAKE_ANIMATION", 3.5f, null, MoveComplete, MoveComplete);
		if (unit.AnimController.HasAnimation("Hit"))
		{
			unit.PlayAnim("Hit");
		}
		unit.EnableInput(false);
	}

	protected void KillUnit(FUnit unit)
	{
		unit.SetObstructs(false);
		unit.RemoveAnimationLayer("SewerWaterSplash", "Play");
		Go.to(unit.GO.transform, 1f, new GoTweenConfig().scale(0.25f).setUpdateType(GoUpdateType.FWorldUpdate));
		FPoint2 dir = ((mFlowDirection == 0) ? GetStartFlowDir() : GetFlowDir());
		Vector2 offset = new Vector2(dir.x, 0f);
		if (mIsVert)
		{
			offset = new Vector2(0f, dir.y);
		}
		Go.to(unit.GO.transform, 0.75f, new GoTweenConfig().localPosition(new Vector3(offset.x, offset.y, 0f), true).setUpdateType(GoUpdateType.FWorldUpdate));
		unit.AI.StopActions();
		unit.AI.AddAction(new FUnitActionWait(mOwner.World.GetNextActionID(), unit, 2f));
		unit.PlayAnim("Hit");
		unit.SetColor(new Color(0f, 0f, 0f, 1f), true, true, 1f);
		unit.World.RemoveUnit(unit, 1.2f);
		FApp.SoundManager.PlaySoundSet("Waterfall_Death");
	}

	protected FPoint2 GetStartFlowDir()
	{
		if (mIsVert)
		{
			return new FPoint2(0, mStartFlowDirection);
		}
		return new FPoint2(mStartFlowDirection, 0);
	}

	protected FPoint2 GetFlowDir()
	{
		FUnitAIWaterSegment source = ((mConnectedSegment != null && mLeadsToSegment) ? mConnectedSegment : this);
		if (source.mIsVert)
		{
			return new FPoint2(0, source.mFlowDirection);
		}
		return new FPoint2(source.mFlowDirection, 0);
	}

	protected void MoveComplete(FUnit unit)
	{
		unit.SetIdle();
		unit.EnableInput(true);
		if (!unit.IsPlayer)
		{
			if (!unit.Room.ContainsFPoint(unit.TilePosition))
			{
				unit.World.RemoveUnit(unit);
			}
		}
		else
		{
			unit.World.PlayerMoveComplete(false);
		}
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		FUnit unit = null;
		string message = m.Message;
		if (message == "UnitAddedToTile")
		{
			if (!mOwner.Visible || m.SenderID < 0)
			{
				return false;
			}
			unit = ((m.SenderID == mOwner.ID) ? GetUnitOnTileThatIsntMe() : mOwner.World.GetUnitByID(m.SenderID));
			if (CanHitUnit(unit))
			{
				if (!unit.HasAnimationLayer("SewerWaterSplash", "Play"))
				{
					unit.AddAnimationLayer("SewerWaterSplash", "Play");
					if (mFlowDirection == 0)
					{
						FApp.SoundManager.PlaySoundSet("Water_Enter");
					}
				}
				FUnit hit = unit;
				FGameUtil.ExecuteAtEndOfFrame(delegate
				{
					DoHitUnit(hit);
				});
				return true;
			}
			return false;
		}
		if (message == "UnitRemovedFromTile")
		{
			if (mOwner.Visible && m.SenderID >= 0)
			{
				unit = ((m.SenderID == mOwner.ID) ? GetUnitOnTileThatIsntMe() : mOwner.World.GetUnitByID(m.SenderID));
				if (CanHitUnit(unit))
				{
					unit.RemoveAnimationLayer("SewerWaterSplash", "Play");
				}
			}
			return false;
		}
		if (message == "-1" || message == "0" || message == "1")
		{
			if (!(mOwner.World.CurrentRoom != mOwner.Room) || m.Data.Contains("NoReveal"))
			{
				SetFlowDir(int.Parse(m.Message));
			}
			else
			{
				mOwner.World.TryTriggerReveal(mOwner, m.Message);
			}
			mOwner.SetSavedUnitInfo("WaterDirection", m.Message);
			return true;
		}
		return false;
	}

	protected bool CanHitUnit(FUnit unit)
	{
		if (unit == null)
		{
			return false;
		}
		return !unit.IsTag("Emitted");
	}

	protected FUnit GetUnitOnTileThatIsntMe()
	{
		FTile tile = mOwner.World.GetTile(mOwner.TilePosition);
		if (tile == null)
		{
			return null;
		}
		FUnit result = null;
		for (int i = 0; i < tile.Units.Count; i++)
		{
			FUnit unit = tile.Units[i];
			if (unit != mOwner && unit.ID != mOwner.ID && unit.Obstructs)
			{
				result = unit;
			}
		}
		return result;
	}

	protected void SetFlowDir(int dir)
	{
		int oldDirection = mFlowDirection;
		mFlowDirection = mStartFlowDirection * dir;
		SyncAnimation();
		if (oldDirection == 0 && mFlowDirection != 0)
		{
			FUnit unit = GetUnitOnTileThatIsntMe();
			if (CanHitUnit(unit))
			{
				DoHitUnit(unit);
			}
		}
		SyncPathingCost();
		if (m_mFlowDirUpdated != null)
		{
			m_mFlowDirUpdated();
		}
	}

	protected void SyncPathingCost()
	{
		FTile tile = mOwner.World.GetTile(mOwner.TilePosition);
		if (tile != null)
		{
			tile.SetUnitAffectsPathingCost(mOwner, mFlowDirection != 0);
		}
	}

	protected void SyncAnimation()
	{
		string anim;
		if (mState == SewerWaterState.ENTRANCE)
		{
			anim = ((mFlowDirection != 0) ? "Entrance" : "Idle_Entrance");
		}
		else
		{
			anim = ((mState == SewerWaterState.EXIT) ? "Exit" : string.Empty);
		}
		if (mFlowDirection == 0)
		{
			bool notPositive = mStartFlowDirection != 1;
			FAnimEntry.AnimFacing facing = (notPositive ? FAnimEntry.AnimFacing.LEFT : FAnimEntry.AnimFacing.RIGHT);
			FAnimEntry.AnimFacing alternate = (notPositive ? FAnimEntry.AnimFacing.DOWN : FAnimEntry.AnimFacing.UP);
			if (mIsVert)
			{
				facing = alternate;
			}
			mFlowFacing = facing;
			mOwner.SetFacing(facing);
			mOwner.SetIdle(anim.Valid() ? anim : "Idle", true, true);
			return;
		}
		FAnimEntry.AnimFacing flowFacing = ((mFlowDirection == 1) ? FAnimEntry.AnimFacing.RIGHT : FAnimEntry.AnimFacing.LEFT);
		FAnimEntry.AnimFacing flowAlternate = ((mFlowDirection == 1) ? FAnimEntry.AnimFacing.UP : FAnimEntry.AnimFacing.DOWN);
		if (mIsVert)
		{
			flowFacing = flowAlternate;
		}
		mFlowFacing = flowFacing;
		if (mConnectedSegment == null)
		{
			mOwner.SetFacing(flowFacing);
		}
		else
		{
			FUnitAIWaterSegment connected = mConnectedSegment;
			FUnit connectedOwner = connected.mOwner;
			bool leads;
			if (!connected.mIsVert)
			{
				leads = ((connected.mFlowDirection < 1) ? (connectedOwner.TilePosition.x < mOwner.TilePosition.x) : (mOwner.TilePosition.x < connectedOwner.TilePosition.x));
			}
			else
			{
				leads = ((0 < connected.mFlowDirection) ? (mOwner.TilePosition.y < connectedOwner.TilePosition.y) : (connectedOwner.TilePosition.y < mOwner.TilePosition.y));
			}
			mLeadsToSegment = leads;
			mOwner.SetFacing(flowFacing);
			if (!mLeadsToSegment)
			{
				Vector2 d1 = mConnectedSegment.mOwner.Left() - mOwner.Forward();
				anim = ((1E-10f <= d1.x * d1.x + d1.y * d1.y) ? "Turn_Right" : "Turn_Left");
				mOwner.SetFacing(mConnectedSegment.mOwner.Facing);
			}
			else
			{
				Vector2 d2 = mOwner.Left() - mConnectedSegment.mOwner.Forward();
				anim = ((d2.x * d2.x + d2.y * d2.y < 1E-10f) ? "Turn_Left" : "Turn_Right");
			}
		}
		if (!anim.Valid())
		{
			anim = "Flow";
		}
		if (mHackFlowAnimOverride.Valid())
		{
			anim = mHackFlowAnimOverride;
		}
		mOwner.PlayAnim(anim, true, 0, true);
	}

	private void SetFacingFromDir(int dir)
	{
		FAnimEntry.AnimFacing facing = ((dir == 1) ? FAnimEntry.AnimFacing.RIGHT : FAnimEntry.AnimFacing.LEFT);
		FAnimEntry.AnimFacing alternate = ((dir == 1) ? FAnimEntry.AnimFacing.UP : FAnimEntry.AnimFacing.DOWN);
		if (mIsVert)
		{
			facing = alternate;
		}
		mFlowFacing = facing;
	}
}
