// Rewritten from libil2cpp.so 1.0.605 (FAnimDefManager @ 0x1606574..0x16083d0, Ghidra vaddr).
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class FAnimDefManager
{
	protected static Dictionary<string, int> mAnimTypeIDs = new Dictionary<string, int>();

	protected static Dictionary<int, string> mAnimReverseTypeIDs = new Dictionary<int, string>();

	protected static Dictionary<string, FAnimDef> mAnimDefs = new Dictionary<string, FAnimDef>();

	public const string cXmlFilename = "Data/AnimDefs";

	public static string mCurrentXmlData;

	public static byte[] mXmlHash;

	public static byte[] mCanonicalXmlHash;

	public static string FullFilename => Application.dataPath + "/Resources/Data/AnimDefs.xml";

	static FAnimDefManager()
	{
		string[] names = Enum.GetNames(typeof(AnimTypes));
		for (int i = 0; i < names.Length; i++)
		{
			GetOrCreateAnimTypeID(names[i]);
		}
	}

	protected static void Clear()
	{
		mAnimTypeIDs.Clear();
		mAnimReverseTypeIDs.Clear();
		mAnimDefs.Clear();
	}

	public static bool Init(bool canonicalFileChanged = false)
	{
		if (!canonicalFileChanged && mXmlHash != null && FCoreUtil.CompareByteArrays(mCanonicalXmlHash, mXmlHash))
		{
			FLog.UnityLog("FAnimDefManager.Init -- already have canonical data loaded");
			return true;
		}
		XmlElement xmlElement = FUtil.LoadXmlAsset("Data/AnimDefs", out mCanonicalXmlHash);
		if (xmlElement != null)
		{
			FLog.UnityLog("Data/AnimDefs canonical hash: " + FCoreUtil.HexString(mCanonicalXmlHash));
			mXmlHash = mCanonicalXmlHash;
			mCurrentXmlData = null;
			LoadXml(xmlElement);
			return true;
		}
		return false;
	}

	public static bool LoadXml(XmlElement root)
	{
		Clear();
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				FAnimDef fAnimDef = new FAnimDef();
				if (fAnimDef.LoadXml(childNode as XmlElement))
				{
					mAnimDefs.Add(fAnimDef.Name, fAnimDef);
				}
			}
		}
		return true;
	}

	public static FAnimDef GetAnimDef(string name)
	{
		FAnimDef value = null;
		if (name != null && !mAnimDefs.TryGetValue(name, out value))
		{
			value = null;
		}
		return value;
	}

	public static int GetAnimTypeID(string name)
	{
		int value = -1;
		if (name != null && name.Length != 0 && !mAnimTypeIDs.TryGetValue(name, out value))
		{
			value = -1;
		}
		return value;
	}

	public static int GetOrCreateAnimTypeID(string name)
	{
		if (name == null || name.Length == 0)
		{
			return -1;
		}
		int value = -1;
		if (mAnimTypeIDs.TryGetValue(name, out value))
		{
			return value;
		}
		int count = mAnimTypeIDs.Count;
		mAnimTypeIDs.Add(name, count);
		mAnimReverseTypeIDs.Add(count, name);
		return count;
	}

	public static string GetAnimTypeByID(int typeID)
	{
		string value = string.Empty;
		if (!mAnimReverseTypeIDs.TryGetValue(typeID, out value))
		{
			value = string.Empty;
		}
		return value;
	}

	public static void SaveData(FBinWriter writer)
	{
		writer.BeginChunk("AnimDefs.xml");
		string text = mCurrentXmlData ?? FUtil.LoadTextAsset("Data/AnimDefs");
		writer.Write(mXmlHash, true);
		writer.Write(text);
		writer.EndChunk("AnimDefs.xml");
	}

	public static void LoadData(FBinReader reader)
	{
		if (!reader.BeginChunk("AnimDefs.xml", true))
		{
			throw new System.IO.IOException();
		}
		byte[] array = reader.ReadByteArray();
		FLog.UnityLog("FAnimDefManager loaded hash: " + FCoreUtil.HexString(mXmlHash) + ", savegame hash: " + FCoreUtil.HexString(array));
		if (!FCoreUtil.CompareByteArrays(array, mXmlHash))
		{
			FLog.UnityLog("FAnimDefManager: load data from savegame.");
			mXmlHash = array;
			mCurrentXmlData = reader.ReadString();
			XmlElement xmlElement = FCoreUtil.LoadXmlString(mCurrentXmlData);
			if (xmlElement == null)
			{
				throw new System.IO.IOException();
			}
			LoadXml(xmlElement);
			if (!reader.EndChunk("AnimDefs.xml", true))
			{
				throw new System.IO.IOException();
			}
		}
		else
		{
			FLog.UnityLog("FAnimDefManager: no need to load data.");
			reader.SkipRestOfChunk("AnimDefs.xml", true);
		}
	}

	public static void RevertToCanonicalData()
	{
		Init();
	}
}
