// Rewritten from libil2cpp.so 1.0.605 (FAnimDef @ 0x1606be4..0x16072b8, Ghidra vaddr).
using System.Collections.Generic;
using System.Xml;

public class FAnimDef
{
	protected string mName;

	protected bool mLogicDriven;

	protected Dictionary<int, FAnimSet> mAnimSets = new Dictionary<int, FAnimSet>();

	public string Name => mName;

	public bool LogicDriven => mLogicDriven;

	public Dictionary<int, FAnimSet> AnimSets => mAnimSets;

	public bool LoadXml(XmlElement root)
	{
		mName = root.GetAttrib("Name");
		if (mName == null)
		{
			return false;
		}
		mLogicDriven = root.GetAttrib("LogicDriven", false);
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				FAnimSet fAnimSet = new FAnimSet();
				if (fAnimSet.LoadXml(childNode as XmlElement))
				{
					mAnimSets.Add(fAnimSet.AnimType, fAnimSet);
				}
			}
		}
		return true;
	}

	public FAnimSet GetAnimSet(int animType)
	{
		if (!mAnimSets.TryGetValue(animType, out var value))
		{
			value = null;
		}
		return value;
	}

	public FAnimEntry SelectForType(int animType, FRandom random)
	{
		return GetAnimSet(animType)?.Select(random);
	}

	public bool HasAnim(int animType)
	{
		return GetAnimSet(animType)?.HasEntries() ?? false;
	}

	public static void SaveRef(FBinWriter writer, FAnimDef animDef)
	{
		if (animDef == null || string.IsNullOrEmpty(animDef.mName))
		{
			writer.Write(false);
			return;
		}
		writer.Write(true);
		writer.Write(animDef.mName);
	}

	public static FAnimDef LoadRef(FBinReader reader)
	{
		if (reader.ReadBool())
		{
			return FAnimDefManager.GetAnimDef(reader.ReadString());
		}
		return null;
	}

	public override string ToString()
	{
		return string.Format("[FAnimDef: Name={0}]", mName);
	}
}
