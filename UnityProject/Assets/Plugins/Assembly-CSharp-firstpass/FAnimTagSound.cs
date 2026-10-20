using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagSound @ 0x1608ab4..0x1608bd4, Ghidra vaddr).
public class FAnimTagSound : FAnimTag
{
	protected string mSoundSetName;

	protected string mSoundSetNameVO;

	public string SoundSetName => mSoundSetName;

	public string SoundSetNameVO => mSoundSetNameVO;

	public FAnimTagSound()
		: base(AnimTagTypes.Sound)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			mSoundSetName = root.GetAttrib("Name");
			mSoundSetNameVO = root.GetAttrib("VOName");
			return !string.IsNullOrEmpty(mSoundSetName) || !string.IsNullOrEmpty(mSoundSetNameVO);
		}
		return flag;
	}
}
