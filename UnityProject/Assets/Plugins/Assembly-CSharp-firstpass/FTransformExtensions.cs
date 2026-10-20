using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FTransformExtensions @ 0x166d6f8..0x166d804, Ghidra vaddr).
public static class FTransformExtensions
{
	public static void AddAttachment(this Transform xform, Transform attachment)
	{
		Vector3 localPosition = attachment.localPosition;
		Vector3 localScale = attachment.localScale;
		Quaternion localRotation = attachment.localRotation;
		attachment.parent = xform;
		attachment.localPosition = localPosition;
		attachment.localScale = localScale;
		attachment.localRotation = localRotation;
	}

	public static void AddAttachment(this Transform xform, GameObject attachment)
	{
		xform.AddAttachment(attachment.transform);
	}
}
