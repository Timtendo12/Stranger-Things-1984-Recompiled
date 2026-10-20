// Rewritten from libil2cpp.so 1.0.605 (FUnitAICosmetic @ 0x1744e18..0x1744f2c, Ghidra vaddr).
public class FUnitAICosmetic : FUnitAIBase
{
	protected float mTimer;

	protected bool mFirstUpdate;

	public FUnitAICosmetic(FUnit owner)
		: base(owner)
	{
	}

	public override void Update(float deltaTime)
	{
		if (mTimer <= 0f)
		{
			mTimer = FRandom.Generic.RangeFloat(2f, 20f);
			if (mFirstUpdate)
			{
				mFirstUpdate = false;
			}
		}
		else
		{
			mTimer -= deltaTime;
		}
	}
}
