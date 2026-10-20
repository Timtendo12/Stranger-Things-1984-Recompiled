using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FResourceType @ 0x17ca2a8..0x17caaec, Ghidra vaddr).
public class FResourceType
{
	public int ID { get; set; }

	public string Name { get; set; }

	public string VisibleName { get; set; }

	public string Icon { get; set; }

	public string MediumIcon { get; set; }

	public string SmallIcon { get; set; }

	public Color FlyoffTextColor { get; set; }

	public bool IsXP { get; set; }

	public FVisualDefinition VisualDefinition { get; set; }

	public bool LoadXml(XmlElement root)
	{
		Name = root.GetAttribute("Name");
		if (string.IsNullOrEmpty(Name))
		{
			FLog.UnityError("Invalid Name specified for ResourceID=" + ID.ToString());
			return false;
		}
		VisibleName = FLoc.GetString("Resource." + Name + ".VisibleName", quiet: true);
		Icon = root.GetAttribute("Icon");
		if (!Icon.Contains(".png"))
		{
			Icon += ".png";
		}
		MediumIcon = root.GetAttribute("MediumIcon");
		if (!MediumIcon.Valid())
		{
			MediumIcon = Icon;
		}
		else if (!MediumIcon.Contains(".png"))
		{
			MediumIcon += ".png";
		}
		SmallIcon = root.GetAttribute("SmallIcon");
		if (!SmallIcon.Valid())
		{
			SmallIcon = Icon;
		}
		else if (!SmallIcon.Contains(".png"))
		{
			SmallIcon += ".png";
		}
		IsXP = root.GetAttrib("IsXP", false);
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				if (childNode.Name == "FlyoffTextColor")
				{
					FlyoffTextColor = FUtil.ParseXMLColorNode(childNode as XmlElement);
				}
				else if (childNode.Name == "Visual")
				{
					VisualDefinition = FVisualDefinition.CreateFromXmlNode(childNode as XmlElement);
				}
			}
		}
		return true;
	}
}
