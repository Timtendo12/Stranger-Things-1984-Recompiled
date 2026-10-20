// Rewritten from libil2cpp.so 1.0.605 (FUnitAIDialogueSpace @ 0x1744f2c..0x1745154, Ghidra vaddr).
public class FUnitAIDialogueSpace : FUnitAIBase
{
	protected bool mTriggered;

	public FUnitAIDialogueSpace(FUnit owner)
		: base(owner)
	{
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (mTriggered)
		{
			DoDialogue();
		}
	}

	public virtual void DoDialogue()
	{
		mOwner.World.TryShowDialogue(mOwner.World.ProtoWorld.Name, mOwner.ProtoUnit.Name);
		mOwner.World.RemoveUnit(mOwner);
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.Message != "UnitAddedToTile" || m.SenderID < 0)
		{
			return false;
		}
		if (m.SenderID != FWorld.Player.ID)
		{
			return false;
		}
		if (!mOwner.ProtoUnit.CheckInteractRequirements() && !FWorld.Player.TrySwitchToRequiredCharacter(mOwner.ProtoUnit))
		{
			return false;
		}
		mTriggered = true;
		return true;
	}
}
