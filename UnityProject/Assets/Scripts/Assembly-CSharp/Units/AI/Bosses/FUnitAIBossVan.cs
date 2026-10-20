// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBossVan @ 0x1742bc4..0x1742dc0, Ghidra vaddr).
public class FUnitAIBossVan : FUnitAIBase
{
	public int AnimIndex { get; set; }

	public FUnitAIBossVan(FUnit owner)
		: base(owner)
	{
		AnimIndex = 1;
	}

	public override string GetAnimName(string anim)
	{
		if (anim == "Walk")
		{
			anim = "Idle";
		}
		else if (anim != "Transition" && anim != "Idle" && anim != "Swerve" && anim != "Stun" && anim != "Hit")
		{
			return anim;
		}
		return anim + "0" + AnimIndex.ToString();
	}
}
