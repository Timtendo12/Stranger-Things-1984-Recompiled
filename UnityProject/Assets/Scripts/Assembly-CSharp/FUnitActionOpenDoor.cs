using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitActionOpenDoor @ 0x172b9ec..0x172bbfc, Ghidra vaddr).
public class FUnitActionOpenDoor : FUnitActionWork
{
	public FUnitActionOpenDoor(int newID, FUnit owner, FUnit targetUnit)
		: base(newID, owner, targetUnit)
	{
		mTargetUnit = targetUnit;
		mState = States.None;
	}

	public override void DoWork()
	{
		if (!(mTargetUnit.GO == null))
		{
			Vector3 targetPos = mTargetUnit.GO.transform.position;
			Vector3 myPos = mOwner.GO.transform.position;
			mOwner.SetFacing(FUnit.GetFacingFromDir(new Vector2(targetPos.x - myPos.x, targetPos.y - myPos.y)));
			if (mTargetUnit.ToggleDoor())
			{
				mState = States.Success;
				return;
			}
			FApp.SoundManager.PlaySoundSet("UIFail");
		}
		mState = States.Fail;
	}
}
