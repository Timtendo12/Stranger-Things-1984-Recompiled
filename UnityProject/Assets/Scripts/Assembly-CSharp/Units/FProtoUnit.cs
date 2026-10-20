using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FProtoUnit @ 0x0181af20..0x0182472c, Ghidra vaddr).
public class FProtoUnit
{
	protected HashSet<int> mTags = new HashSet<int>();

	public int ID { get; protected set; }

	public string Name { get; protected set; }

	public string DisplayName
	{
		get
		{
			if (!CharacterSkin.Valid())
			{
				return Name;
			}
			return CharacterSkin;
		}
	}

	public int BaseTagID { get; protected set; }

	public int ParentTagID { get; set; }

	public HashSet<int> Tags => mTags;

	public int ClassTagID { get; protected set; }

	public string PrefabName { get; protected set; }

	public string VisualName { get; protected set; }

	public string CharacterSkin { get; protected set; }

	public float Speed { get; protected set; }

	public string AIType { get; protected set; }

	public int LeashDistance { get; protected set; }

	public int Health { get; protected set; }

	public string Weapon { get; protected set; }

	public string LootTable { get; protected set; }

	public bool ResetOnDeath { get; protected set; }

	public bool Obstructs { get; protected set; }

	public bool ImmuneToKnockback { get; protected set; }

	public bool ImmuneToStun { get; protected set; }

	public int PurchasePrice { get; protected set; }

	public string EmitUnit { get; protected set; }

	public bool KeycardUnlockable { get; protected set; }

	public int AdditionalPathingCost { get; protected set; }

	public bool BlocksProjectiles { get; protected set; }

	public bool NoWorldTint { get; protected set; }

	public float MinPatrolPointPause { get; protected set; }

	public float MaxPatrolPointPause { get; protected set; }

	public float DamageTakenModifier { get; protected set; }

	public float SpeedModifier { get; protected set; }

	public string WalkAnimOverride { get; protected set; }

	public string[] IgnoreCollisionWithProtoUnits { get; protected set; }

	public int SortingOrderAdjust { get; protected set; }

	public List<string> AlternateWeapons { get; protected set; }

	public List<FPlayerStatsRequirement> SpawnRequirements { get; protected set; }

	public List<FPlayerStatsRequirement> InteractRequirements { get; protected set; }

	public List<FPlayerStatsRequirement> UnlockRequirements { get; protected set; }

	public FProtoUnit()
	{
		Init();
	}

	public void Init()
	{
		Name = "";
		ID = -1;
		ClassTagID = -1;
		BaseTagID = -1;
		ParentTagID = -1;
		mTags.Clear();
		PrefabName = "";
		VisualName = "";
		CharacterSkin = "";
		Speed = 0f;
		AIType = "";
		Weapon = "";
		LeashDistance = 7;
		Health = 0;
		LootTable = "";
		PurchasePrice = 0;
		EmitUnit = "";
		ResetOnDeath = false;
		Obstructs = true;
		ImmuneToKnockback = false;
		ImmuneToStun = false;
		KeycardUnlockable = false;
		AdditionalPathingCost = 0;
		BlocksProjectiles = true;
		NoWorldTint = false;
		MinPatrolPointPause = 0f;
		MaxPatrolPointPause = 0f;
		DamageTakenModifier = 1f;
		IgnoreCollisionWithProtoUnits = null;
		SpeedModifier = 1f;
		WalkAnimOverride = "";
		AlternateWeapons = null;
		SortingOrderAdjust = 0;
	}

	public FProtoUnit DeepCopy(int newID)
	{
		FProtoUnit fProtoUnit = new FProtoUnit();
		fProtoUnit.Name = Name;
		fProtoUnit.ID = newID;
		fProtoUnit.BaseTagID = BaseTagID;
		fProtoUnit.ClassTagID = ClassTagID;
		foreach (int mTag in mTags)
		{
			fProtoUnit.mTags.Add(mTag);
		}
		fProtoUnit.PrefabName = PrefabName;
		fProtoUnit.VisualName = VisualName;
		fProtoUnit.CharacterSkin = CharacterSkin;
		fProtoUnit.Speed = Speed;
		fProtoUnit.AIType = AIType;
		fProtoUnit.LeashDistance = LeashDistance;
		fProtoUnit.Health = Health;
		fProtoUnit.Weapon = Weapon;
		fProtoUnit.LootTable = LootTable;
		fProtoUnit.ResetOnDeath = ResetOnDeath;
		fProtoUnit.Obstructs = Obstructs;
		fProtoUnit.ImmuneToKnockback = ImmuneToKnockback;
		fProtoUnit.ImmuneToStun = ImmuneToStun;
		fProtoUnit.PurchasePrice = PurchasePrice;
		fProtoUnit.EmitUnit = EmitUnit;
		fProtoUnit.KeycardUnlockable = KeycardUnlockable;
		fProtoUnit.AdditionalPathingCost = AdditionalPathingCost;
		fProtoUnit.BlocksProjectiles = BlocksProjectiles;
		fProtoUnit.NoWorldTint = NoWorldTint;
		fProtoUnit.MinPatrolPointPause = MinPatrolPointPause;
		fProtoUnit.MaxPatrolPointPause = MaxPatrolPointPause;
		fProtoUnit.IgnoreCollisionWithProtoUnits = IgnoreCollisionWithProtoUnits;
		fProtoUnit.DamageTakenModifier = DamageTakenModifier;
		fProtoUnit.SpeedModifier = SpeedModifier;
		fProtoUnit.WalkAnimOverride = WalkAnimOverride;
		fProtoUnit.SortingOrderAdjust = SortingOrderAdjust;
		fProtoUnit.AlternateWeapons = AlternateWeapons;
		return fProtoUnit;
	}

	public bool LoadXml(XmlElement root, int newID)
	{
		ID = newID;
		Name = root.GetAttrib("Name", Name);
		if (string.IsNullOrEmpty(Name))
		{
			FLog.Error("FProtoUnit::LoadXml: Failed, no Name.");
			return false;
		}
		if (FTagManager.GetTagID(Name) >= 0)
		{
			FLog.Error("FProtoUnit::LoadXml: Error, BaseTag of '" + Name + "' is already defined.");
			return false;
		}
		BaseTagID = FTagManager.GetOrCreateTagID(Name);
		mTags.Add(BaseTagID);
		mTags.Add(FTagManager.AllTagID);
		PrefabName = root.GetAttrib("Prefab", PrefabName);
		if (string.IsNullOrEmpty(PrefabName))
		{
			FLog.Error("FProtoUnit::LoadXml: '" + Name + "' failed, no PrefabName.");
			return false;
		}
		VisualName = root.GetAttrib("Visual", VisualName);
		if (string.IsNullOrEmpty(VisualName))
		{
			VisualName = Name;
		}
		CharacterSkin = root.GetAttrib("CharacterSkin", CharacterSkin);
		Speed = root.GetAttrib("Speed", Speed);
		AIType = root.GetAttrib("AI", AIType);
		LeashDistance = root.GetAttrib("LeashDistance", LeashDistance);
		Health = root.GetAttrib("Health", Health);
		Weapon = root.GetAttrib("Weapon", Weapon);
		LootTable = root.GetAttrib("LootTable", "");
		ResetOnDeath = root.GetAttrib("ResetOnDeath", ResetOnDeath);
		Obstructs = root.GetAttrib("Obstructs", Obstructs);
		ImmuneToKnockback = root.GetAttrib("ImmuneToKnockback", ImmuneToKnockback);
		ImmuneToStun = root.GetAttrib("ImmuneToStun", ImmuneToStun);
		PurchasePrice = root.GetAttrib("PurchasePrice", PurchasePrice);
		EmitUnit = root.GetAttrib("EmitUnit", EmitUnit);
		KeycardUnlockable = root.GetAttrib("KeycardUnlockable", KeycardUnlockable);
		AdditionalPathingCost = root.GetAttrib("AdditionalPathingCost", AdditionalPathingCost);
		if (PurchasePrice > 0)
		{
			AdditionalPathingCost += 10;
		}
		BlocksProjectiles = root.GetAttrib("BlocksProjectiles", BlocksProjectiles);
		NoWorldTint = root.GetAttrib("NoWorldTint", NoWorldTint);
		MinPatrolPointPause = root.GetAttrib("MinPatrolPointPause", MinPatrolPointPause);
		MaxPatrolPointPause = root.GetAttrib("MaxPatrolPointPause", MaxPatrolPointPause);
		DamageTakenModifier = root.GetAttrib("DamageTakenModifier", DamageTakenModifier);
		SpeedModifier = root.GetAttrib("SpeedModifier", SpeedModifier);
		WalkAnimOverride = root.GetAttrib("WalkAnimOverride", WalkAnimOverride);
		string text = root.GetAttrib("IgnoreCollisionWithProtoUnit", "");
		if (text.Valid())
		{
			IgnoreCollisionWithProtoUnits = text.Split(',');
		}
		SortingOrderAdjust = root.GetAttrib("SortingOrderAdjust", SortingOrderAdjust);
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement xmlElement = (XmlElement)childNode;
			if (xmlElement.Name == "Tag")
			{
				ParseTagNode(xmlElement);
			}
			else if (xmlElement.Name == "SpawnRequire")
			{
				if (FRequirement<FPlayerStats>.CreateFromXml(xmlElement, FApp.PlayerStats) is FPlayerStatsRequirement item)
				{
					if (SpawnRequirements == null)
					{
						SpawnRequirements = new List<FPlayerStatsRequirement>();
					}
					SpawnRequirements.Add(item);
				}
			}
			else if (xmlElement.Name == "InteractRequire")
			{
				if (FRequirement<FPlayerStats>.CreateFromXml(xmlElement, FApp.PlayerStats) is FPlayerStatsRequirement item2)
				{
					if (InteractRequirements == null)
					{
						InteractRequirements = new List<FPlayerStatsRequirement>();
					}
					InteractRequirements.Add(item2);
				}
			}
			else if (xmlElement.Name == "UnlockRequire")
			{
				if (FRequirement<FPlayerStats>.CreateFromXml(xmlElement, FApp.PlayerStats) is FPlayerStatsRequirement item3)
				{
					if (UnlockRequirements == null)
					{
						UnlockRequirements = new List<FPlayerStatsRequirement>();
					}
					UnlockRequirements.Add(item3);
				}
			}
			else if (xmlElement.Name == "AlternateWeapon")
			{
				FProtoWeapon protoWeapon = FApp.ProtoWeaponManager.GetProtoWeapon(xmlElement.GetAttribute("Name"));
				if (protoWeapon != null)
				{
					if (AlternateWeapons == null)
					{
						AlternateWeapons = new List<string>();
					}
					if (!AlternateWeapons.Contains(protoWeapon.Name))
					{
						AlternateWeapons.Add(protoWeapon.Name);
					}
				}
			}
		}
		return true;
	}

	protected void ParseTagNode(XmlElement root)
	{
		int orCreateTagID = FTagManager.GetOrCreateTagID(root.GetAttrib("Name", ""));
		if (!root.GetAttrib("Remove", false))
		{
			mTags.Add(orCreateTagID);
		}
		else
		{
			mTags.Remove(orCreateTagID);
		}
		if (root.HasAttribute("ClassTag") && root.GetAttrib("ClassTag", false))
		{
			ClassTagID = orCreateTagID;
		}
	}

	public bool IsTag(int id)
	{
		return mTags.Contains(id);
	}

	public bool IsTag(string tag)
	{
		return IsTag(FTagManager.GetTagID(tag));
	}

	public bool HasAllTags(IEnumerable<string> tags)
	{
		foreach (string tag in tags)
		{
			if (!IsTag(tag))
			{
				return false;
			}
		}
		return true;
	}

	public bool HasAnyTag(IEnumerable<string> tags)
	{
		foreach (string tag in tags)
		{
			if (IsTag(tag))
			{
				return true;
			}
		}
		return false;
	}

	public void PostLoadFixup()
	{
		// Empty in the shipped build.
	}

	public FProtoWeapon GetProtoWeapon()
	{
		string name = Weapon;
		if (AlternateWeapons != null)
		{
			foreach (string alternateWeapon in AlternateWeapons)
			{
				FProtoWeapon protoWeapon = FApp.ProtoWeaponManager.GetProtoWeapon(alternateWeapon);
				if (protoWeapon != null && protoWeapon.CheckRequirements())
				{
					name = alternateWeapon;
				}
			}
		}
		return FApp.ProtoWeaponManager.GetProtoWeapon(name);
	}

	public bool CheckSpawnRequirements()
	{
		return CheckRequirements(SpawnRequirements);
	}

	public bool CheckInteractRequirements()
	{
		return CheckRequirements(InteractRequirements);
	}

	private static bool CheckRequirements(List<FPlayerStatsRequirement> requirements)
	{
		if (FApp.PlayerStats == null)
		{
			return false;
		}
		if (requirements == null || requirements.Count < 1)
		{
			return true;
		}
		for (int i = 0; i < requirements.Count; i++)
		{
			if (!requirements[i].Evaluate(FApp.PlayerStats))
			{
				return false;
			}
		}
		return true;
	}

	public string GetInteractRequirementsRequiredCharacter()
	{
		if (InteractRequirements != null)
		{
			for (int i = 0; i < InteractRequirements.Count; i++)
			{
				if (!string.IsNullOrEmpty(InteractRequirements[i].RequiredCharacter))
				{
					return InteractRequirements[i].RequiredCharacter;
				}
			}
		}
		return null;
	}

	public bool CheckUnlockRequirements()
	{
		return CheckRequirements(UnlockRequirements);
	}

	public void SetWeapon(string name)
	{
		Weapon = name;
	}

	public void SetDamageTakenModifier(float dmgTaken)
	{
		DamageTakenModifier = dmgTaken;
	}

	public void SetSpeedModifier(float speed)
	{
		SpeedModifier = speed;
	}

	public void SetWalkAnimOverride(string anim)
	{
		WalkAnimOverride = anim;
	}
}
