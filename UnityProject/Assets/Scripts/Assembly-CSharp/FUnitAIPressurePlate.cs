using System.Collections.Generic;
using System.Linq;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIPressurePlate @ 0x174c95c..0x174e070, Ghidra vaddr).
public class FUnitAIPressurePlate : FUnitAITrigger
{
	protected enum States
	{
		UNPRESSED = 0,
		PRESSED = 1,
		REALPRESSED = 2
	}

	protected States mState;

	protected string mFakeTriggerEnterMessage = "FakeOpen";

	protected string mFakeTriggerExitMessage = "Close";

	protected string mSequenceName = string.Empty;

	protected int mSequenceIndex = -1;

	protected List<FUnitAIPressurePlate> mSequenceMates = new List<FUnitAIPressurePlate>();

	public bool PressedInOrder;

	public bool Pressed
	{
		get
		{
			if (mState != States.PRESSED)
			{
				return mState == States.REALPRESSED;
			}
			return true;
		}
	}

	public string SequenceName => mSequenceName;

	public int SequenceIndex => mSequenceIndex;

	public FUnitAIPressurePlate(FUnit owner)
		: base(owner)
	{
		string enter = mOwner.StartData.GetObjectProperty<string>("FakeTriggerEnterMessage");
		if (enter.Valid())
		{
			mFakeTriggerEnterMessage = enter;
		}
		string exit = mOwner.StartData.GetObjectProperty<string>("FakeTriggerExitMessage");
		if (exit.Valid())
		{
			mFakeTriggerExitMessage = exit;
		}
		string sequence = mOwner.StartData.GetObjectProperty<string>("SequenceName");
		if (sequence.Valid())
		{
			mSequenceName = sequence;
			mSequenceIndex = mOwner.StartData.GetObjectProperty<int>("SequenceIndex");
		}
	}

	protected override void Init()
	{
		base.Init();
		if (!mSequenceName.Valid())
		{
			return;
		}
		List<FUnit> units = mOwner.Room.Units;
		for (int i = 0; i < units.Count; i++)
		{
			if (units[i].AI is FUnitAIPressurePlate plate && !(plate.mSequenceName != mSequenceName))
			{
				mSequenceMates.Add(plate);
			}
		}
		mSequenceMates = mSequenceMates.OrderBy((FUnitAIPressurePlate p) => p.mSequenceIndex).ToList();
	}

	public void DoPress(FUnit presser, bool pressed, bool fromKnockback = false)
	{
		if (mSequenceName.Valid())
		{
			if (pressed)
			{
				DoFakePress(false);
				PressedInOrder = GetSequenceOnTrack();
				CheckSequence();
			}
		}
		else if (!pressed)
		{
			if (presser.IsTag("Pushable"))
			{
				DoRealUnPress();
			}
			DoUnPress();
		}
		else if (!presser.IsTag("Pushable") || !fromKnockback)
		{
			DoFakePress();
		}
		else
		{
			presser.AnimController.EndCurrentLoop();
			DoRealPress();
		}
	}

	protected void DoFakePress(bool sendMessage = true)
	{
		mOwner.PlayAnim("Triggered", false);
		mState = States.PRESSED;
		if (sendMessage)
		{
			TriggerUnits(mFakeTriggerEnterMessage);
		}
	}

	protected void DoUnPress(bool sendMessage = true)
	{
		mOwner.SetIdle();
		FApp.SoundManager.PlaySoundSet("PressurePlateOff");
		mState = States.UNPRESSED;
		if (sendMessage)
		{
			TriggerUnits(mFakeTriggerExitMessage);
		}
	}

	protected void DoRealPress()
	{
		mOwner.PlayAnim("Triggered", false);
		TriggerUnits();
		mState = States.REALPRESSED;
	}

	protected void DoRealUnPress()
	{
		mOwner.SetIdle();
		FApp.SoundManager.PlaySoundSet("PressurePlateOff");
		mState = States.UNPRESSED;
		TriggerUnits();
	}

	protected bool GetSequenceOnTrack()
	{
		bool previousPressed = false;
		for (int i = 0; i < mSequenceMates.Count; i++)
		{
			FUnitAIPressurePlate mate = mSequenceMates[i];
			if (i != 0 && !previousPressed && mate.Pressed)
			{
				return false;
			}
			previousPressed = mate.Pressed;
		}
		return true;
	}

	protected bool GetSequenceComplete()
	{
		return mSequenceMates.All((FUnitAIPressurePlate p) => p.Pressed);
	}

	protected bool CheckSequence()
	{
		if (!GetSequenceComplete())
		{
			return false;
		}
		if (mSequenceMates.All((FUnitAIPressurePlate p) => p.PressedInOrder))
		{
			TriggerUnits();
			for (int i = 0; i < mSequenceMates.Count; i++)
			{
				mSequenceMates[i].mOwner.Interactable = false;
			}
			return true;
		}
		for (int j = 0; j < mSequenceMates.Count; j++)
		{
			mOwner.SendMessage(mSequenceMates[j].mOwner, "SequenceFailed");
		}
		return false;
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (!mOwner.Interactable)
		{
			return false;
		}
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.SenderID >= 0)
		{
			FUnit sender = mOwner.World.GetUnitByID(m.SenderID);
			if (sender != null)
			{
				string message = m.Message;
				if (message == "UnitAddedToTile")
				{
					if (sender.Obstructs)
					{
						if (mState != States.UNPRESSED)
						{
							return true;
						}
						DoPress(sender, true);
						return true;
					}
				}
				else if (message == "UnitRemovedFromTile")
				{
					if (sender.Obstructs && (!mSequenceName.Valid() || !CheckSequence()))
					{
						if (mState == States.UNPRESSED)
						{
							return true;
						}
						DoPress(sender, false);
						return true;
					}
				}
				else if (message == "KnockbackEndedOnTile")
				{
					if (sender.IsTag("Pushable"))
					{
						DoPress(sender, true, true);
					}
				}
				else if (message == "UnitOnTileStunned")
				{
					if (!mSequenceName.Valid())
					{
						if (mState == States.REALPRESSED)
						{
							return true;
						}
						if (mState == States.PRESSED && mTriggerMessages != null && mTriggerMessages.Length != 0 && mTriggerMessages[0] == mFakeTriggerEnterMessage)
						{
							return false;
						}
						DoRealPress();
						return true;
					}
				}
				else if (message == "UnitOnTileUnStunned")
				{
					if (!mSequenceName.Valid())
					{
						if (mState != States.UNPRESSED)
						{
							DoRealUnPress();
						}
						return true;
					}
				}
				else if (message == "SequenceFailed")
				{
					DoUnPress(false);
					PressedInOrder = false;
					return true;
				}
			}
		}
		return false;
	}
}
