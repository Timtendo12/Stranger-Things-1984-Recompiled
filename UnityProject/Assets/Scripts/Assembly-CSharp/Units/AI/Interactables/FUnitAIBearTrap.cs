// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBearTrap @ 0x1730728..0x1730960, Ghidra vaddr).
public class FUnitAIBearTrap : FUnitAIBase
{
	public FUnitAIBearTrap(FUnit owner)
		: base(owner)
	{
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (m.SenderID >= 0)
		{
			FUnit sender = mOwner.World.GetUnitByID(m.SenderID);
			if (sender != null && (m.Message == "Attacked" || m.Message == "Interact"))
			{
				mOwner.Obstructs = false;
				mOwner.Interactable = false;
				mOwner.SetIdle("Idle_Closed", true, true);
				mOwner.PlayAnim("Open", false);
				RegisterInteraction();
				return true;
			}
		}
		return false;
	}

	protected override void Reset()
	{
		mOwner.SetIdle("Idle_Locked", true, true);
		mOwner.Obstructs = true;
		mOwner.Interactable = true;
		base.Reset();
	}
}
