using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBear @ 0x172f7a4..0x1730728, Ghidra vaddr).
public class FUnitAIBear : FUnitAIBase
{
	protected FPath mPath;

	protected List<FUnit> mTriggerUnits = new List<FUnit>();

	protected bool mOnTriggerUnit;

	protected bool mOnFinalTriggerUnit;

	public FUnitAIBear(FUnit owner)
		: base(owner)
	{
	}

	protected override void Init()
	{
		mOnFinalTriggerUnit = false;
		mInitialized = true;
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (mOnFinalTriggerUnit)
		{
			return;
		}
		bool arrived;
		if (mPath == null)
		{
			arrived = false;
		}
		else
		{
			int newIndex = 0;
			FUnit.FUnitMoveResult result = mOwner.MoveTowards(mPath, 1, mOwner.Speed, deltaTime, true, out newIndex);
			bool stillMoving = false;
			if (result == FUnit.FUnitMoveResult.Success)
			{
				Vector3 pos = mOwner.Position;
				Vector2 last = mPath.LastWaypoint.ToVector2();
				float dx = pos.x - last.x;
				float dy = pos.y - last.y;
				if (FUnitAIBase.cMoveTolerance < Mathf.Sqrt(dx * dx + dy * dy))
				{
					stillMoving = true;
				}
			}
			if (stillMoving)
			{
				arrived = false;
			}
			else
			{
				mPath = null;
				mOwner.SetIdle();
				arrived = true;
			}
		}
		if (mTriggerUnits.Count < 1)
		{
			return;
		}
		FUnit first = mTriggerUnits[0];
		if (!first.AI.WaitingOnReset)
		{
			Reset();
			return;
		}
		if (!mOnTriggerUnit)
		{
			Vector3 myPos = mOwner.Position;
			Vector3 firstPos = mTriggerUnits[0].Position;
			float ddx = myPos.x - firstPos.x;
			float ddy = myPos.y - firstPos.y;
			if (FUnitAIBase.cMoveTolerance < Mathf.Sqrt(ddx * ddx + ddy * ddy))
			{
				if (arrived)
				{
					mOwner.ShowEmote("Confusion", false);
					FGameUtil.DelayExecute(3f, Reset);
				}
				return;
			}
			mPath = null;
			mOwner.ShowEmote("Love", false);
			mOwner.PlayAnim("Eat");
			FApp.SoundManager.PlaySoundSet("Bear_Eating");
			if (mTriggerUnits[0].StartData != null)
			{
				if (mTriggerUnits[0].StartData.GetObjectProperty<bool>("FinalTrigger"))
				{
					mOnFinalTriggerUnit = true;
					mOwner.World.TryTriggerReveal(mOwner, "RunHomeBabyBoy");
				}
				mOnTriggerUnit = true;
			}
		}
		else if (mTriggerUnits.Count > 1)
		{
			mTriggerUnits.RemoveAt(0);
			ChargeNextTarget();
		}
		else if (!(mTriggerUnits[0].TilePosition == mOwner.TilePosition))
		{
			Reset();
		}
	}

	protected override void Reset()
	{
		mOwner.World.SpawnEffect("LeafPoof", mOwner.Position, 200);
		mTriggerUnits.Clear();
		base.Reset();
	}

	public override bool CollidedWithUnit(FUnit other)
	{
		if (!other.IsTag("Attackable") || other.IsTag("BearImpassable"))
		{
			return true;
		}
		Vector2 forward = mOwner.Forward();
		other.OnAttacked(mOwner, new Vector2(forward.x * 2f, forward.y * 2f), true);
		FApp.SoundManager.PlaySoundSet("Bear_Swipe");
		return false;
	}

	protected void DoCharge(Vector2 direction, int distance)
	{
		if (!GetHasMoveAction())
		{
			mOwner.SetFacing(FUnit.GetFacingFromDir(direction));
			FPoint2 endPoint = mOwner.TilePosition;
			mOwner.CanPush(mOwner, direction, distance, out endPoint);
			mPath = new FPath();
			mPath.AddWaypoint(mOwner.TilePosition);
			mPath.AddWaypoint(endPoint);
			mOwner.PlayAnim("Walk");
		}
	}

	protected void ExitComplete(FUnit unit)
	{
		mOwner.World.RemoveUnit(unit);
		mOwner.World.SpawnEffect("LeafPoof", mOwner.Position, 200);
	}

	protected void ChargeNextTarget()
	{
		if (mTriggerUnits.Count != 0)
		{
			mOwner.ShowEmote("Surprise", false);
			mOnTriggerUnit = false;
			FUnit target = mTriggerUnits[0];
			Vector3 targetPos = target.Position;
			Vector3 myPos = mOwner.Position;
			DoCharge(new Vector2(targetPos.x - myPos.x, targetPos.y - myPos.y), mTriggerUnits[0].TilePosition.ManhattanDistanceTo(mOwner.TilePosition));
		}
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (m.SenderID < 0)
		{
			return false;
		}
		FUnit sender = mOwner.World.GetUnitByID(m.SenderID);
		if (sender == null)
		{
			return false;
		}
		string message = m.Message;
		if (message == "Attacked")
		{
			mOwner.World.SpawnFlyOffText(FLoc.GetString("Immune"), mOwner.Position, new Vector2(30f, 0f));
			mOwner.ShowEmote("Anger", false);
			mOwner.FacePosition(sender.TilePosition);
			return true;
		}
		if (message == "Trigger")
		{
			if (!mOnFinalTriggerUnit)
			{
				mTriggerUnits.Add(sender);
				if (mTriggerUnits.Count == 1)
				{
					ChargeNextTarget();
				}
			}
			return true;
		}
		if (message == "RunHomeBabyBoy")
		{
			if (mPatrolPoints.Count > 1)
			{
				DoMoveToPoint(mPatrolPoints[1], 0, false, false, true, string.Empty, -1f, null, ExitComplete);
				return true;
			}
			return false;
		}
		return false;
	}
}
