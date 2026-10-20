// Rewritten from libil2cpp.so 1.0.605 (FUnitAINPC @ 0x174bae8..0x174bc84, Ghidra vaddr).
public class FUnitAINPC : FUnitAIEnemy
{
	public FUnitAINPC(FUnit owner)
		: base(owner)
	{
		mShouldAggro = false;
	}

	protected override void AddIdleAction(float deltaTime)
	{
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (m.Message == "Interact")
		{
			FUnit unit = mOwner.World.GetUnitByID(m.SenderID);
			if (unit != null)
			{
				CollidedWithUnit(unit);
			}
		}
		return base.ProcessMessage(m);
	}

	public override bool CollidedWithUnit(FUnit other)
	{
		if (other == null)
		{
			return false;
		}
		StopActions();
		mOwner.FacePosition(other.TilePosition);
		mOwner.SetIdle();
		AddAction(new FUnitActionWait(mOwner.World.GetNextActionID(), mOwner, 1f));
		return false;
	}
}
