using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FDialogSet @ 0x17eb984..0x17ec6fc, Ghidra vaddr).
public class FDialogSet
{
	protected List<FDialogueLine> mLines;

	public string World { get; protected set; }

	public string Unit { get; protected set; }

	public List<FDialogueLine> Lines => mLines;

	public List<FPlayerStatsRequirement> Requirements { get; protected set; }

	public FDialogSet(string world, string unit)
	{
		mLines = new List<FDialogueLine>();
		World = world;
		Unit = unit;
	}

	public bool LoadXML(XmlElement root)
	{
		foreach (XmlNode node in root.ChildNodes)
		{
			if (node.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement element = (XmlElement)node;
			if (element.Name == "Dialogue")
			{
				string text = element.GetAttrib("Text", string.Empty);
				if (!FCoreUtil.Valid(text))
				{
					continue;
				}
				FDialogueLine line = new FDialogueLine();
				line.Line = text;
				line.Speaker = element.GetAttrib("Speaker", string.Empty);
				line.RunTagsFirst = element.GetAttrib("RunTagsFirst", false);
				line.Tags = new List<FAnimTag>();
				foreach (XmlNode child in element.ChildNodes)
				{
					if (child.NodeType == XmlNodeType.Element)
					{
						FAnimTag tag = FAnimTag.CreateFromXml(child as XmlElement);
						if (tag != null)
						{
							line.Tags.Add(tag);
						}
					}
				}
				mLines.Add(line);
			}
			else if (element.Name == "Require")
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
		}
		return root != null;
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
