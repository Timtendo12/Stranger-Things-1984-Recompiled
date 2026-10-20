using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagData @ 0x1608bd4..0x1608d74, Ghidra vaddr).
public class FAnimTagData : FAnimTag
{
	protected string mDataName;

	protected Fix64 mDataFloat;

	protected int mDataInt;

	public string DataName => mDataName;

	public Fix64 DataFloat => mDataFloat;

	public int DataInt => mDataInt;

	public FAnimTagData()
		: base(AnimTagTypes.Data)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		if (base.LoadXml(root))
		{
			mDataName = root.GetAttrib("Name");
			if (mDataName.Valid())
			{
				mDataFloat = root.GetAttrib("DataFloat", -Fix64.MinusOne);
				mDataInt = root.GetAttrib("DataInt", -1);
				return true;
			}
		}
		return false;
	}
}
