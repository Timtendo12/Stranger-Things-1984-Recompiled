// Rewritten from libil2cpp.so 1.0.605 (FAnimSet @ 0x1605f0c..0x1606bc4, Ghidra vaddr).
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public class FAnimSet
{
	protected int mAnimType = -1;

	protected int mFPS;

	protected int mLoops;

	protected float mLoopInterval;

	protected int mFrameStart;

	protected int mFrameEnd;

	protected bool mReversed;

	protected int mRotation;

	protected List<FAnimEntry> mAnimEntries = new List<FAnimEntry>();

	protected int mTotalWeight;

	public int AnimType => mAnimType;

	public int FPS => mFPS;

	public int Loops => mLoops;

	public float LoopInterval => mLoopInterval;

	public int FrameStart => mFrameStart;

	public int FrameEnd => mFrameEnd;

	public bool Reversed => mReversed;

	public int Rotation => mRotation;

	public List<FAnimEntry> AnimEntries => mAnimEntries;

	public bool LoadXml(XmlElement root)
	{
		mAnimType = FAnimDefManager.GetOrCreateAnimTypeID(root.GetAttribute("Type"));
		if (mAnimType < 0)
		{
			return false;
		}
		mFrameStart = root.GetAttrib("FrameStart", -1);
		mFrameEnd = root.GetAttrib("FrameEnd", -1);
		mFPS = root.GetAttrib("FPS", 12);
		mLoops = root.GetAttrib("Loops", 1);
		mLoopInterval = root.GetAttrib("LoopInterval", 0f);
		mReversed = root.GetAttrib("Reversed", false);
		mRotation = root.GetAttrib("Rotation", 0);
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			if (childNode.Name == "Anim")
			{
				FAnimEntry fAnimEntry = new FAnimEntry();
				if (fAnimEntry.LoadXml(this, childNode as XmlElement))
				{
					mAnimEntries.Add(fAnimEntry);
					mTotalWeight += fAnimEntry.Weight;
				}
			}
			else
			{
				FLog.UnityError("Founded unexpected node type: " + childNode.Name);
			}
		}
		return true;
	}

	public FAnimEntry Select(FRandom random)
	{
		int num = random.RangeInt(0, mTotalWeight - 1);
		int num2 = 0;
		FAnimEntry result = null;
		foreach (FAnimEntry mAnimEntry in mAnimEntries)
		{
			result = mAnimEntry;
			num2 += mAnimEntry.Weight;
			if (num2 > num)
			{
				break;
			}
		}
		return result;
	}

	public bool HasEntries()
	{
		return mAnimEntries.Count > 0;
	}

	public FAnimEntry GetAnimEntryByIndex(int index)
	{
		if (index >= 0 && index < mAnimEntries.Count)
		{
			return mAnimEntries[index];
		}
		return null;
	}

	public FAnimEntry GetAnimEntryByFacing(FAnimEntry.AnimFacing facing)
	{
		return mAnimEntries.FirstOrDefault((FAnimEntry a) => a.Facing == facing);
	}

	public int GetIndexForAnimEntry(FAnimEntry entry)
	{
		for (int i = 0; i < mAnimEntries.Count; i++)
		{
			if (mAnimEntries[i] == entry)
			{
				return i;
			}
		}
		return -1;
	}

	public override string ToString()
	{
		return string.Format("[FAnimSet: AnimType={0}]", mAnimType);
	}
}
