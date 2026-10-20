using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitActionPushObject @ 0x17ddbac..0x17dde2c, Ghidra vaddr).
public class FUnitActionPushObject : FUnitActionWork
{
	protected int mDistance;

	public FUnitActionPushObject(int newID, FUnit owner, FUnit targetUnit)
		: base(newID, owner, targetUnit)
	{
		mTargetUnit = targetUnit;
		int distance = targetUnit.StartData.GetObjectProperty<int>("PushDistance");
		if (distance < 2)
		{
			distance = 1;
		}
		mDistance = distance;
	}

	public override void DoWork()
	{
		Vector3 targetPos = mTargetUnit.GO.transform.position;
		Vector3 myPos = mOwner.GO.transform.position;
		Vector2 toTarget = new Vector2(targetPos.x - myPos.x, targetPos.y - myPos.y);
		mOwner.SetFacing(FUnit.GetFacingFromDir(toTarget));
		if (mDistance < 1)
		{
			SetState(States.Fail);
			return;
		}
		mTargetUnit.SetFacing(FUnit.GetFacingFromDir(toTarget));
		if (!mOwner.CanPush(mTargetUnit, mOwner.GetTileDirectionToUnit(mTargetUnit), 1))
		{
			if (mTargetUnit.ProtoUnit.ResetOnDeath)
			{
				mTargetUnit.Die();
			}
			else
			{
				mTargetUnit.Push(mOwner.GetDirFromFacing(), mDistance, true, false, true);
			}
		}
		else
		{
			mTargetUnit.Push(mOwner.GetDirFromFacing(), mDistance, true, false, true);
		}
		mOwner.PlayAnim("Push");
		SetState(States.Success);
	}
}
