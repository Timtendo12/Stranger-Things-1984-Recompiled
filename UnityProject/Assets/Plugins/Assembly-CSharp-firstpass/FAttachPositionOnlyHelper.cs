using UnityEngine;

public static class FAttachPositionOnlyHelper
{
	public static void AddAttachmentPositionOnly(this Transform xform, GameObject attachObj, float cameraOffset, Vector3 offset = default(Vector3), Vector3 centerOffset = default(Vector3), FAttachPositionOnly.Options option = FAttachPositionOnly.Options.None, bool trackRotation = false, bool billboardTrackY = false, bool virtualAttachmentAutoKill = true)
	{
	}

	public static void AddAttachmentPositionOnly(this GameObject obj, GameObject attachObj, float cameraOffset, Vector3 offset = default(Vector3), Vector3 centerOffset = default(Vector3), FAttachPositionOnly.Options option = FAttachPositionOnly.Options.None, bool trackRotation = false, bool billboardTrackY = false, bool virtualAttachmentAutoKill = true)
	{
	}
}
