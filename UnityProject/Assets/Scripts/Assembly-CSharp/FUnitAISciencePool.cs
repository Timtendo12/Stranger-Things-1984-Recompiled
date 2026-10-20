using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAISciencePool @ 0x174ed30..0x174f034, Ghidra vaddr).
public class FUnitAISciencePool : FUnitAIBase
{
	public FUnitAISciencePool(FUnit owner)
		: base(owner)
	{
	}

	protected void DoHitUnit(FUnit unit)
	{
		Vector3 pos = unit.Position;
		unit.OnAttacked(mOwner, 1, GetNormalInDirToPoint(new Vector2(pos.x, pos.y)), (1 < unit.HP) ? 1 : 0, true);
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
		FUnit unit = mOwner.World.GetUnitByID(m.SenderID);
		if (unit == null)
		{
			return false;
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
				return false;
			}
		}
		if (unit.IsPlayer && unit.Obstructs && m.Message == "UnitAddedToTile" && mOwner.Visible)
		{
			DoHitUnit(unit);
			return true;
		}
		return false;
	}
}
