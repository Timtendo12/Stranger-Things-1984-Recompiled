using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FEffect @ 0x16fc228..0x16fc678, Ghidra vaddr).
public class FEffect
{
	protected static Dictionary<string, ConstructorInfo> mEffectConstructors;

	static FEffect()
	{
		mEffectConstructors = new Dictionary<string, ConstructorInfo>();
	}

	public static FEffect AllocateForXmlNode(XmlElement root)
	{
		ConstructorInfo value = null;
		string name = root.Name;
		if (name == null)
		{
			FLog.UnityError("Missing effect type");
			return null;
		}
		if (!mEffectConstructors.TryGetValue(name, out value))
		{
			FLog.UnityError("No constructor registered for type: " + name + ". Add to static FEffect constructor or register somewhere at startup.");
			return null;
		}
		return value.Invoke(null) as FEffect;
	}

	public static void AddConstructorInfo(string name, Type type)
	{
		ConstructorInfo constructor = type.GetConstructor(new Type[0]);
		if (constructor == null)
		{
			FLog.UnityError("Did not find parameterless constructor for: " + type.Name);
		}
		else
		{
			mEffectConstructors.Add(name, constructor);
		}
	}

	public static FEffect CreateFromXmlNode(XmlElement root)
	{
		FEffect fEffect = AllocateForXmlNode(root);
		if (fEffect != null && !fEffect.LoadXml(root))
		{
			fEffect = null;
		}
		return fEffect;
	}

	public virtual bool Apply()
	{
		return true;
	}

	public virtual bool LoadXml(XmlElement root)
	{
		return true;
	}
}
