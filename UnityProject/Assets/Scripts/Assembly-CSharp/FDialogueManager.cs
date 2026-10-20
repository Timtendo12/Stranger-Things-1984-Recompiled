using System.Collections.Generic;
using System.Linq;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FDialogueManager @ 0x17ec6fc..0x17ed7b4, Ghidra vaddr).
public class FDialogueManager
{
	protected List<FDialogSet> mDialogueSets = new List<FDialogSet>();

	protected Dictionary<string, int> mDialogueIndices = new Dictionary<string, int>();

	protected string mFilename;

	public List<FDialogSet> DialogueSets => mDialogueSets;

	public static FDialogueManager CreateFromFile(string filename)
	{
		FDialogueManager manager = new FDialogueManager();
		manager.LoadXml(filename);
		return manager;
	}

	public bool LoadXml(string filename)
	{
		mFilename = filename;
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
			if (element.Name != "DialogueSet")
			{
				continue;
			}
			string world = element.GetAttrib("World", string.Empty);
			string unit = element.GetAttrib("Unit", string.Empty);
			if (!FCoreUtil.Valid(unit))
			{
				continue;
			}
			FDialogSet set = new FDialogSet(world, unit);
			if (set.LoadXML(element))
			{
				mDialogueSets.Add(set);
			}
		}
		return true;
	}

	public void SaveDialogueIndices()
	{
		if (FApp.PlayerStats == null)
		{
			return;
		}
		FApp.PlayerStats.DialogueIndices = new List<string>();
		foreach (KeyValuePair<string, int> pair in mDialogueIndices)
		{
			FApp.PlayerStats.DialogueIndices.Add(pair.Key + "|" + pair.Value.ToString());
		}
	}

	public void LoadDialogueIndices()
	{
		if (FApp.PlayerStats == null)
		{
			return;
		}
		mDialogueIndices.Clear();
		foreach (string entry in FApp.PlayerStats.DialogueIndices)
		{
			string[] parts = entry.Split('|');
			// PC port: TryParse so entries saved by earlier builds with a "|" key separator don't throw.
			if (parts.Length > 1 && int.TryParse(parts[1], out int savedIndex))
			{
				mDialogueIndices[parts[0]] = savedIndex;
			}
		}
	}

	public FDialogueLine GetDialogLine(string worldName, string unitName, int index = 0)
	{
		FDialogueLine[] lines = GetDialogLines(worldName, unitName);
		if (lines == null)
		{
			return null;
		}
		return lines[index];
	}

	public FDialogueLine[] GetDialogLines(string worldName, string unitName, bool incrementUsage = false)
	{
		if (!FCoreUtil.Valid(unitName))
		{
			return null;
		}
		FDialogSet[] unconditional = mDialogueSets.Where((FDialogSet s) => (!FCoreUtil.Valid(s.World) || s.World == worldName) && s.Unit == unitName && s.Requirements == null).ToArray();
		FDialogSet[] conditional = mDialogueSets.Where((FDialogSet s) => (!FCoreUtil.Valid(s.World) || s.World == worldName) && s.Unit == unitName && s.Requirements != null && s.CheckRequirements()).ToArray();
		FDialogSet chosen;
		if (conditional.Length == 0)
		{
			if (unconditional.Length == 0)
			{
				return null;
			}
			string key = worldName + "_" + unitName;
			int pick;
			if (!mDialogueIndices.ContainsKey(key))
			{
				mDialogueIndices[key] = 0;
				pick = 0;
			}
			else
			{
				pick = System.Math.Min(mDialogueIndices[key], unconditional.Length - 1);
			}
			if (incrementUsage)
			{
				mDialogueIndices[key] = mDialogueIndices[key] + 1;
			}
			chosen = unconditional[pick];
		}
		else
		{
			chosen = conditional[0];
		}
		if (chosen == null)
		{
			return null;
		}
		return chosen.Lines.ToArray();
	}

	public void ResetDialogueIndices()
	{
		mDialogueIndices.Clear();
	}
}
