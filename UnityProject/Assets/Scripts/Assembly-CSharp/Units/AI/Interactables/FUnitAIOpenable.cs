// Rewritten from libil2cpp.so 1.0.605 (FUnitAIOpenable @ 0x174bc84..0x174c094, Ghidra vaddr).
public class FUnitAIOpenable : FUnitAIBase
{
	protected int mNumHitsNeeded = 1;

	protected bool mHasDroppedLoot;

	public FUnitAIOpenable(FUnit owner)
		: base(owner)
	{
		mNumHitsNeeded = owner.HP;
	}

	public override void Update(float deltaTime)
	{
	}

	public virtual void DoOpen(FUnit triggeredBy, bool force = false)
	{
		if (mOwner.IsPlayingAnim("Hit"))
		{
			return;
		}
		if (mNumHitsNeeded < 1 && !mOwner.IsTag("InputAfterLootDrop") && !force)
		{
			return;
		}
		mOwner.PlayAnim("Hit", false);
		mNumHitsNeeded--;
		if (mNumHitsNeeded > 0)
		{
			return;
		}
		if (mOwner.IsTag("DespawnOnOpen"))
		{
			mOwner.World.RemoveUnit(mOwner);
		}
		else
		{
			if (!mOwner.IsTag("ObstructsAfterOpen"))
			{
				mOwner.Obstructs = false;
			}
			mOwner.PlayAnim("Death");
		}
		if (!mHasDroppedLoot)
		{
			FGameUtil.DelayExecute(mOwner.GetCurrentAnimLength(), delegate
			{
				mOwner.DropLoot(true);
			});
			if (!mOwner.IsTag("InputAfterLootDrop"))
			{
				mOwner.Interactable = false;
			}
		}
		mHasDroppedLoot = true;
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.Message != "Attacked")
		{
			return false;
		}
		if (m.SenderID >= 0)
		{
			FUnit sender = mOwner.World.GetUnitByID(m.SenderID);
			if (sender != null)
			{
				if (!sender.IsPlayer && !mOwner.IsTag("AICanOpen"))
				{
					return true;
				}
				DoOpen(sender);
			}
		}
		// 0x174bfc4: every path after the Attacked handling shares the tail "mov w0, #1; ret" (returns true).
		return true;
	}
}
