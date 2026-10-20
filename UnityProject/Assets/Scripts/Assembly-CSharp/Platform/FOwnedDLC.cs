using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FOwnedDLC @ 0x179686c..0x1796a04, Ghidra vaddr).
public class FOwnedDLC
{
	public string Name { get; protected set; }

	public bool IsApplied { get; protected set; }

	public FOwnedDLC()
	{
	}

	public FOwnedDLC(string dlcName, bool isApplied = false)
	{
		Name = dlcName;
		IsApplied = isApplied;
	}

	public void MarkApplied()
	{
		IsApplied = true;
	}

	public bool Load(XmlElement root)
	{
		Name = root.GetAttrib("Name");
		IsApplied = root.GetAttrib("IsApplied", false);
		return true;
	}

	public void Save(XmlElement parent)
	{
		parent.AddAttribute("Name", Name);
		parent.AddAttribute("IsApplied", IsApplied);
	}
}
