using UnityEngine;

public static class CameraExtensions
{
	public static bool IsScreenPosBehindCamera(this Camera camera, Vector3 screenPos)
	{
		return false;
	}

	public static bool IsBehindCamera(this Camera camera, Vector3 position)
	{
		return false;
	}
}
