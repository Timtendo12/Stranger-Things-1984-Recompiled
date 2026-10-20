using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FCameraShakeParams @ 0x1787ac0..0x1788858, Ghidra vaddr).
public class FCameraShakeParams
{
	protected float mDuration;

	protected float mFrequency = 1f;

	protected float mMagnitude;

	public bool LoadXml(XmlElement root)
	{
		mDuration = root.GetAttrib("Duration", mDuration);
		mFrequency = root.GetAttrib("Frequency", mFrequency);
		mMagnitude = root.GetAttrib("Magnitude", mMagnitude);
		return true;
	}

	public void Apply(Vector3 position)
	{
		// The shipped body is empty (camera shake was disabled in the 2D game).
	}
}
