using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIEmitReflector @ 0x1745ee4..0x17466c4, Ghidra vaddr).
public class FUnitAIEmitReflector : FUnitAIBase
{
	protected FUnitEmitter mEmitterOwner;

	protected int mTotalTurns;

	protected FUnit mReflectedUnit;

	public FUnitAIEmitReflector(FUnit owner)
		: base(owner)
	{
		mEmitterOwner = owner as FUnitEmitter;
		if (mEmitterOwner != null)
		{
			mEmitterOwner.TurnOff();
		}
		if (mOwner.HasSavedUnitInfo("MirrorTurns"))
		{
			DoTurn(mOwner.GetSavedUnitInfo<int>("MirrorTurns"));
		}
	}

	protected override void Init()
	{
		base.Init();
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (mReflectedUnit != null)
		{
			if (!(mReflectedUnit.GO == null))
			{
				if (!mReflectedUnit.AliveAndWell)
				{
					mReflectedUnit = null;
				}
			}
			else
			{
				mReflectedUnit = null;
			}
			if (mReflectedUnit != null)
			{
				return;
			}
		}
		if (mEmitterOwner.On)
		{
			mEmitterOwner.RemoveEmitUnits();
			mEmitterOwner.TurnOff();
		}
	}

	protected void DoTurn(int turnValue)
	{
		mTotalTurns += turnValue;
		int steps = Mathf.Abs(turnValue);
		int facing = (int)mOwner.Facing;
		int direction = ((turnValue >= 1) ? 1 : (-1));
		for (int i = 0; i < steps; i++)
		{
			facing += direction;
			if (facing < 1)
			{
				facing = 4;
			}
			if (facing > 4)
			{
				facing = 1;
			}
		}
		mOwner.SetFacing((FAnimEntry.AnimFacing)facing);
		mOwner.SetSavedUnitInfo("MirrorTurns", (object)mTotalTurns);
		TryEmit();
	}

	protected void TryEmit()
	{
		if (mEmitterOwner.On)
		{
			mEmitterOwner.RemoveEmitUnits();
			mEmitterOwner.TurnOff();
		}
		if (mReflectedUnit == null || !mReflectedUnit.AliveAndWell)
		{
			return;
		}
		Vector2 reflectedForward = mReflectedUnit.Forward();
		Vector2 d1 = -reflectedForward - mOwner.Forward();
		Vector2 emitDir;
		if (d1.x * d1.x + d1.y * d1.y >= 1E-10f)
		{
			Vector2 d2 = -reflectedForward - mOwner.Right();
			if (d2.x * d2.x + d2.y * d2.y >= 1E-10f)
			{
				return;
			}
			emitDir = mOwner.Forward();
		}
		else
		{
			emitDir = mOwner.Right();
		}
		mEmitterOwner.FacingOverride = FUnit.GetFacingFromDir(emitDir);
		mEmitterOwner.DoEmit();
		mEmitterOwner.TurnOn();
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (mEmitterOwner == null)
		{
			return false;
		}
		FUnit unit = mOwner.World.GetUnitByID(m.SenderID);
		if (unit == null || m.SenderID == mOwner.ID)
		{
			FTile tile = mOwner.World.GetTile(mOwner.TilePosition);
			for (int i = 0; i < tile.Units.Count; i++)
			{
				FUnit other = tile.Units[i];
				if (other.ID != mOwner.ID)
				{
					unit = other;
				}
			}
			if (unit == null)
			{
				return false;
			}
		}
		string message = m.Message;
		if (message == "UnitAddedToTile")
		{
			if (mReflectedUnit == null && unit.IsTag("Emitted") && !mEmitterOwner.On && unit.AI is FUnitAILaserSegment && unit != mEmitterOwner.EmittedUnit)
			{
				mReflectedUnit = unit;
				mEmitterOwner.EmitUnit = unit.ProtoUnit.Name;
				TryEmit();
			}
		}
		else if (message == "UnitRemovedFromTile")
		{
			if (unit == mReflectedUnit)
			{
				mReflectedUnit = null;
			}
		}
		else
		{
			int turns;
			if (!int.TryParse(m.Message, out turns))
			{
				return false;
			}
			DoTurn(turns);
		}
		return true;
	}
}
