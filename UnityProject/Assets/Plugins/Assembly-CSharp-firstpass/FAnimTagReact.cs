using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagReact @ 0x160a36c..0x160a5ac, Ghidra vaddr).
public class FAnimTagReact : FAnimTag
{
	protected string mName = string.Empty;

	protected bool mStart = true;

	public string Name => mName;

	public bool Start => mStart;

	public bool End => !mStart;

	public FAnimTagReact()
		: base(AnimTagTypes.React)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			mName = root.GetAttrib("Name", mName);
			mStart = root.GetAttrib("Start", mStart);
		}
		return flag;
	}
}
