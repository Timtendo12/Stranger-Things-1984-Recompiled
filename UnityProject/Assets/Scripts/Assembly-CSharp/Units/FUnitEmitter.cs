// Rewritten from libil2cpp.so 1.0.605 (FUnitEmitter @ 0x17524ec..0x1752e20, Ghidra vaddr).
public class FUnitEmitter : FUnit
{
	protected bool mOn = true;

	protected FUnit mEmittedUnit;

	public FAnimEntry.AnimFacing FacingOverride;

	public bool On => mOn;

	public string EmitUnit { get; set; }

	public FUnit EmittedUnit => mEmittedUnit;

	public FAnimEntry.AnimFacing EmitFacing
	{
		get
		{
			if (FacingOverride == FAnimEntry.AnimFacing.NONE)
			{
				return Facing;
			}
			return FacingOverride;
		}
	}

	public FUnitEmitter(FRoom room, FProtoUnitStartData pusd, int newID)
		: base(room, pusd, newID)
	{
		EmitUnit = ProtoUnit.EmitUnit;
		if (pusd.GetObjectProperty<bool>("StartOff"))
		{
			mOn = false;
		}
		if (GetSavedUnitInfo("EmitterOn").Valid())
		{
			mOn = bool.Parse(GetSavedUnitInfo("EmitterOn"));
		}
	}

	protected override void SpawnCompleted()
	{
		base.SpawnCompleted();
		if (mOn)
		{
			TurnOn();
		}
		else
		{
			TurnOff();
		}
	}

	public bool DoEmit(FPoint2 offset = default(FPoint2))
	{
		FProtoUnit proto = FApp.ProtoUnitManager.GetProtoUnit(EmitUnit);
		if (proto == null)
		{
			return false;
		}
		FProtoUnitStartData pusd = new FProtoUnitStartData();
		pusd.Position = TilePosition + offset;
		pusd.ProtoUnit = EmitUnit;
		pusd.Facing = EmitFacing;
		int counter = 0;
		pusd.Name = Name + "_" + proto.Name + "_" + counter.ToString();
		int newID = -1;
		if (Room.SpawnUnit(pusd, out newID) == FRoom.SpawnUnitResult.Success && newID >= 0)
		{
			mEmittedUnit = World.GetUnitByID(newID);
			SendMessage(mEmittedUnit, "FirstBorn");
			return true;
		}
		return false;
	}

	public void RemoveEmitUnits()
	{
		if (mEmittedUnit != null)
		{
			World.RemoveUnit(mEmittedUnit);
			mEmittedUnit = null;
		}
	}

	public void TurnOn()
	{
		if (mEmittedUnit != null || DoEmit())
		{
			mOn = true;
			PlayAnim("Idle");
			SetSavedUnitInfo("EmitterOn", mOn.ToString());
			if (mEmittedUnit != null)
			{
				SendMessage(mEmittedUnit, "Show");
			}
		}
	}

	public void TurnOff()
	{
		mOn = false;
		PlayAnim("Idle_Off");
		SetSavedUnitInfo("EmitterOn", mOn.ToString());
		if (mEmittedUnit != null)
		{
			SendMessage(mEmittedUnit, "Hide");
		}
	}

	public override void Trigger(int unitID = -1)
	{
		if (mOn)
		{
			TurnOff();
		}
		else
		{
			TurnOn();
		}
	}
}
