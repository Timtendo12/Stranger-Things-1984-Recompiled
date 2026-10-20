// Rewritten from libil2cpp.so 1.0.605 (FUnitAIWillPipe @ 0x1751d5c..0x1752330, Ghidra vaddr).
public class FUnitAIWillPipe : FUnitAIBase
{
	protected int mNumHitsNeeded = 1;

	public FUnitAIWillPipe(FUnit owner)
		: base(owner)
	{
	}

	public void DoOpen(FUnit triggeredBy, bool force = false)
	{
		string leadsTo = mOwner.StartData.GetObjectProperty<string>("LeadsTo");
		if (!leadsTo.Valid())
		{
			return;
		}
		string[] parts = leadsTo.Split('.');
		if (parts.Length < 2)
		{
			FLog.Warning("Will pipe has LeadsTo property of: " + leadsTo + " which is not in the correct format. Please contact your service provider.");
			return;
		}
		FPlayerStartParams startParams = new FPlayerStartParams();
		startParams.EntryPoint = parts[1];
		startParams.PlaySound = "Will_Pipe_Out";
		mOwner.World.TryWorldTransition(parts[0], null, startParams);
		triggeredBy.AI.StopActions();
		triggeredBy.PlayAnim("Duck", false);
		FApp.SoundManager.PlaySoundSet("Will_Pipe_In");
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.Message != "Interact")
		{
			return false;
		}
		if (!mOwner.ProtoUnit.CheckInteractRequirements() && !FWorld.Player.TrySwitchToRequiredCharacter(mOwner.ProtoUnit))
		{
			return false;
		}
		if (m.SenderID >= 0)
		{
			FUnit sender = mOwner.World.GetUnitByID(m.SenderID);
			if (sender != null && sender.IsPlayer)
			{
				DoOpen(sender);
			}
		}
		return true;
	}
}
