// Rewritten from libil2cpp.so 1.0.605 (FUnitAIHelperArrow @ 0x174aa6c..0x174ab88, Ghidra vaddr).
public class FUnitAIHelperArrow : FUnitAIBase
{
	public FUnitAIHelperArrow(FUnit owner)
		: base(owner)
	{
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.Message == "UnitAddedToTile" && m.SenderID >= 0 && m.SenderID != mOwner.ID)
		{
			mOwner.World.RemoveUnit(mOwner);
			return true;
		}
		return false;
	}
}
