using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagMoveToPatrolPoint @ 0x160ac0c..0x160acc0, Ghidra vaddr).
public class FAnimTagMoveToPatrolPoint : FAnimTag
{
	public int Index { get; protected set; }

	public FAnimTagMoveToPatrolPoint()
		: base(AnimTagTypes.MoveToPatrolPoint)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			Index = root.GetAttrib("Index", Index);
		}
		return flag;
	}
}
