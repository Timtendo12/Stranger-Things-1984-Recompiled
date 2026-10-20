using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLazyPrefabPool @ 0x178b2b0..0x178c038, Ghidra vaddr).
public class FLazyPrefabPool
{
	protected class FPoolEntry
	{
		public GameObject Prefab;

		public bool ActuallyPool;

		public Stack<GameObject> Instances;

		public GameObject CreateInstance(FLazyPrefabPool pool)
		{
			GameObject instance = Object.Instantiate(Prefab);
			Object.DontDestroyOnLoad(instance);
			if (ActuallyPool)
			{
				FUtil.GetOrCreateComponent<FLazyPrefabPoolHelper>(instance);
				pool.AddUsedInstance(this, instance);
			}
			return instance;
		}

		public GameObject GetInstance(FLazyPrefabPool pool)
		{
			if (ActuallyPool && Instances.Count >= 1)
			{
				GameObject instance = Instances.Pop();
				instance.SetActive(true);
				instance.transform.SetParent(null);
				return instance;
			}
			return CreateInstance(pool);
		}

		public void ReturnInstance(FLazyPrefabPool pool, GameObject go)
		{
			go.SetActive(false);
			go.transform.SetParent(pool.mHolderObject.transform);
			Instances.Push(go);
		}
	}

	protected Dictionary<string, FPoolEntry> mEntries;

	protected Dictionary<GameObject, FPoolEntry> mUsedInstances = new Dictionary<GameObject, FPoolEntry>();

	protected string mFilename;

	protected GameObject mHolderObject;

	public GameObject HolderObject => mHolderObject;

	public static FLazyPrefabPool CreateFromXmlFile(string filename)
	{
		FLazyPrefabPool pool = new FLazyPrefabPool();
		pool.LoadXml(filename);
		return pool;
	}

	public bool LoadXml(string filename)
	{
		mFilename = filename;
		mHolderObject = new GameObject("PrefabPoolHolder");
		Object.DontDestroyOnLoad(mHolderObject);
		XmlElement root = FUtil.LoadXmlAsset(filename);
		if (root == null)
		{
			FLog.UnityError("Failed to parse LazyObjectPool file.");
		}
		else
		{
			LoadXml(root);
		}
		return root != null;
	}

	public bool LoadXml(XmlElement root)
	{
		mEntries = new Dictionary<string, FPoolEntry>();
		foreach (XmlNode node in root.ChildNodes)
		{
			if (node.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement element = (XmlElement)node;
			string name = element.GetAttribute("Name");
			if (string.IsNullOrEmpty(name))
			{
				continue;
			}
			FPoolEntry entry = new FPoolEntry();
			string prefabPath = element.GetAttribute("Prefab");
			if (string.IsNullOrEmpty(prefabPath))
			{
				continue;
			}
			entry.Prefab = Resources.Load(prefabPath) as GameObject;
			if (entry.Prefab == null)
			{
				continue;
			}
			int count = element.GetAttrib("Count", -1);
			if (count > 0)
			{
				entry.ActuallyPool = true;
				entry.Instances = new Stack<GameObject>();
				for (int i = 0; i < count; i++)
				{
					GameObject instance = entry.CreateInstance(this);
					instance.name = instance.name + "_" + i.ToString() + "_";
					entry.ReturnInstance(this, instance);
				}
			}
			mEntries.Add(name, entry);
		}
		return true;
	}

	public GameObject GetPrefab(string prefabName)
	{
		FPoolEntry entry = mEntries.Get(prefabName);
		return (entry != null) ? entry.Prefab : null;
	}

	public GameObject GetObject(string prefabName)
	{
		FPoolEntry entry = mEntries.Get(prefabName);
		return (entry != null) ? entry.GetInstance(this) : null;
	}

	public void ReleaseObject(GameObject o)
	{
		FPoolEntry entry = mUsedInstances.Get(o);
		if (entry != null)
		{
			entry.ReturnInstance(this, o);
		}
		else
		{
			Object.Destroy(o);
		}
	}

	public void ObjectDestroyed(GameObject o)
	{
		mUsedInstances.Remove(o);
	}

	protected void AddUsedInstance(FPoolEntry entry, GameObject o)
	{
		mUsedInstances[o] = entry;
	}
}
