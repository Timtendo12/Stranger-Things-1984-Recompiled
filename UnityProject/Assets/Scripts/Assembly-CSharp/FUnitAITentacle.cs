// Rewritten from libil2cpp.so 1.0.605 (FUnitAITentacle @ 0x174f98c..0x174fc10, Ghidra vaddr).
public class FUnitAITentacle : FUnitAIBase
{
	public FUnitAITentacle(FUnit owner)
		: base(owner)
	{
	}

	protected override void Init()
	{
		base.Init();
		if (mOwner.GetSavedUnitInfo<bool>("DeadFoLife"))
		{
			SetDeadStuff();
		}
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		bool handled = base.ProcessMessage(m);
		if (!handled && m.SenderID >= 0 && m.Message == "DeadFoLife")
		{
			SetDeadStuff();
			mOwner.PlayAnim("Death", false);
			mOwner.AI.RegisterInteraction(true);
			mOwner.SetSavedUnitInfo("DeadFoLife", (object)true);
		}
		return handled;
	}

	protected void SetDeadStuff()
	{
		mOwner.Obstructs = false;
		mOwner.Interactable = false;
		mOwner.SetIdle("Idle_Dead", true, true);
	}
}
