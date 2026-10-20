using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FWorldManager @ 0x17688bc..0x1769214, Ghidra vaddr).
public class FWorldManager
{
	protected Dictionary<string, FProtoWorld> mProtoWorlds = new Dictionary<string, FProtoWorld>();

	protected string mFilename;

	protected int mNextWorldID;

	protected int mLoadedWorldCount;

	protected Material mBlitMat;

	public Dictionary<string, FProtoWorld> ProtoWorlds => mProtoWorlds;

	public string DefaultWorldName { get; protected set; }

	public static FWorldManager CreateFromFile(string filename)
	{
		FWorldManager manager = new FWorldManager();
		manager.LoadXml(filename);
		return manager;
	}

	public bool LoadXml(string filename)
	{
		mFilename = filename;
		mBlitMat = Resources.Load<Material>("Materials/BlitMat");
		XmlElement root = FUtil.LoadXmlAsset(filename);
		if (root == null)
		{
			// (sic) the shipped build logs "Resources" here.
			FLog.UnityError("Failed to parse Resources file.");
		}
		else
		{
			LoadXml(root);
		}
		return root != null;
	}

	public bool LoadXml(XmlElement root)
	{
		foreach (XmlNode node in root.ChildNodes)
		{
			if (node.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement element = (XmlElement)node;
			if (element.Name == "World")
			{
				FProtoWorld world = new FProtoWorld();
				if (world.LoadXml(element, mNextWorldID++))
				{
					mProtoWorlds.Add(world.Name, world);
				}
			}
			else if (element.Name == "Default")
			{
				DefaultWorldName = element.GetAttrib("Name", DefaultWorldName);
			}
		}
		return true;
	}

	public IEnumerator LoadWorlds(int count)
	{
		if (mProtoWorlds == null || mProtoWorlds.Count == 0)
		{
			FLog.Warning("mProtoWorlds is null or empty in LoadWorlds!", true);
			yield break;
		}
		int end = mLoadedWorldCount + count;
		for (int i = mLoadedWorldCount; i < end; i++)
		{
			if (mProtoWorlds.Count <= i)
			{
				end = mLoadedWorldCount + count;
				break;
			}
			mProtoWorlds.ElementAt(i).Value.Init();
			end = mLoadedWorldCount + count;
		}
		mLoadedWorldCount = end;
	}

	public FProtoWorld GetProtoWorld(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (!mProtoWorlds.ContainsKey(name))
		{
			return null;
		}
		return mProtoWorlds[name];
	}
}
