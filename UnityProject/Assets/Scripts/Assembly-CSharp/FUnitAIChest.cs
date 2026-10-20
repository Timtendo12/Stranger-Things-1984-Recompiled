// Rewritten from libil2cpp.so 1.0.605 (FUnitAIChest @ 0x1743e80..0x1744364, Ghidra vaddr).
public class FUnitAIChest : FUnitAIBase
{
	protected bool mHasDroppedLoot;

	public FUnitAIChest(FUnit owner)
		: base(owner)
	{
		mHasDroppedLoot = false;
		if (mOwner.HasSavedUnitInfo("ChestOpened"))
		{
			mHasDroppedLoot = mOwner.GetSavedUnitInfo<bool>("ChestOpened");
			mOwner.SetIdle("Idle_Opened", true, true);
		}
	}

	public override void Update(float deltaTime)
	{
	}

	public virtual void DoOpen(FUnit triggeredBy, bool force = false)
	{
		if (!mHasDroppedLoot && !mOwner.IsPlayingAnim("Open"))
		{
			mOwner.SetIdle("Idle_Opened", true, true);
			mOwner.PlayAnim("Open", false);
			FGameUtil.DelayExecute(mOwner.GetCurrentAnimLength(), DoLoot);
			mHasDroppedLoot = true;
			mOwner.SetSavedUnitInfo("ChestOpened", (object)true);
		}
	}

	protected void DoLoot()
	{
		System.Collections.Generic.List<FProtoDroppedLootStartData> datas = mOwner.GetLootStartDatas();
		for (int i = 0; i < datas.Count; i++)
		{
			FProtoDroppedLootStartData data = datas[i];
			if (data != null)
			{
				if (!mOwner.IsTag("DontDropLootOnPlayer"))
				{
					data.IsDroppedLoot = false;
					data.Position = FWorld.Player.TilePosition;
				}
				data.DoFanfare = true;
				mOwner.Room.AddUnit(data);
			}
		}
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.Message == "Interact" && mOwner.ProtoUnit.CheckInteractRequirements())
		{
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
		return false;
	}
}
