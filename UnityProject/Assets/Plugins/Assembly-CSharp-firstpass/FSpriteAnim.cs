using System.Collections.Generic;
using System.Xml;

public class FSpriteAnim
{
	public string Name;

	public List<FSpriteAnimFrame> Frames;

	protected float mDuration;

	protected float mRecipDuration;

	public float Duration
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public float RecipDuration => 0f;

	public bool LoadXml(XmlElement xmlRoot)
	{
		return false;
	}

	public float GetWrappedTime(float unwrappedTime)
	{
		return 0f;
	}

	public int GetFrameIndexForWrappedTime(float wrappedTime)
	{
		return 0;
	}

	public FSpriteAnimFrame GetFrameForTime(float t)
	{
		return null;
	}
}
