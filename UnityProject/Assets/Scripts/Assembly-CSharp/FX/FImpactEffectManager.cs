using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FImpactEffectManager @ 0x1788984..0x1788f9c, Ghidra vaddr).
public class FImpactEffectManager
{
	protected static Dictionary<string, FImpactEffect> mImpactEffects = new Dictionary<string, FImpactEffect>();

	public static bool Init()
	{
		XmlElement xmlElement = FUtil.LoadXmlAsset("data/ImpactEffects");
		if (xmlElement == null)
		{
			FLog.UnityError("Failed to load/parse impact effects");
		}
		else
		{
			LoadXml(xmlElement);
		}
		return xmlElement != null;
	}

	public static bool LoadXml(XmlElement root)
	{
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				FImpactEffect fImpactEffect = new FImpactEffect();
				fImpactEffect.LoadXml(childNode as XmlElement);
				mImpactEffects.Add(fImpactEffect.Name, fImpactEffect);
			}
		}
		return true;
	}

	public static FImpactEffect GetImpactEffect(string name)
	{
		FImpactEffect value = null;
		if (name == null || !mImpactEffects.TryGetValue(name, out value))
		{
			value = null;
		}
		return value;
	}
}
