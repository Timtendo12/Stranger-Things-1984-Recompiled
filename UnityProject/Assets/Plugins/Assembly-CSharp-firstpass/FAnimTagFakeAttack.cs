using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagFakeAttack @ 0x160acf8..0x160adb8, Ghidra vaddr).
public class FAnimTagFakeAttack : FAnimTag
{
	public string LaunchBone { get; protected set; }

	public FAnimTagFakeAttack()
		: base(AnimTagTypes.FakeAttack)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			LaunchBone = root.GetAttrib("LaunchBone", LaunchBone);
		}
		return flag;
	}
}
