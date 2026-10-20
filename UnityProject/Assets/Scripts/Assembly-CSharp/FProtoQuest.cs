using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FProtoQuest @ 0x17c3fdc..0x17c4d40, Ghidra vaddr).
public class FProtoQuest
{
	public int DBID { get; protected set; } = -1;

	public string Name { get; protected set; } = string.Empty;

	public string VisibleName => FLoc.GetString("Quest." + Name + ".Name", true);

	public string Details => FLoc.GetString("Quest." + Name + ".Details", true);

	public string CollectedDialogue => "Quest." + Name + ".Collected";

	public string LootTable { get; protected set; } = string.Empty;

	public string Icon { get; protected set; } = string.Empty;

	public List<QuestReminder> Reminders { get; protected set; }

	public List<FPlayerStatsRequirement> Requirements { get; protected set; }

	public bool LoadXml(XmlElement root)
	{
		Name = root.GetAttrib("Name", Name);
		DBID = root.GetAttrib("DBID", DBID);
		LootTable = root.GetAttrib("Reward", LootTable);
		Icon = root.GetAttrib("Icon", Icon);
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement element = (XmlElement)childNode;
			if (element.Name == "Require")
			{
				FPlayerStatsRequirement requirement = FRequirement<FPlayerStats>.CreateFromXml(element, FApp.PlayerStats) as FPlayerStatsRequirement;
				if (requirement != null)
				{
					if (Requirements == null)
					{
						Requirements = new List<FPlayerStatsRequirement>();
					}
					Requirements.Add(requirement);
				}
			}
			else if (element.Name == "Reminder")
			{
				QuestReminder reminder = new QuestReminder();
				if (reminder.LoadXML(element))
				{
					if (Reminders == null)
					{
						Reminders = new List<QuestReminder>();
					}
					Reminders.Add(reminder);
				}
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
