using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAICollectible @ 0x1744368..0x1744e18, Ghidra vaddr).
public class FUnitAICollectible : FUnitAIBase
{
	public const float cCollectDist = 0.2f;

	protected bool mCollected;

	protected FUnit mCollectingUnit;

	protected bool mOnlyPlayerCollectable = true;

	public FUnitAICollectible(FUnit owner)
		: base(owner)
	{
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (mOwner.SpawnComplete && mCollectingUnit != null)
		{
			DoCollect(mCollectingUnit);
		}
	}

	public virtual void DoCollect(FUnit other)
	{
		if (mCollected || other == null)
		{
			return;
		}
		int price = mOwner.PurchasePrice;
		if (other.IsPlayer && price > 0)
		{
			if (FApp.PlayerStats.Coins < price)
			{
				mCollectingUnit = null;
				FApp.SoundManager.PlaySoundSet("UI.PurchaseFail");
				mOwner.World.SpawnFlyOffText(FLoc.GetString("CantAfford"), mOwner.Position, new Vector2(50f, 0f));
				return;
			}
			FApp.SoundManager.PlaySoundSet("UI.PurchaseSuccess");
			FApp.PlayerStats.Coins -= price;
			mOwner.ReleaseUI();
		}
		if (string.IsNullOrEmpty(mOwner.ProtoCollectibleName))
		{
			mOwner.TryEvaluateLootTable();
		}
		CompleteCollect();
	}

	protected void CompleteCollect()
	{
		string dialogueName = string.Empty;
		FApp.SoundManager.PlaySoundSet(mOwner.ProtoUnit.Name + "Pickup");
		mCollected = true;
		FApp.PlayerStats.AddCollectible(mOwner.ProtoCollectibleName);
		FProtoCollectible collectible = FApp.ProtoCollectibleManager.GetProtoCollectible(mOwner.ProtoCollectibleName);
		if (collectible != null)
		{
			mOwner.TryAwardDungeonItem();
			FProtoQuest quest = FApp.ProtoQuestManager.GetProtoQuestRequiresItemDBID(collectible.DBID);
			dialogueName = ((quest == null) ? (collectible.Name + "_Collect") : quest.CollectedDialogue);
		}
		System.Action showDialogue = delegate
		{
			mOwner.World.TryShowDialogue(string.Empty, dialogueName);
		};
		if (mOwner.IsTag("DoFanfare") || mOwner.StartData.DoFanfare)
		{
			mOwner.World.TryFanfareLootReveal(mOwner.ProtoUnit.Name, showDialogue);
			FApp.World.RemoveUnit(mOwner);
		}
		else
		{
			showDialogue();
			mOwner.SendMessage(mCollectingUnit, "Collected");
		}
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.Message == "UnitAddedToTile" && m.SenderID >= 0)
		{
			if (m.SenderID == mOwner.ID)
			{
				FTile tile = mOwner.World.GetTile(mOwner.TilePosition);
				if (tile == null)
				{
					return false;
				}
				for (int i = 0; i < tile.Units.Count; i++)
				{
					FUnit unit = tile.Units[i];
					if (CanUnitCollect(unit))
					{
						mCollectingUnit = unit;
						break;
					}
				}
				return false;
			}
			FUnit sender = mOwner.World.GetUnitByID(m.SenderID);
			if (CanUnitCollect(sender))
			{
				mCollectingUnit = sender;
				return true;
			}
		}
		return false;
	}

	protected bool CanUnitCollect(FUnit unit)
	{
		if (unit != null && mOwner != unit && unit.AliveAndWell)
		{
			if (mOnlyPlayerCollectable)
			{
				return unit.IsPlayer;
			}
			if (unit.IsPlayer)
			{
				return true;
			}
			return unit.IsTag("EnemyCharacter");
		}
		return false;
	}
}
