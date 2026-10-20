using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIEnemyOwl @ 0x1747b24..0x1748e0c, Ghidra vaddr).
public class FUnitAIEnemyOwl : FUnitAIEnemy
{
	public enum OwlStates
	{
		Asleep = 0,
		Swooping = 1,
		Flying = 2,
		Idle = 3
	}

	protected OwlStates mState;

	protected float mIdleTime;

	protected bool mHitPlayerThisSwoop;

	protected float mSwoopTime;

	private List<FUnit> mUnitsHit = new List<FUnit>();

	public FUnitAIEnemyOwl(FUnit owner)
		: base(owner)
	{
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (!FWorld.Player.AliveAndWell)
		{
			return;
		}
		switch (mState)
		{
		case OwlStates.Asleep:
			if (!GetHasMoveAction())
			{
				float distance = mOwner.DistanceToUnitEdge(FWorld.Player, false);
				if (distance <= (float)mOwner.Weapon.Range)
				{
					SetState(EnemyState.AGGROD);
					StopActions();
					mOwner.SetIdle("Walk", true, true);
				}
				if (State == EnemyState.AGGROD)
				{
					FlyUp();
				}
			}
			break;
		case OwlStates.Swooping:
			if (!mHitPlayerThisSwoop)
			{
				mOwner.SyncTilePosition();
				if (HitPlayerThisFrame())
				{
					DoHitPlayer();
					mHitPlayerThisSwoop = true;
				}
			}
			break;
		case OwlStates.Flying:
			if (mActions.Count == 0)
			{
				if (!mOwner.IsPlayingAnim("Walk"))
				{
					mOwner.PlayAnim("Walk");
				}
				if (mOwner.World.Gametime - mSwoopTime > 2f)
				{
					DoSwoop();
				}
			}
			break;
		case OwlStates.Idle:
			if ((float)mOwner.ProtoUnit.LeashDistance <= mOwner.DistanceToUnitEdge(FWorld.Player, false))
			{
				DoMoveToPoint(mOwner.StartData.Position, 0, false, false, false, string.Empty, -1f);
				mState = OwlStates.Asleep;
			}
			else if (!(mOwner.World.Gametime - mIdleTime < 3f))
			{
				FlyUp();
			}
			break;
		}
	}

	protected void DoHitPlayer()
	{
		if (!InputEnabled)
		{
			return;
		}
		FUnit player = FWorld.Player;
		List<FPoint2> candidates = new List<FPoint2>();
		candidates.Add(player.TilePosition + new FPoint2(0, 1));
		// 0x1748340: FRandom.Generic.RangeInt(0, 1) (w1=0, w2=1), branch on == 0 as written here.
		if (FRandom.Generic.RangeInt(0, 1) == 0)
		{
			candidates.Add(player.TilePosition + new FPoint2(0, -1));
		}
		else
		{
			candidates.Insert(0, player.TilePosition + new FPoint2(0, -1));
		}
		FTile free = null;
		for (int i = 0; i < candidates.Count; i++)
		{
			FTile tile = mOwner.World.GetTile(candidates[i]);
			if (tile != null && tile.GetPassable(player, new List<int>()))
			{
				free = tile;
				break;
			}
		}
		Vector2 pushDir;
		if (free == null)
		{
			pushDir = Vector2.zero;
		}
		else
		{
			Vector2 tilePos = free.Position.ToVector2();
			Vector3 playerPos = player.Position;
			pushDir = new Vector2(tilePos.x - playerPos.x, tilePos.y - playerPos.y);
		}
		player.OnAttacked(mOwner, pushDir);
	}

	protected bool HitPlayerThisFrame()
	{
		if (!mOwner.Visible)
		{
			return false;
		}
		Vector3 pos = mOwner.Position;
		int count = mOwner.World.GetUnitsByCircleCast(new Vector2(pos.x, pos.y), 0.4f, mUnitsHit);
		for (int i = 0; i < count; i++)
		{
			if (mUnitsHit[i].IsPlayer)
			{
				return true;
			}
		}
		return false;
	}

	protected void DoSwoop()
	{
		FUnit player = FWorld.Player;
		int offsetX = ((mOwner.TilePosition.x < player.TilePosition.x) ? 2 : (-2));
		FTile tile = mOwner.Room.GetClosestOpenTile(new FPoint2(player.TilePosition.x + offsetX, player.TilePosition.y), true, player, true);
		if (tile == null)
		{
			return;
		}
		Vector3 start = mOwner.Position;
		Vector2 end = tile.Position.ToVector2();
		List<Vector3> nodes = new List<Vector3>();
		nodes.Add(start);
		nodes.Add(new Vector3(start.x + (end.x - start.x) * 0.5f + Vector3.down.x * 2f, start.y + (end.y - start.y) * 0.5f + Vector3.down.y * 2f, start.z + (0f - start.z) * 0.5f + Vector3.down.z * 2f));
		nodes.Add(new Vector3(end.x, end.y, 0f));
		GoSpline spline = new GoSpline(nodes);
		mOwner.SetFacing(FUnit.GetFacingFromDir(new Vector2(end.x - mOwner.Position.x, 0f)));
		float delay;
		if (mState == OwlStates.Asleep)
		{
			mOwner.PlayAnim("WakeUp", false);
			mOwner.QueueAnim("Attack");
			delay = mOwner.GetCurrentAnimLength();
		}
		else
		{
			mOwner.PlayAnim("Attack");
			delay = 1f;
		}
		Vector3 now = mOwner.Position;
		float travel = Mathf.Sqrt((end.x - now.x) * (end.x - now.x) + (end.y - now.y) * (end.y - now.y)) * 0.25f;
		float duration = ((0.4f <= travel) ? travel : 0.4f);
		Go.to(mOwner.GO.transform, duration, new GoTweenConfig().positionPath(spline).setDelay(delay).onComplete(delegate
		{
			SwoopComplete();
		}).setUpdateType(GoUpdateType.FWorldUpdate));
		mHitPlayerThisSwoop = false;
		mState = OwlStates.Swooping;
	}

	protected void SwoopComplete()
	{
		mOwner.SyncTilePosition();
		mOwner.PlayAnim("Walk");
		mState = OwlStates.Idle;
		mIdleTime = mOwner.World.Gametime;
	}

	protected void FlyUp()
	{
		FPoint2 target = new FPoint2(0, 0);
		int x = mOwner.TilePosition.x;
		float side = ((mOwner.Position.x - FWorld.Player.Position.x >= 0f) ? 1f : (-1f));
		x += (int)side * 2;
		int y = Mathf.Max(mOwner.TilePosition.y + 2, FWorld.Player.TilePosition.y + 2);
		target = new FPoint2(x, y);
		target = mOwner.Room.ClampPoint(target);
		DoMoveToPoint(target, 0, false, false, false, string.Empty, -1f);
		mState = OwlStates.Flying;
		mSwoopTime = mOwner.World.Gametime;
	}

	protected override void AddIdleAction(float deltaTime)
	{
	}

	public override bool CollidedWithUnit(FUnit other)
	{
		return false;
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		bool handled = base.ProcessMessage(m);
		if (!handled)
		{
			if (m.Message == "Attacked")
			{
				if (m.SenderID >= 0)
				{
					FUnit attacker = mOwner.World.GetUnitByID(m.SenderID);
					if (attacker != null)
					{
						if (mState == OwlStates.Swooping)
						{
							Go.killAllAbstractTweensWithTarget(mOwner.GO.transform);
							SwoopComplete();
						}
						if (mState == OwlStates.Flying)
						{
							mSwoopTime = mOwner.World.Gametime;
						}
					}
				}
			}
			else if (m.Message == "Stunned" && mState == OwlStates.Swooping)
			{
				Go.killAllAbstractTweensWithTarget(mOwner.GO.transform);
				SwoopComplete();
			}
		}
		return handled;
	}
}
