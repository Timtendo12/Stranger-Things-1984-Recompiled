using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FNameGenerator @ 0x178f06c..0x178faf8, Ghidra vaddr).
public class FNameGenerator
{
	protected List<string> mFirstNames = new List<string>();

	protected List<string> mLastNames = new List<string>();

	protected List<string> mWholeNames = new List<string>();

	protected string mFilename;

	public static FNameGenerator CreateFromXmlFile(string filename)
	{
		FNameGenerator fNameGenerator = new FNameGenerator();
		fNameGenerator.LoadXml(filename);
		return fNameGenerator;
	}

	public bool LoadXml(string filename)
	{
		mFilename = filename;
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename);
		if (xmlElement == null)
		{
			FLog.UnityError("Failed to parse Resources file.");
		}
		else
		{
			LoadXml(xmlElement);
		}
		return xmlElement != null;
	}

	public bool LoadXml(XmlElement root)
	{
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement xmlElement = childNode as XmlElement;
			if (childNode.Name == "WholeName")
			{
				string attrib = xmlElement.GetAttrib("Name", string.Empty);
				if (attrib.Valid())
				{
					mWholeNames.Add(attrib);
				}
			}
			else if (childNode.Name == "FirstName")
			{
				string attrib2 = xmlElement.GetAttrib("Name", string.Empty);
				if (attrib2.Valid())
				{
					mFirstNames.Add(attrib2);
				}
			}
			else if (childNode.Name == "LastName")
			{
				string attrib3 = xmlElement.GetAttrib("Name", string.Empty);
				if (attrib3.Valid())
				{
					mLastNames.Add(attrib3);
				}
			}
		}
		return true;
	}

	public string GetName()
	{
		return GetName(FRandom.Generic);
	}

	public string GetName(FRandom random)
	{
		// 0x178f9a4..0x178f9b8: random.RangeInt(0, 100) == 0 picks a whole name (rare); otherwise first + " " + last
		if (random.RangeInt(0, 100) == 0 && mWholeNames.Count > 0)
		{
			return mWholeNames[random.RangeIntExclusive(0, mWholeNames.Count)];
		}
		if (mFirstNames.Count < 1 || mLastNames.Count < 1)
		{
			if (mWholeNames.Count > 0)
			{
				return mWholeNames[random.RangeIntExclusive(0, mWholeNames.Count)];
			}
			return null;
		}
		return mFirstNames[random.RangeIntExclusive(0, mFirstNames.Count)] + " " + mLastNames[random.RangeIntExclusive(0, mLastNames.Count)];
	}
}
