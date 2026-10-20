using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAnalyticEvent @ 0x17e819c..0x17e8a08, Ghidra vaddr).
public class FAnalyticEvent
{
	public bool HasPosted;

	public string Progression1 { get; protected set; }

	public string Progression2 { get; protected set; }

	public string Progression3 { get; protected set; }

	public int Score { get; protected set; }

	public List<FPlayerStatsRequirement> Requirements { get; protected set; }

	public virtual void SendEvent()
	{
		// Empty in the shipped build (no analytics upload).
	}

	public virtual bool LoadXml(XmlElement root)
	{
		Progression1 = root.GetAttrib("Progression1", Progression1);
		if (string.IsNullOrEmpty(Progression1))
		{
			Debug.LogError("Missing AnalyticEvent Progression1");
			return false;
		}
		Progression2 = root.GetAttrib("Progression2", Progression2);
		Progression3 = root.GetAttrib("Progression3", Progression3);
		Score = root.GetAttrib("Score", Score);
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

	public void Init()
	{
		HasPosted = CheckRequirements();
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
