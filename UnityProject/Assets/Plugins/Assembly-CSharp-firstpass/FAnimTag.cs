using System;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTag @ 0x16055bc..0x1608840, Ghidra vaddr).
public class FAnimTag
{
	protected AnimTagTypes mTagType = AnimTagTypes.Unknown;

	protected int mFrame;

	public bool PlayOnLoops = true;

	public string mWaitEvent;

	private bool mOneWaitPerEvent;

	public AnimTagTypes TagType => mTagType;

	public int Frame => mFrame;

	public string WaitEvent => mWaitEvent;

	public bool OneWaitPerEvent => mOneWaitPerEvent;

	public virtual bool Primary => false;

	public FAnimTag(AnimTagTypes tagType)
	{
		mTagType = tagType;
	}

	public override string ToString()
	{
		return string.Format("AnimTag {0} {1}  {2}", mTagType, mFrame, Time.time);
	}

	public virtual bool LoadXml(XmlElement root)
	{
		bool flag = root.GetAttrib("Frame", ref mFrame, 0);
		if (flag)
		{
			mWaitEvent = root.GetAttrib("WaitEvent");
			PlayOnLoops = root.GetAttrib("PlayOnLoops", PlayOnLoops);
			mOneWaitPerEvent = root.GetAttrib("OneWaitPerEvent", mOneWaitPerEvent);
		}
		return flag;
	}

	public static FAnimTag AllocateForType(AnimTagTypes tagType)
	{
		CreatesAttribute enumAttrib = tagType.GetEnumAttrib<CreatesAttribute>();
		if (enumAttrib.CreatesType == null)
		{
			FAssert.Fail("Tag type '" + tagType.ToString() + "' asks to create null type");
			return null;
		}
		if (typeof(FAnimTag).IsAssignableFrom(enumAttrib.CreatesType))
		{
			return Activator.CreateInstance(enumAttrib.CreatesType) as FAnimTag;
		}
		FAssert.Fail("Tag type '" + tagType.ToString() + "' asks to create non-FAnimTag: " + enumAttrib.CreatesType.ToString());
		return null;
	}

	public static FAnimTag CreateFromXml(XmlElement root)
	{
		AnimTagTypes result = AnimTagTypes.Unknown;
		FAnimTag fAnimTag = null;
		if (FEnum.TryParse(root.GetAttrib("Type"), ref result))
		{
			fAnimTag = AllocateForType(result);
			if (!fAnimTag.LoadXml(root))
			{
				fAnimTag = null;
			}
		}
		return fAnimTag;
	}

	public void ClampFrame(int low, int high)
	{
		mFrame = ((low <= mFrame) ? Math.Min(mFrame, high) : low);
	}
}
