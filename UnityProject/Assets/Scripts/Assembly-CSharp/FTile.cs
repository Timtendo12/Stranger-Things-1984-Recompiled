using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FTile @ 0x171740c..0x1719c24, Ghidra vaddr).
public class FTile
{
	public const int cSoftImpassableThreshold = 100;

	public const int cFXSortOrder = 200;

	public const int cPlayerSortOrder = 100;

	public const int cFriendlyUnitSortOrder = 75;

	public const int cEnemyUnitSortOrder = 50;

	public const int cEmittedUnitSortOrder = -50;

	protected List<FUnit> mUnits = new List<FUnit>();

	protected List<int> mUnitsNotAffectingPathingCost = new List<int>();

	protected GameObject mWorldGO;

	protected GameObject mGO;

	protected int mAdditionalPathingCost;

	protected int mSortingOrder = -1;

	public FPoint2 Position { get; protected set; }

	public FProtoTile ProtoTile { get; protected set; }

	public bool TerrainPassable { get; protected set; }

	public bool Hole { get; protected set; }

	public List<FUnit> Units => mUnits;

	public int AdditionalPathingCost => mAdditionalPathingCost;

	public FTile()
	{
		mGO = null;
		TerrainPassable = false;
		Position = FPoint2.invalid;
		mUnits.Clear();
	}

	public FTile(FProtoTile proto, int x, int y)
	{
		ProtoTile = proto;
		if (ProtoTile != null)
		{
			// the shipped code copies the adjacent Passable/Hole bytes of the proto tile in one 16-bit load
			TerrainPassable = ProtoTile.Passable;
			Hole = ProtoTile.Hole;
		}
		Position = new FPoint2(x, y);
		mUnits.Clear();
	}

	public void Create(GameObject worldGO)
	{
		if (ProtoTile != null)
		{
			mWorldGO = worldGO;
			SyncBoxCollider();
		}
		else
		{
			FLog.Error("FTile::Create called on tile " + Position.ToString() + " with invalid proto tile or invalid sprite.");
		}
	}

	protected void SyncBoxCollider()
	{
		if (!TerrainPassable)
		{
			if (mGO == null)
			{
				mGO = new GameObject(string.Format("{0}, {1}", Position.x, Position.y));
				mGO.isStatic = true;
				mGO.transform.parent = mWorldGO.transform;
				mGO.transform.position = new Vector3(Position.x, Position.y, 0.1f);
				mGO.layer = LayerMask.NameToLayer("Tile");
			}
			mGO.GetOrCreateComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
		}
		else if (mGO != null)
		{
			Object.Destroy(mGO);
		}
	}

	public void AddCosmeticTile(FProtoTile proto)
	{
		if (TerrainPassable && !proto.Passable)
		{
			TerrainPassable = false;
			SyncBoxCollider();
		}
		if (!Hole && proto.Hole)
		{
			Hole = true;
		}
	}

	public bool AddUnit(FUnit unit)
	{
		if (mUnits.Contains(unit))
		{
			return false;
		}
		mUnits.Add(unit);
		UpdatePathingCost();
		for (int i = 0; i < mUnits.Count; i++)
		{
			FUnit fUnit = mUnits[i];
			FUnitMessage fUnitMessage = new FUnitMessage();
			fUnitMessage.Message = "UnitAddedToTile";
			fUnitMessage.RecipientID = fUnit.ID;
			fUnitMessage.SenderID = unit.ID;
			fUnit.ProcessMessage(fUnitMessage);
			fUnit.SetSortingOrder(i + GetSortingOrderOffset(fUnit) + mSortingOrder + fUnit.ProtoUnit.SortingOrderAdjust);
		}
		if (unit.IsPlayer)
		{
			FApp.World.PlayerPositionChanged();
		}
		return true;
	}

	public void SendMessageToAllUnits(string message, int senderID = -1)
	{
		for (int i = 0; i < mUnits.Count; i++)
		{
			FUnit fUnit = mUnits[i];
			FUnitMessage fUnitMessage = new FUnitMessage();
			fUnitMessage.Message = message;
			fUnitMessage.SenderID = senderID;
			fUnitMessage.RecipientID = mUnits[i].ID;
			fUnit.ProcessMessage(fUnitMessage);
		}
	}

	public void AddPathingCost(FUnit unit)
	{
		mAdditionalPathingCost += unit.ProtoUnit.AdditionalPathingCost;
	}

	public void SubtractPathingCost(FUnit unit)
	{
		mAdditionalPathingCost -= unit.ProtoUnit.AdditionalPathingCost;
	}

	public void SetUnitAffectsPathingCost(FUnit unit, bool affects)
	{
		if (!affects)
		{
			if (!mUnitsNotAffectingPathingCost.Contains(unit.ID))
			{
				mUnitsNotAffectingPathingCost.Add(unit.ID);
			}
		}
		else if (mUnitsNotAffectingPathingCost.Contains(unit.ID))
		{
			mUnitsNotAffectingPathingCost.Remove(unit.ID);
		}
		UpdatePathingCost();
	}

	protected void UpdatePathingCost()
	{
		mAdditionalPathingCost = 0;
		foreach (FUnit mUnit in mUnits)
		{
			if (!mUnitsNotAffectingPathingCost.Contains(mUnit.ID))
			{
				AddPathingCost(mUnit);
			}
		}
	}

	protected int GetSortingOrderOffset(FUnit unit)
	{
		if (unit.IsPlayer)
		{
			return 100;
		}
		if (unit.ProtoUnit.IsTag("FriendlyCharacter"))
		{
			return 75;
		}
		if (unit.ProtoUnit.IsTag("EnemyCharacter"))
		{
			return 50;
		}
		if (unit.ProtoUnit.IsTag("Emitted"))
		{
			return -50;
		}
		return 0;
	}

	public bool RemoveUnit(FUnit unit)
	{
		if (!mUnits.Contains(unit))
		{
			return false;
		}
		for (int i = 0; i < mUnits.Count; i++)
		{
			FUnit fUnit = mUnits[i];
			FUnitMessage fUnitMessage = new FUnitMessage();
			fUnitMessage.Message = "UnitRemovedFromTile";
			fUnitMessage.RecipientID = fUnit.ID;
			fUnitMessage.SenderID = unit.ID;
			fUnit.ProcessMessage(fUnitMessage);
		}
		mUnits.Remove(unit);
		UpdatePathingCost();
		return true;
	}

	public void SetTileColor(Color color, bool includeUnits = false, bool animate = false)
	{
		if (mGO == null)
		{
			return;
		}
		SpriteRenderer[] componentsInChildren = mGO.GetComponentsInChildren<SpriteRenderer>();
		if (componentsInChildren == null)
		{
			return;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			SpriteRenderer spriteRenderer = componentsInChildren[i];
			if (!(spriteRenderer == null))
			{
				if (!animate)
				{
					spriteRenderer.color = color;
					continue;
				}
				Go.killAllAbstractTweensWithTarget(spriteRenderer);
				GoTweenConfig config = new GoTweenConfig().colorProp("color", color).setUpdateType(GoUpdateType.FWorldUpdate);
				Go.to(spriteRenderer, 0.2f, config);
			}
		}
		if (!includeUnits)
		{
			return;
		}
		for (int j = 0; j < mUnits.Count; j++)
		{
			FUnit fUnit = mUnits[j];
			if (fUnit.GO == null)
			{
				break;
			}
			fUnit.SetColor(color, animate);
		}
	}

	public bool GetPassable()
	{
		return GetPassable(null, new List<int>(), null);
	}

	public bool GetPassable(string tagToIgnore)
	{
		return GetPassable(null, new List<int>(), tagToIgnore);
	}

	public bool GetPassable(FUnit unit, List<int> ignoreUnits)
	{
		return GetPassable(unit, ignoreUnits, null);
	}

	public bool GetPassable(FUnit unit, List<int> ignoreUnits, string tagToIgnore, bool includeNonObstructingUnits = false)
	{
		if (!TerrainPassable)
		{
			return false;
		}
		for (int i = 0; i < mUnits.Count; i++)
		{
			FUnit fUnit = mUnits[i];
			if (fUnit == unit || (!fUnit.Obstructs && !includeNonObstructingUnits))
			{
				continue;
			}
			if (ignoreUnits != null && ignoreUnits.Contains(fUnit.ID))
			{
				continue;
			}
			if (tagToIgnore.Valid() && fUnit.ProtoUnit.IsTag(tagToIgnore))
			{
				continue;
			}
			if (!fUnit.ProtoUnit.IsTag("IgnoreInPather"))
			{
				return false;
			}
		}
		return true;
	}

	public FUnit GetTopmostUnit(string tag)
	{
		if (mUnits == null || mUnits.Count == 0)
		{
			return null;
		}
		if (string.IsNullOrEmpty(tag))
		{
			return mUnits.Last();
		}
		for (int num = mUnits.Count - 1; num >= 0; num--)
		{
			if (mUnits[num].ProtoUnit.IsTag(tag))
			{
				return mUnits[num];
			}
		}
		return null;
	}
}
