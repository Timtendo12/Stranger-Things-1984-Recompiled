using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FProtoWeapon @ 0x01824174..0x01825ddc, Ghidra vaddr).
public class FProtoWeapon
{
	public static string cDefaultImpactFX = "RockImpact";

	public string Name { get; protected set; }

	public string Visual { get; protected set; }

	public string PrefabName { get; protected set; }

	public float Cooldown { get; protected set; }

	public string Type { get; protected set; }

	public int Knockback { get; protected set; }

	public int Damage { get; protected set; }

	public int Range { get; protected set; }

	public int Speed { get; protected set; }

	public string Sound { get; protected set; }

	public bool SingleAxis { get; protected set; }

	public string AmmoResource { get; protected set; }

	public bool HasAmmoResource => !string.IsNullOrEmpty(AmmoResource);

	public string ImpactFX { get; protected set; }

	public bool CanTargetGround { get; protected set; }

	public string AttackAnimOverride { get; protected set; }

	public bool OnlyDamageTarget { get; protected set; }

	public int ChargeRange { get; protected set; }

	public string SwitchToWeaponAfterUse { get; protected set; }

	public float StunTime { get; protected set; }

	public float CritChance { get; protected set; }

	public int CritMultiplier { get; protected set; }

	public float AttackSpeedMultiplier { get; protected set; }

	public string IconOverride { get; protected set; }

	public List<FPlayerStatsRequirement> Requirements { get; protected set; }

	public FProtoWeapon()
	{
		Init();
	}

	public void Init()
	{
		Name = "";
		Visual = "";
		PrefabName = "";
		Cooldown = 0f;
		Type = "";
		Sound = "";
		Knockback = 0;
		Damage = 1;
		Range = 0;
		Speed = 0;
		SingleAxis = false;
		AmmoResource = "";
		ImpactFX = "";
		CanTargetGround = false;
		AttackAnimOverride = "";
		OnlyDamageTarget = false;
		ChargeRange = 0;
		StunTime = 0f;
		CritChance = 0f;
		CritMultiplier = 1;
		AttackSpeedMultiplier = 1f;
		SwitchToWeaponAfterUse = "";
		IconOverride = "";
	}

	public bool LoadXml(XmlElement root)
	{
		Name = root.GetAttrib("Name", Name);
		if (string.IsNullOrEmpty(Name))
		{
			FLog.UnityError("ProtoWeapon missing Name.");
			return false;
		}
		Visual = root.GetAttrib("Visual", Visual);
		if (!Visual.Valid())
		{
			Visual = Name;
		}
		PrefabName = root.GetAttrib("Prefab", PrefabName);
		if (string.IsNullOrEmpty(PrefabName))
		{
			FLog.UnityError("ProtoWeapon '" + Name + "' missing PrefabName.");
			return false;
		}
		Cooldown = root.GetAttrib("Cooldown", Cooldown);
		Type = root.GetAttrib("Type", Type);
		Knockback = root.GetAttrib("Knockback", Knockback);
		Damage = root.GetAttrib("Damage", Damage);
		Sound = root.GetAttrib("Sound", "");
		Range = root.GetAttrib("Range", 0);
		Speed = root.GetAttrib("Speed", 0);
		SingleAxis = root.GetAttrib("SingleAxis", false);
		AmmoResource = root.GetAttrib("AmmoResource", AmmoResource);
		ImpactFX = root.GetAttrib("ImpactFX", ImpactFX);
		CanTargetGround = root.GetAttrib("CanTargetGround", CanTargetGround);
		AttackAnimOverride = root.GetAttrib("AttackAnimOverride", AttackAnimOverride);
		OnlyDamageTarget = root.GetAttrib("OnlyDamageTarget", OnlyDamageTarget);
		ChargeRange = root.GetAttrib("ChargeRange", ChargeRange);
		SwitchToWeaponAfterUse = root.GetAttrib("SwitchToWeaponAfterUse", SwitchToWeaponAfterUse);
		StunTime = root.GetAttrib("StunTime", StunTime);
		CritChance = root.GetAttrib("CritChance", CritChance);
		CritMultiplier = root.GetAttrib("CritMultiplier", CritMultiplier);
		AttackSpeedMultiplier = root.GetAttrib("AttackSpeedMultiplier", AttackSpeedMultiplier);
		IconOverride = root.GetAttrib("IconOverride", IconOverride);
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element && ((XmlElement)childNode).Name == "Require" && FRequirement<FPlayerStats>.CreateFromXml((XmlElement)childNode, FApp.PlayerStats) is FPlayerStatsRequirement item)
			{
				if (Requirements == null)
				{
					Requirements = new List<FPlayerStatsRequirement>();
				}
				Requirements.Add(item);
			}
		}
		return true;
	}

	public bool CheckRequirements()
	{
		if (FApp.PlayerStats == null)
		{
			return false;
		}
		if (Requirements == null || Requirements.Count < 1)
		{
			return true;
		}
		for (int i = 0; i < Requirements.Count; i++)
		{
			if (!Requirements[i].Evaluate(FApp.PlayerStats))
			{
				return false;
			}
		}
		return true;
	}
}
