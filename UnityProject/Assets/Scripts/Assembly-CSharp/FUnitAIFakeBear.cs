using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIFakeBear @ 0x174a868..0x174aa6c, Ghidra vaddr).
public class FUnitAIFakeBear : FUnitAIEnemy
{
	public FUnitAIFakeBear(FUnit owner)
		: base(owner)
	{
		mShouldAggro = false;
	}

	protected override void Init()
	{
		base.Init();
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.SenderID >= 0)
		{
			FUnit sender = mOwner.World.GetUnitByID(m.SenderID);
			if (sender == null)
			{
				return false;
			}
			if (m.Message == "Attacked")
			{
				mOwner.World.SpawnFlyOffText(FLoc.GetString("Immune"), mOwner.Position, new Vector2(30f, 0f));
				mOwner.ShowEmote("Anger", false);
				mOwner.FacePosition(sender.TilePosition);
				return true;
			}
		}
		return false;
	}
}
