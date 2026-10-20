using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FResourceManager @ 0x17c773c..0x17cb5f4, Ghidra vaddr).
public class FResourceManager
{
	protected List<FResourceType> mResources;

	protected Dictionary<string, FResourceType> mResourcesDictionary;

	protected int mNextID;

	protected string mFilename;

	protected Dictionary<int, Fix64> mBoostedResources;

	protected int mBloomID = -1;

	public string mCurrentXmlData;

	public byte[] mXmlHash;

	public byte[] mCanonicalXmlHash;

	public int NumberResources => mResources.Count;

	public List<FResourceType> ResourceTypes => mResources;

	public int BloomID => mBloomID;

	public int XPHumanID { get; protected set; }

	public static FResourceManager CreateFromXmlFile(string filename)
	{
		FResourceManager fResourceManager = new FResourceManager();
		fResourceManager.LoadXml(filename);
		return fResourceManager;
	}

	protected void Clear()
	{
		mResources = null;
		mResourcesDictionary = null;
		mBoostedResources = null;
		mNextID = 0;
		mBloomID = -1;
	}

	public bool LoadXml(string filename)
	{
		if (mXmlHash != null && FCoreUtil.CompareByteArrays(mCanonicalXmlHash, mXmlHash))
		{
			FLog.UnityLog("FResourceManager.LoadXml -- already have canonical data loaded");
			return true;
		}
		mFilename = filename;
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename, out mCanonicalXmlHash);
		if (xmlElement == null)
		{
			FLog.UnityError("Failed to parse Resources file.");
			return false;
		}
		FLog.UnityLog(mFilename + " canonical hash: " + FCoreUtil.HexString(mCanonicalXmlHash));
		mXmlHash = mCanonicalXmlHash;
		return LoadXml(xmlElement);
	}

	public bool LoadXml(XmlElement root)
	{
		Clear();
		mResources = new List<FResourceType>();
		mResourcesDictionary = new Dictionary<string, FResourceType>();
		mBoostedResources = new Dictionary<int, Fix64>();
		mNextID = 0;
		bool result = true;
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			FResourceType fResourceType = new FResourceType();
			fResourceType.ID = mNextID;
			if (fResourceType.LoadXml(childNode as XmlElement))
			{
				mResources.Add(fResourceType);
				mResourcesDictionary.Add(fResourceType.Name, fResourceType);
				mNextID++;
			}
			else
			{
				FLog.UnityError("Error loading Resource name '" + fResourceType.Name + "'.");
				result = false;
			}
		}
		mBloomID = GetID("Bloom");
		XPHumanID = GetID("XPHuman");
		return result;
	}

	public FResourceType GetType(string name)
	{
		FResourceType value;
		if (!mResourcesDictionary.TryGetValue(name, out value))
		{
			value = null;
		}
		return value;
	}

	public FResourceType GetType(int id)
	{
		if (id >= 0 && id < mResources.Count)
		{
			return mResources[id];
		}
		return null;
	}

	public int GetID(string name)
	{
		FResourceType value;
		if (!mResourcesDictionary.TryGetValue(name, out value))
		{
			return -1;
		}
		return value.ID;
	}

	public void ResetBoosters()
	{
		mBoostedResources.Clear();
	}

	public bool IsBoosted(string name)
	{
		return mBoostedResources.ContainsKey(GetID(name));
	}

	public Fix64 BoostAmount(string name)
	{
		return BoostAmount(GetID(name));
	}

	public Fix64 BoostAmount(int id)
	{
		Fix64 value;
		if (!mBoostedResources.TryGetValue(id, out value))
		{
			return Fix64.Zero;
		}
		return value;
	}
}
