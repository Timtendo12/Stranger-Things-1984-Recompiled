using UnityEngine;

public static class FParseUnityTypes
{
	public static bool TryParseVector2(this string str, out Vector2 val, Vector2 defaultVal = default(Vector2))
	{
		val = default;
		return false;
	}

	public static Vector2 ParseVector2(this string str, Vector2 defaultVal = default(Vector2))
	{
		return default;
	}

	public static bool TryParseVector3(this string str, out Vector3 val, Vector3 defaultVal = default(Vector3))
	{
		val = default;
		return false;
	}

	public static Vector3 ParseVector3(this string str, Vector3 defaultVal = default(Vector3))
	{
		return default;
	}
}
