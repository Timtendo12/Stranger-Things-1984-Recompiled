using System.Collections.Generic;
using System.Linq;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBossDoor @ 0x173146c..0x17320bc, Ghidra vaddr).
public class FUnitAIBossDoor : FUnitAIBase
{
	protected int mUniqueTriggersNeeded;

	protected List<int> mTriggerUnits = new List<int>();

	protected bool mReadyToOpen;

	protected bool mOpened;

	protected List<string> mFallbackSwitchesToTrack = new List<string>();

	public FUnitAIBossDoor(FUnit owner)
		: base(owner)
	{
		mUniqueTriggersNeeded = mOwner.StartData.GetObjectProperty<int>("UniqueTriggersNeeded");
		string saved = mOwner.GetSavedUnitInfo("TriggerUnits");
		if (saved.Valid())
		{
			mTriggerUnits = saved.Split(',').Select((string s) => int.Parse(s)).ToList();
			SyncReadyToOpen();
		}
		string fallback = mOwner.StartData.GetObjectProperty<string>("FallbackSwitchesToTrack");
		if (fallback.Valid())
		{
			mFallbackSwitchesToTrack = fallback.Split(',').ToList();
			SyncReadyToOpen();
		}
	}

	protected override void Init()
	{
		mInitialized = true;
		mOwner.AddToTile(mOwner.TilePosition + new FPoint2(1, 0));
	}

	public override void Update(float deltaTime)
	{
		if (!(mOwner.World.CurrentRoom != mOwner.Room))
		{
			if (!mInitialized || !mReadyToOpen)
			{
				if (!mOpened && !mReadyToOpen)
				{
					SyncReadyToOpen();
				}
			}
			else
			{
				FGameUtil.DelayExecute(1f, delegate
				{
					mOwner.World.TryTriggerReveal(mOwner, "Trigger");
				});
				mReadyToOpen = false;
				mOpened = true;
			}
			if (!mInitialized)
			{
				mInitialized = true;
				Init();
			}
		}
	}

	protected void Triggered(int unitID)
	{
		if (!mOpened)
		{
			if (mTriggerUnits.Contains(unitID))
			{
				mTriggerUnits.Remove(unitID);
			}
			else
			{
				mTriggerUnits.Add(unitID);
			}
			mOwner.SetSavedUnitInfo("TriggerUnits", string.Join(",", mTriggerUnits.Select((int id) => id.ToString()).ToArray()));
			SyncReadyToOpen();
		}
		else
		{
			mOwner.OpenDoor(true);
		}
	}

	protected void SyncReadyToOpen()
	{
		int count = mTriggerUnits.Count;
		if (count > 0 && mUniqueTriggersNeeded <= count)
		{
			mReadyToOpen = true;
		}
		if (mFallbackSwitchesToTrack.Count <= 0)
		{
			return;
		}
		foreach (string name in mFallbackSwitchesToTrack)
		{
			FUnit unit = mOwner.World.GetUnitByName(name);
			FUnitAITrigger trigger = ((unit != null) ? (unit.AI as FUnitAITrigger) : null);
			if (trigger == null)
			{
				return;
			}
			string on = trigger.Owner.GetSavedUnitInfo("TriggerOn");
			if (!on.Valid() || bool.Parse(on))
			{
				return;
			}
		}
		mReadyToOpen = true;
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		bool isTrigger = m.Message == "Trigger";
		if (isTrigger)
		{
			Triggered(m.SenderID);
		}
		return isTrigger;
	}
}
