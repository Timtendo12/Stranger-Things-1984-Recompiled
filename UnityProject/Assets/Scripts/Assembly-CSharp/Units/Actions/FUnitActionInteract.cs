using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitActionInteract @ 0x1729d28..0x1729ee0, Ghidra vaddr).
public class FUnitActionInteract : FUnitActionWork
{
	public FUnitActionInteract(int newID, FUnit owner, FUnit targetUnit)
		: base(newID, owner, targetUnit)
	{
	}

	public override void DoWork()
	{
		if (GetIsFinished())
		{
			return;
		}
		if (mTargetUnit.GO == null)
		{
			SetState(States.Fail);
			return;
		}
		Vector3 targetPos = mTargetUnit.GO.transform.position;
		Vector3 myPos = mOwner.GO.transform.position;
		mOwner.SetFacing(FUnit.GetFacingFromDir(new Vector2(targetPos.x - myPos.x, targetPos.y - myPos.y)));
		FUnitMessage message = new FUnitMessage();
		message.SenderID = mOwner.ID;
		message.RecipientID = mTargetUnit.ID;
		message.Message = "Interact";
		mTargetUnit.ProcessMessage(message);
		SetState(States.Success);
	}
}
