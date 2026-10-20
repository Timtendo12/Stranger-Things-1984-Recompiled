// Rewritten from libil2cpp.so 1.0.605 (FAnimEntry @ 0x1604d24..0x1605eb0, Ghidra vaddr).
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public class FAnimEntry
{
	public enum AnimFacing
	{
		NONE = 0,
		LEFT = 1,
		UP = 2,
		RIGHT = 3,
		DOWN = 4
	}

	protected string mName;

	protected string mSpriteSheet;

	protected int mFPS;

	protected int mLoops;

	protected int mFrameStart;

	protected int mFrameEnd;

	protected bool mReversed;

	protected AnimFacing mFacing = AnimFacing.LEFT;

	protected int mRotation;

	protected bool mFlipX;

	protected bool mFlipY;

	protected int mWeight = 1;

	protected float mLoopInterval;

	protected List<FAnimTag> mTags = new List<FAnimTag>();

	public string Name => mName;

	public string SpriteSheet => mSpriteSheet;

	public int FPS => mFPS;

	public int Loops => mLoops;

	public int FrameStart => mFrameStart;

	public int FrameEnd => mFrameEnd;

	public bool Reversed => mReversed;

	public AnimFacing Facing => mFacing;

	public int Rotation => mRotation;

	public bool FlipX => mFlipX;

	public bool FlipY => mFlipY;

	public int Weight => mWeight;

	public float LoopInterval => mLoopInterval;

	public bool LoadXml(FAnimSet set, XmlElement root)
	{
		mSpriteSheet = root.GetAttrib("SpriteSheet");
		if (mSpriteSheet == null)
		{
			return false;
		}
		mWeight = root.GetAttrib("Weight", 1);
		mFacing = (AnimFacing)Enum.Parse(typeof(AnimFacing), root.GetAttrib("Facing", "DOWN"));
		mName = set.AnimType.ToString() + "_" + mFacing.ToString();
		mFlipX = root.GetAttrib("FlipX", false);
		mFlipY = root.GetAttrib("FlipY", false);
		mFrameStart = root.GetAttrib("FrameStart", set.FrameStart);
		mFrameEnd = root.GetAttrib("FrameEnd", set.FrameEnd);
		mFPS = root.GetAttrib("FPS", set.FPS);
		mLoops = root.GetAttrib("Loops", set.Loops);
		mLoopInterval = root.GetAttrib("LoopInterval", set.LoopInterval);
		mReversed = root.GetAttrib("Reversed", set.Reversed);
		mRotation = root.GetAttrib("Rotation", set.Rotation);
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				FAnimTag fAnimTag = FAnimTag.CreateFromXml(childNode as XmlElement);
				if (fAnimTag != null)
				{
					mTags.Add(fAnimTag);
				}
			}
		}
		mTags = mTags.OrderBy((FAnimTag tag) => tag.Frame).ToList();
		return true;
	}

	public void EvaluateTags(int currentFrame, int totalFrames, int actionID, IAnimTagResponder TagResponder, bool firstRun)
	{
		for (int i = 0; i < mTags.Count; i++)
		{
			if (mTags[i].Frame == currentFrame && (mTags[i].PlayOnLoops || firstRun))
			{
				TagResponder.FireTag(mTags[i], actionID);
			}
		}
	}

	public int GetTagCount(AnimTagTypes tagType, bool primary)
	{
		int num = 0;
		for (int i = 0; i < mTags.Count; i++)
		{
			if (mTags[i].TagType == tagType)
			{
				num += ((mTags[i].Primary == primary) ? 1 : 0);
			}
		}
		return num;
	}

	public void ClampTagFrames(int low, int high)
	{
		for (int i = 0; i < mTags.Count; i++)
		{
			mTags[i].ClampFrame(low, high);
		}
	}

	public override string ToString()
	{
		return string.Format("[FAnimEntry: Name={0}, Facing={1}, Weight={2}, FlipX={3}, FlipY={4}, SpriteSheet={5}, FrameStart={6}, FrameEnd={7}, FPS={8}, Loops={9}, Reversed={10}, Rotation={11}]", mName, mFacing, mWeight, mFlipX, mFlipY, mSpriteSheet, mFrameStart, mFrameEnd, mFPS, mLoops, mReversed, mRotation);
	}
}
