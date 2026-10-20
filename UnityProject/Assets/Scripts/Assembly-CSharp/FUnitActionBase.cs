// Rewritten from libil2cpp.so 1.0.605 (FUnitActionBase @ 0x17287cc..0x17288f0, Ghidra vaddr).
public class FUnitActionBase
{
	protected FUnit mOwner;

	public int ID { get; protected set; }

	public FUnit Owner => mOwner;

	public bool IsFailed => GetIsFailed();

	public bool IsSuccessful => GetIsSuccessful();

	public bool IsFinished => GetIsFinished();

	public bool DoesInterrupt { get; set; }

	public virtual int TargetID => -1;

	public virtual FPoint2 TargetTile => FPoint2.invalid;

	public virtual bool IsAttack => false;

	public virtual bool IsMove => false;

	public virtual bool IsIdle => false;

	public FUnitActionBase(int newID, FUnit owner)
	{
		mOwner = owner;
		ID = newID;
	}

	public virtual bool GetIsFailed()
	{
		return false;
	}

	public virtual bool GetIsSuccessful()
	{
		return false;
	}

	public virtual bool GetIsFinished()
	{
		return false;
	}

	public virtual bool Start()
	{
		return false;
	}

	public virtual void Stop()
	{
	}

	public virtual void Update(float deltaTime)
	{
	}

	public virtual bool CollidedWithUnit(FUnit other)
	{
		return false;
	}
}
