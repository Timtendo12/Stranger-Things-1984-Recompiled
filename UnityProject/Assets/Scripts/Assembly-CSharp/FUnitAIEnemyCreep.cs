using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIEnemyCreep @ 0x1747408..0x1747b24, Ghidra vaddr).
public class FUnitAIEnemyCreep : FUnitAIEnemy
{
	protected bool mReverseNextPath;

	public FUnitAIEnemyCreep(FUnit owner)
		: base(owner)
	{
		mShouldAggro = false;
	}

	public override void Update(float deltaTime)
	{
		if (mSpawnPoint == FPoint2.invalid)
		{
			mSpawnPoint = mOwner.TilePosition;
		}
		base.Update(deltaTime);
	}

	protected override void AddIdleAction(float deltaTime)
	{
		AddAction(new FUnitActionEnemyIdleCreep(mOwner.World.GetNextActionID(), mOwner, mReverseNextPath));
	}

	public override bool CollidedWithUnit(FUnit other)
	{
		bool inputEnabled = InputEnabled;
		if (other == null || !inputEnabled || !other.IsPlayer)
		{
			mReverseNextPath = true;
			return true;
		}
		Vector2 forward = mOwner.GetDirFromFacing();
		Vector3 otherPos = other.Position;
		Vector3 myPos = mOwner.Position;
		if (forward.x * (otherPos.x - myPos.x) + forward.y * (otherPos.y - myPos.y) <= 0.4f)
		{
			return false;
		}
		FPoint2 dir = new FPoint2((int)forward.x, (int)forward.y);
		FPoint2 ahead = mOwner.TilePosition + dir;
		FTile aheadTile = mOwner.World.GetTile(ahead);
		List<FPoint2> sides = new List<FPoint2>();
		sides.Add(ahead + new FPoint2(dir.y, -dir.x));
		sides.Add(ahead + new FPoint2(-dir.y, dir.x));
		List<FPoint2> ordered = sides.OrderBy((FPoint2 p) =>
		{
			Vector3 pos = other.Position;
			float dx = (float)p.x - pos.x;
			float dy = (float)p.y - pos.y;
			return Mathf.Sqrt(dx * dx + dy * dy);
		}).ToList();
		FTile free = null;
		for (int i = 0; i < ordered.Count; i++)
		{
			FTile candidate = mOwner.World.GetTile(ordered[i]);
			if (candidate.GetPassable(other, new List<int>()))
			{
				free = candidate;
				break;
			}
		}
		FTile destination = ((free != null) ? free : aheadTile);
		if (destination != null)
		{
			Vector2 pushDir = destination.Position.ToVector2() - aheadTile.Position.ToVector2();
			other.OnAttacked(mOwner, pushDir);
			return false;
		}
		return false;
	}
}
