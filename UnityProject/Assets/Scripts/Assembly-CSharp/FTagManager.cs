using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FTagManager @ 0x017d1a48..0x017d2554, Ghidra vaddr).
public class FTagManager
{
	protected static Dictionary<string, FTag> mTags = null;

	protected static int mNextTagID = 0;

	protected static int mAllTagID = -1;

	protected static int mNoneTagID = -1;

	public static int AllTagID => mAllTagID;

	public static int NoneTagID => mNoneTagID;

	public static void Init()
	{
		if (mTags != null)
		{
			FLog.Error("FTagManager::Init: Error, mTags is already allocated.");
			return;
		}
		mTags = new Dictionary<string, FTag>();
		mAllTagID = GetOrCreateTagID("All");
		mNoneTagID = GetOrCreateTagID("None");
	}

	public static bool AddTags(string filename)
	{
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename);
		if (xmlElement != null)
		{
			return LoadXml(xmlElement);
		}
		FLog.UnityError("Failed to parse Tags file.");
		return false;
	}

	public static bool LoadXml(XmlElement root)
	{
		bool result = true;
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			string attrib = (childNode as XmlElement).GetAttrib("Name", "");
			if (!string.IsNullOrEmpty(attrib))
			{
				if (GetTagID(attrib) < 0)
				{
					GetOrCreateTagID(attrib);
					continue;
				}
				FLog.Error("FTagManager::LoadXML: '" + attrib + "' already exists.");
				result = false;
			}
		}
		return result;
	}

	public static string GetTagName(int id)
	{
		foreach (KeyValuePair<string, FTag> mTag in mTags)
		{
			if (mTag.Value.ID == id)
			{
				return mTag.Value.Name;
			}
		}
		return null;
	}

	public static int GetTagID(string name)
	{
		if (name != null && mTags.TryGetValue(name, out var value))
		{
			return value.ID;
		}
		return -1;
	}

	public static int GetOrCreateTagID(string name)
	{
		if (!mTags.TryGetValue(name, out var value))
		{
			value.Name = name;
			value.ID = mNextTagID;
			mTags.Add(name, value);
			mNextTagID++;
		}
		return value.ID;
	}

	public static void Clear()
	{
		mTags = null;
		mNextTagID = 0;
		mAllTagID = -1;
		mNoneTagID = -1;
		Init();
	}
}
