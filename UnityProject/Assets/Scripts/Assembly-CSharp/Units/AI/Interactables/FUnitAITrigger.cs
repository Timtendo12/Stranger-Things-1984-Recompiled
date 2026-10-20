using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAITrigger @ 0x1740a5c..0x174fe90, Ghidra vaddr).
public class FUnitAITrigger : FUnitAIBase
{
	protected bool mOn = true;

	protected bool mTransitioning;

	protected string[] mTriggerUnits;

	protected string[] mTriggerMessages;

	protected int mTriggerMessageIndex;

	public bool On => mOn;

	public FUnitAITrigger(FUnit owner)
		: base(owner)
	{
	}

	public override void Update(float deltaTime)
	{
		if (!mInitialized)
		{
			Init();
		}
		CheckActions(deltaTime);
		CheckResetTimer(deltaTime);
	}

	protected override void Init()
	{
		mTriggerUnits = mOwner.StartData.GetTriggerUnits();
		mTriggerMessages = mOwner.StartData.GetTriggerMessages();
		bool startOff = mOwner.StartData.GetObjectProperty<bool>("StartOff");
		bool on = !startOff;
		string saved = mOwner.GetSavedUnitInfo("TriggerOn");
		if (!DoesReset && saved.Valid())
		{
			on = bool.Parse(saved);
		}
		if (mOn != on)
		{
			mOwner.PlayAnim(mOn ? "TurnOff" : "TurnOn", false, 100, true);
			mOn = on;
		}
		if (mOwner.HasSavedUnitInfo("TriggerIndex"))
		{
			mTriggerMessageIndex = mOwner.GetSavedUnitInfo<int>("TriggerIndex");
		}
		base.Init();
	}

	public void DoTrigger(bool animate = true)
	{
		if (mTransitioning)
		{
			return;
		}
		RegisterInteraction();
		int? startFrame = (animate ? ((int?)null) : ((int?)100));
		if (!mOwner.PlayAnim(mOn ? "TurnOff" : "TurnOn", false, startFrame, true))
		{
			return;
		}
		mTransitioning = true;
		if (animate)
		{
			float length = mOwner.GetCurrentAnimLength();
			FGameUtil.DelayExecute(length, DoTriggerUnits);
			FWorld.Player.EnableInput(false);
			FGameUtil.DelayExecute(Mathf.Min(length, 0.6f), delegate
			{
				FWorld.Player.EnableInput(true);
			});
		}
		else
		{
			DoTriggerUnits();
		}
	}

	protected int GetNextTriggerIndex()
	{
		int index = mTriggerMessageIndex;
		mTriggerMessageIndex++;
		if (mTriggerMessages.Length <= mTriggerMessageIndex)
		{
			mTriggerMessageIndex = 0;
		}
		mOwner.SetSavedUnitInfo("TriggerIndex", (object)mTriggerMessageIndex);
		return index;
	}

	protected bool TriggerUnits(string triggerMessageOverride = null)
	{
		if (mTriggerUnits == null || mTriggerUnits.Length == 0 || mTriggerMessages == null || mTriggerMessages.Length == 0)
		{
			return false;
		}
		if (triggerMessageOverride == null)
		{
			triggerMessageOverride = mTriggerMessages[GetNextTriggerIndex()];
		}
		for (int i = 0; i < mTriggerUnits.Length; i++)
		{
			FUnit[] units = mOwner.World.GetUnitsByName(mTriggerUnits[i]);
			for (int j = 0; j < units.Length; j++)
			{
				FUnit unit = units[j];
				if (unit != null)
				{
					FUnitMessage message = new FUnitMessage();
					message.SenderID = mOwner.ID;
					message.RecipientID = unit.ID;
					message.Message = triggerMessageOverride;
					if (!mInitialized)
					{
						message.Data = "NoReveal";
					}
					unit.ProcessMessage(message);
				}
			}
		}
		return true;
	}

	protected void DoTriggerUnits()
	{
		if (TriggerUnits())
		{
			mTransitioning = false;
			mOn = !mOn;
			mOwner.SetSavedUnitInfo("TriggerOn", mOn.ToString());
		}
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
			if (sender != null && sender.IsPlayer)
			{
				DoTrigger();
				return true;
			}
		}
		return false;
	}
}
