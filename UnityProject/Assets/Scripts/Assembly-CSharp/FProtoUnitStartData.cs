using System;
using System.Collections.Generic;
using System.Xml.Serialization;

// Rewritten from libil2cpp.so 1.0.605 (FProtoUnitStartData @ 0x0181adf4..0x019eed00, Ghidra vaddr).
[Serializable]
[XmlInclude(typeof(FProtoDoorStartData))]
[XmlInclude(typeof(FProtoDroppedLootStartData))]
public class FProtoUnitStartData
{
	protected List<string> mPatrolPoints = new List<string>();

	public string ProtoUnit { get; set; }

	public FPoint2 Position { get; set; }

	public string Name { get; set; }

	public FAnimEntry.AnimFacing Facing { get; set; }

	public FProtoUnit GetProtoUnit
	{
		get
		{
			if (ProtoUnit.Valid() && FApp.ProtoUnitManager != null)
			{
				return FApp.ProtoUnitManager.GetProtoUnit(ProtoUnit, true);
			}
			return null;
		}
	}

	public string[] PropertyKeys { get; set; }

	public object[] PropertyValues { get; set; }

	public List<string> PatrolPoints => mPatrolPoints;

	public string ProtoCollectible { get; set; }

	public int PurchasePrice { get; set; }

	public string LootTableOverride { get; set; }

	public bool IsFanfareReveal { get; set; }

	public bool DoFanfare { get; set; }

	public string World { get; set; }

	public string Room { get; set; }

	public FProtoUnitStartData()
	{
		ProtoUnit = null;
		Name = "";
		Position = FPoint2.invalid;
		Facing = FAnimEntry.AnimFacing.NONE;
		mPatrolPoints.Clear();
		ProtoCollectible = "";
		PurchasePrice = 0;
		LootTableOverride = "";
		IsFanfareReveal = false;
		World = "";
		Room = "";
	}

	public T GetObjectProperty<T>(string propName)
	{
		object value = default(T);
		if (PropertyKeys != null && PropertyValues != null)
		{
			for (int i = 0; i < PropertyKeys.Length; i++)
			{
				if (PropertyKeys[i] == propName)
				{
					value = PropertyValues[i];
				}
			}
			if (!(value is T))
			{
				value = Convert.ChangeType(value, typeof(T));
			}
		}
		return (T)value;
	}

	public string[] GetTriggerUnits()
	{
		string text = GetObjectProperty<string>("TriggerUnit");
		if (!text.Valid())
		{
			return null;
		}
		return text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
	}

	public string[] GetTriggerMessages()
	{
		string text = GetObjectProperty<string>("TriggerMessage");
		if (!text.Valid())
		{
			return null;
		}
		return text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
	}

	public bool IsSecret()
	{
		if (!ProtoCollectible.Valid())
		{
			return false;
		}
		return FApp.ProtoCollectibleManager.GetProtoCollectible(ProtoCollectible) != null;
	}
}
