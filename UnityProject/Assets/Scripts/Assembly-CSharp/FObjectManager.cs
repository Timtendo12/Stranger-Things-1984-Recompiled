using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class FObjectManager : MonoBehaviour
{
	protected List<FObjectList> ObjectLists;

	protected Dictionary<string, int> PrefabIndexNames;

	protected Dictionary<GameObject, int> PrefabIndices;

	private void Awake()
	{
	}

	public bool LoadXml(XmlElement root)
	{
		return false;
	}

	public static GameObject CreateAutoMagnetVariation(GameObject originalPrefab)
	{
		return null;
	}

	public static int AddPrefab(string name, GameObject templateObject, int count, bool keepActive, bool reparent)
	{
		return 0;
	}

	public static GameObject GetObject(string name, bool growPoolIfNeeded = false, Vector3? position = null)
	{
		return null;
	}

	public static bool ReleaseObject(GameObject o)
	{
		return false;
	}

	public static void RecursiveNotifyGet(GameObject o)
	{
	}

	public static void RecursiveNotifyRelease(GameObject o)
	{
	}

	public static FObjectManagerEventHandler GetEventHandler(GameObject target)
	{
		return null;
	}

	public static GameObject Spawn(string name, Vector3 position, bool playParticles = true, string soundSet = null)
	{
		return null;
	}
}
