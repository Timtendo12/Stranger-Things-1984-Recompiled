using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (RectExtensions @ 0x16cace4..0x16cadb8, Ghidra vaddr).
public static class RectExtensions
{
	public static Rect CloneWithExpansion(this Rect rect, float expansionAmount)
	{
		return new Rect(rect.x - expansionAmount, rect.y - expansionAmount, rect.width + expansionAmount * 2f, rect.height + expansionAmount * 2f);
	}

	public static bool CheckIntersect(this Rect rect, Rect otherRect)
	{
		return otherRect.x <= rect.x + rect.width && otherRect.x + otherRect.width >= rect.x && rect.y + rect.height >= otherRect.y && rect.y <= otherRect.y + otherRect.height;
	}

	public static Rect CloneAndMultiply(this Rect rect, float multiplier)
	{
		return new Rect(rect.x * multiplier, rect.y * multiplier, rect.width * multiplier, rect.height * multiplier);
	}

	public static Rect CloneAndScale(this Rect rect, float scaleX, float scaleY)
	{
		return new Rect(rect.x * scaleX, rect.y * scaleY, rect.width * scaleX, rect.height * scaleY);
	}

	public static Rect CloneAndOffset(this Rect rect, float offsetX, float offsetY)
	{
		return new Rect(rect.x + offsetX, rect.y + offsetY, rect.width, rect.height);
	}

	public static Rect CloneAndScaleThenOffset(this Rect rect, float scaleX, float scaleY, float offsetX, float offsetY)
	{
		return new Rect(rect.x * scaleX + offsetX, rect.y * scaleY + offsetY, rect.width * scaleX, rect.height * scaleY);
	}

	public static float DistanceToPoint(this Rect rect, Vector2 point)
	{
		float num = Mathf.Max(Mathf.Abs(point.x - (rect.x + rect.width * 0.5f)) - rect.width * 0.5f, 0f);
		float num2 = Mathf.Max(Mathf.Abs(point.y - (rect.y + rect.height * 0.5f)) - rect.height * 0.5f, 0f);
		return Mathf.Sqrt(num * num + num2 * num2);
	}
}
