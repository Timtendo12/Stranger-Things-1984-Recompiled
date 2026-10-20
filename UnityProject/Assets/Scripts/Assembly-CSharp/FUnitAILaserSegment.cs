using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAILaserSegment @ 0x174ab88..0x174bae8, Ghidra vaddr).
public class FUnitAILaserSegment : FUnitAIBase
{
	protected List<FUnit> mUnitsToHit = new List<FUnit>();

	protected FUnitAILaserSegment mChild;

	protected int mObstructingUnits;

	protected bool mFirstBorn;

	public FUnitAILaserSegment(FUnit owner)
		: base(owner)
	{
	}

	protected override void Init()
	{
		base.Init();
		FTile tile = mOwner.World.GetTile(mOwner.TilePosition);
		bool blocked;
		if (tile == null)
		{
			blocked = true;
		}
		else if (!tile.GetPassable(null, new List<int>()))
		{
			blocked = !tile.Hole;
		}
		else
		{
			blocked = false;
		}
		string idle;
		if (!mFirstBorn)
		{
			bool ended = blocked || mObstructingUnits != 0;
			bool endsHere = blocked || mOwner.Facing == FAnimEntry.AnimFacing.UP;
			bool reachedEnd = false;
			if (!endsHere)
			{
				FTile ahead = mOwner.World.GetTile(mOwner.TilePosition + mOwner.ForwardFP2());
				if (ahead != null && (ahead.TerrainPassable || ahead.Hole))
				{
					for (int i = 0; i < ahead.Units.Count; i++)
					{
						FUnit unit = ahead.Units[i];
						if (unit.Obstructs && !unit.IsTag("Emitter") && !unit.IsTag("Attackable") && unit.AliveAndWell)
						{
							reachedEnd = true;
							break;
						}
					}
				}
				else
				{
					reachedEnd = true;
				}
			}
			if (!reachedEnd && !ended)
			{
				idle = "Idle";
				SpawnChild();
			}
			else
			{
				idle = (blocked ? "End" : "EndFull");
			}
		}
		else
		{
			idle = ((mOwner.Facing == FAnimEntry.AnimFacing.DOWN) ? "Idle_Start" : "Idle");
			SpawnChild();
		}
		mOwner.SetIdle(idle, false, true);
	}

	protected void SpawnChild()
	{
		FProtoUnitStartData pusd = new FProtoUnitStartData();
		pusd.Position = mOwner.TilePosition + mOwner.ForwardFP2();
		pusd.Facing = mOwner.Facing;
		pusd.ProtoUnit = mOwner.ProtoUnit.Name;
		pusd.Name = mOwner.Name;
		int newID = -1;
		if (mOwner.Room.SpawnUnit(pusd, out newID) == FRoom.SpawnUnitResult.Success && newID >= 0)
		{
			FUnit child = mOwner.World.GetUnitByID(newID);
			mChild = child.AI as FUnitAILaserSegment;
			if (mChild != null)
			{
				mChild.Init();
			}
		}
	}

	public override void Update(float deltaTime)
	{
		if (!mOwner.Visible)
		{
			return;
		}
		base.Update(deltaTime);
		for (int i = 0; i < mUnitsToHit.Count; i++)
		{
			DoHitUnit(mUnitsToHit[i]);
		}
		mUnitsToHit.Clear();
		if (mObstructingUnits < 1)
		{
			if (mChild != null)
			{
				return;
			}
		}
		else if (mChild == null)
		{
			return;
		}
		SyncObstructions();
	}

	protected void DoHitUnit(FUnit unit)
	{
		if (!mOwner.Visible)
		{
			return;
		}
		if (!unit.IsTag("Attackable") && !unit.IsTag("Destructible"))
		{
			return;
		}
		Vector2 push = mOwner.Left();
		Vector2 lastTile = unit.LastTilePositionVector;
		Vector3 ownerPos = mOwner.Position;
		Vector2 right = mOwner.Right();
		if ((lastTile.x - ownerPos.x) * right.x + (lastTile.y - ownerPos.y) * right.y > 0f)
		{
			push = mOwner.Right();
		}
		if (unit.LastAttack != null && unit.AI.GetHasActionOfType<FUnitActionKnockback>())
		{
			FUnit attacker = mOwner.World.GetUnitByID(unit.LastAttack.ID);
			if (attacker != null && attacker != mOwner)
			{
				Vector2 d = attacker.Forward() - mOwner.Forward();
				if (d.x * d.x + d.y * d.y < 1E-10f && attacker.AI is FUnitAILaserSegment)
				{
					Vector3 unitPos = unit.Position;
					Vector3 attackerPos = attacker.Position;
					push = new Vector2(unitPos.x - attackerPos.x, unitPos.y - attackerPos.y);
				}
			}
		}
		unit.OnAttacked(mOwner, 1, push, 1, true);
		mOwner.PlayAnim("Pulse", false);
	}

	protected Vector2 GetNormalInDirToPoint(Vector2 pos)
	{
		Vector2 left = mOwner.Left();
		Vector2 right = mOwner.Right();
		Vector3 ownerPos = mOwner.Position;
		float dx = pos.x - ownerPos.x;
		float dy = pos.y - ownerPos.y;
		if (left.x * dx + left.y * dy <= right.x * dx + right.y * dy)
		{
			return right;
		}
		return left;
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		FUnit unit = GetUnitForMessage(m);
		string message = m.Message;
		if (message == "UnitAddedToTile")
		{
			if (!mOwner.Visible)
			{
				return false;
			}
			if (unit == null)
			{
				return false;
			}
			if (unit.IsTag("BlocksLaser"))
			{
				mObstructingUnits++;
				return true;
			}
			mUnitsToHit.Add(unit);
			return true;
		}
		if (message == "UnitRemovedFromTile")
		{
			if (!mOwner.Visible)
			{
				return false;
			}
			if (unit == null)
			{
				return false;
			}
			if (unit.IsTag("BlocksLaser"))
			{
				mObstructingUnits = Mathf.Max(mObstructingUnits - 1, 0);
			}
			return false;
		}
		if (message == "FirstBorn")
		{
			mFirstBorn = true;
			Init();
			return false;
		}
		if (message == "Hide")
		{
			mOwner.SetVisible(false);
			ForwardToChild(m);
			return false;
		}
		if (message == "Show")
		{
			mOwner.SetVisible(true);
			mOwner.SetSortingOrder(-50);
			ForwardToChild(m);
			return false;
		}
		return false;
	}

	protected void SyncObstructions()
	{
		if (mChild != null && mObstructingUnits > 0)
		{
			mOwner.World.RemoveUnit(mChild.mOwner);
			mChild = null;
		}
		Init();
	}

	protected void ForwardToChild(FUnitMessage m)
	{
		if (mChild != null)
		{
			mOwner.SendMessage(mChild.mOwner, m.Message);
		}
	}

	public override void ReleaseStuff()
	{
		base.ReleaseStuff();
		if (mChild != null)
		{
			mOwner.World.RemoveUnit(mChild.mOwner);
		}
	}

	protected FUnit GetUnitForMessage(FUnitMessage m)
	{
		FUnit unit = mOwner.World.GetUnitByID(m.SenderID);
		if (unit == null)
		{
			return null;
		}
		if (m.SenderID == mOwner.ID)
		{
			FTile tile = mOwner.World.GetTile(mOwner.TilePosition);
			for (int i = 0; i < tile.Units.Count; i++)
			{
				FUnit other = tile.Units[i];
				if (other.ID != mOwner.ID && other.Obstructs)
				{
					unit = other;
				}
			}
			if (unit == mOwner)
			{
				return null;
			}
		}
		if (!unit.Obstructs)
		{
			return null;
		}
		return unit;
	}
}
