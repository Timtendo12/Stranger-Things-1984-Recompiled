using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FEffectList @ 0x16fc700..0x16fcd98, Ghidra vaddr).
public class FEffectList
{
	protected List<FEffect> mEffects = new List<FEffect>();

	public bool Apply()
	{
		foreach (FEffect mEffect in mEffects)
		{
			mEffect.Apply();
		}
		return true;
	}

	public int GetNumEffects()
	{
		return mEffects.Count;
	}

	public FEffect GetEffect(int index)
	{
		return mEffects[index];
	}

	public bool LoadXml(XmlElement root)
	{
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				FEffect fEffect = FEffect.CreateFromXmlNode(childNode as XmlElement);
				if (fEffect != null)
				{
					mEffects.Add(fEffect);
				}
			}
		}
		return true;
	}

	public static FEffectList CreateFromXml(XmlElement root)
	{
		FEffectList fEffectList = new FEffectList();
		fEffectList.LoadXml(root);
		return fEffectList;
	}
}
