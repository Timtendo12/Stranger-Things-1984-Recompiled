// Rewritten from libil2cpp.so 1.0.605 (FBikeContestant @ 0x1767b94..0x1767c18, Ghidra vaddr).
public class FBikeContestant
{
	public enum ContestantStates
	{
		IDLE = 0,
		STUNNED = 1,
		CHARGING = 2,
		FIRING = 3
	}

	public ContestantStates State;

	public FUnit Unit { get; set; }

	public float MoveSpeed { get; set; }

	public float LastLaneCheck { get; set; }

	public int CurrentLane { get; set; }

	public float ShakeUntilTime { get; set; }

	public bool InputEnabled
	{
		get
		{
			return Unit != null && Unit.InputEnabled;
		}
		set
		{
			if (Unit != null)
			{
				Unit.EnableInput(value);
			}
		}
	}
}
