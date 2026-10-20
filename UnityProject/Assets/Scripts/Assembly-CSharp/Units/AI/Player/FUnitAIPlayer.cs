using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIPlayer @ 0x174c094..0x174c95c, Ghidra vaddr).
public class FUnitAIPlayer : FUnitAIBase
{
	public FUnitAIPlayer(FUnit owner)
		: base(owner)
	{
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
	}

	public override InputResult HandleWorldTap(Vector3 screenPos, bool considerUnits, bool force = false)
	{
		if (mOwner == null)
		{
			return InputResult.NotHandled;
		}
		FWorld world = mOwner.World;
		if (world == null)
		{
			return InputResult.NotHandled;
		}
		if (!InputEnabled)
		{
			return InputResult.NotHandled;
		}
		Vector3 worldPos = world.Camera.ScreenToWorldPoint(screenPos);
		Collider2D[] hits = Physics2D.OverlapPointAll(new Vector2(worldPos.x, worldPos.y));
		FTile tile = null;
		for (int i = 0; i < hits.Length; i++)
		{
			FUnit unit = world.GetUnitFromGO(hits[i].gameObject);
			if (considerUnits && unit != null && !(unit.Room != mOwner.World.CurrentRoom))
			{
				if (!unit.AliveAndWell)
				{
					return InputResult.Ignored;
				}
				InputResult result = HandleUnitTap(unit, force);
				if (result != InputResult.NotHandled)
				{
					world.FlashUnit(unit);
					return result;
				}
			}
			FTile hitTile = world.GetTileFromGO(hits[i].gameObject);
			if (hitTile != null)
			{
				tile = hitTile;
			}
		}
		if (tile == null)
		{
			tile = world.GetTile(new Vector2(worldPos.x, worldPos.y).ToFPoint2());
			if (tile == null)
			{
				return InputResult.NotHandled;
			}
		}
		FUnit topmost = tile.GetTopmostUnit("Interactable");
		if (considerUnits && topmost != null && !(topmost.Room != mOwner.World.CurrentRoom))
		{
			InputResult result2 = HandleUnitTap(topmost, force);
			if (result2 != InputResult.NotHandled)
			{
				mOwner.World.FlashUnit(topmost);
				return result2;
			}
		}
		return HandleTileTap(tile.Position, force);
	}

	public override InputResult HandleTileTap(FPoint2 tile, bool force = false)
	{
		if (tile == FPoint2.invalid)
		{
			return InputResult.NotHandled;
		}
		FTile target = mOwner.World.CurrentRoom.GetTile(tile);
		if (target == null)
		{
			tile = mOwner.World.CurrentRoom.GetClosestEntranceToPoint(tile);
			target = mOwner.World.CurrentRoom.GetTile(tile);
		}
		if (!target.GetPassable(FWorld.Player, new List<int>()))
		{
			tile = mOwner.Room.GetClosestOpenTile(tile, true).Position;
		}
		StopActions();
		if (!DoWork(tile))
		{
			return InputResult.NotHandled;
		}
		return InputResult.Handled;
	}

	public override InputResult HandleUnitTap(FUnit target, bool force = false)
	{
		if (target != null && !target.IsPlayer && target.IsTag("Interactable") && target.AliveAndWell && target.Interactable)
		{
			if (target.ID == CurrentTargetID && !force)
			{
				return InputResult.Ignored;
			}
			if (GetAvailableWorkAction(target) != null)
			{
				StopActions();
				if (!DoWork(target))
				{
					return InputResult.Ignored;
				}
				return InputResult.Handled;
			}
		}
		return InputResult.NotHandled;
	}

	protected override void TryEquipSpecialWeapon(FUnit targetUnit)
	{
		base.TryEquipSpecialWeapon(targetUnit);
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.Message == "Attacked" && m.SenderID >= 0)
		{
			bool result = !mOwner.World.InputEnabled;
			FUnit attackTarget = mOwner.AttackTarget;
			if (attackTarget != null)
			{
				result = !mOwner.World.InputEnabled || attackTarget.ID == m.SenderID;
			}
			if (CurrentTargetID == m.SenderID)
			{
				FUnit unit = mOwner.World.GetUnitByID(CurrentTargetID);
				if (unit != null)
				{
					result |= mOwner.IsAdjacent(unit);
				}
			}
			return result;
		}
		return false;
	}

	public override string GetAnimName(string anim)
	{
		if (mOwner.ProtoUnit.Name == "Hopper" && mOwner.World.ProtoWorld.UpsideDown)
		{
			anim += "_Hazmat";
		}
		return base.GetAnimName(anim);
	}
}
