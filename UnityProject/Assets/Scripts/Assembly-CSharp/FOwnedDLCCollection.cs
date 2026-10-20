using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FOwnedDLCCollection @ 0x1796a04..0x1797550, Ghidra vaddr).
public class FOwnedDLCCollection : KeyedCollection<string, FOwnedDLC>
{
	public bool Add(string dlcName, bool isApplied = false)
	{
		bool flag = Contains(dlcName);
		if (!flag)
		{
			Add(new FOwnedDLC(dlcName, isApplied));
		}
		return !flag;
	}

	public int Add(List<string> dlcNames, bool isApplied = false)
	{
		for (int i = 0; i < dlcNames.Count; i++)
		{
			Add(dlcNames[i], isApplied);
		}
		return 0;
	}

	public bool HasApplied(string dlcName)
	{
		return Contains(dlcName) && base[dlcName].IsApplied;
	}

	public List<FOwnedDLC> GetUnapplied()
	{
		List<FOwnedDLC> list = new List<FOwnedDLC>(base.Count);
		for (int i = 0; i < base.Count; i++)
		{
			if (!base[i].IsApplied)
			{
				list.Add(base[i]);
			}
		}
		return list;
	}

	public void Save(XmlElement root, string createParentName = null)
	{
		if (!string.IsNullOrEmpty(createParentName))
		{
			root = root.AddChild(createParentName, string.Empty);
		}
		using (IEnumerator<FOwnedDLC> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				FOwnedDLC current = enumerator.Current;
				current.Save(root.AddChild("DLC", string.Empty));
			}
		}
	}

	public void Load(XmlElement root, string findParentName = null)
	{
		if (!string.IsNullOrEmpty(findParentName))
		{
			root = root.GetChild(findParentName);
			if (root == null)
			{
				return;
			}
		}
		Clear();
		foreach (XmlElement item in root.Elements())
		{
			if (item.Name == "DLC")
			{
				FOwnedDLC fOwnedDLC = new FOwnedDLC();
				fOwnedDLC.Load(item);
				Add(fOwnedDLC);
			}
		}
	}

	protected override string GetKeyForItem(FOwnedDLC dlc)
	{
		return dlc.Name;
	}
}
