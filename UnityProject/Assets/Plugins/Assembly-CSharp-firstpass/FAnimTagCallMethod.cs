using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagCallMethod @ 0x1609524..0x1609eb4, Ghidra vaddr).
public class FAnimTagCallMethod : FAnimTag
{
	protected string mMethodName;

	protected object[] mParams;

	public string MethodName => mMethodName;

	public object[] Params => mParams;

	public FAnimTagCallMethod()
		: base(AnimTagTypes.CallMethod)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		if (!base.LoadXml(root))
		{
			return false;
		}
		mMethodName = root.GetAttrib("Name");
		if (string.IsNullOrEmpty(mMethodName))
		{
			return false;
		}
		List<object> list = new List<object>();
		string attrib = root.GetAttrib("StringParam");
		if (attrib != null)
		{
			list.Add(attrib);
		}
		float result = 0f;
		if (root.GetAttrib("FloatParam", ref result))
		{
			list.Add(result);
		}
		int result2 = 0;
		if (root.GetAttrib("IntParam", ref result2))
		{
			list.Add(result2);
		}
		bool result3 = false;
		if (root.GetAttrib("BoolParam", ref result3))
		{
			list.Add(result3);
		}
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement xmlElement = (XmlElement)childNode;
			if (xmlElement.Name == "StringParam")
			{
				list.Add(xmlElement.GetAttrib("Value"));
			}
			else if (xmlElement.Name == "FloatParam")
			{
				list.Add(xmlElement.GetAttrib("Value", 0f));
			}
			else if (xmlElement.Name == "IntParam")
			{
				list.Add(xmlElement.GetAttrib("Value", 0));
			}
			else if (xmlElement.Name == "BoolParam")
			{
				list.Add(xmlElement.GetAttrib("Value", false));
			}
		}
		if (list.Count < 1)
		{
			mParams = null;
		}
		else
		{
			mParams = list.ToArray();
		}
		return true;
	}
}
