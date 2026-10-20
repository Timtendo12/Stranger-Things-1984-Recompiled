using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoKitTweenExtensions @ 0x016ce640..0x016cfb58, Ghidra vaddr).
public static class GoKitTweenExtensions
{
	public static GoTween eularAnglesTo(this Transform self, float duration, Vector3 endValue, bool isRelative = false)
	{
		return Go.to(self, duration, new GoTweenConfig().eulerAngles(endValue, isRelative));
	}

	public static GoTween localEularAnglesTo(this Transform self, float duration, Vector3 endValue, bool isRelative = false)
	{
		return Go.to(self, duration, new GoTweenConfig().localEulerAngles(endValue, isRelative));
	}

	public static GoTween positionTo(this Transform self, float duration, Vector3 endValue, bool isRelative = false)
	{
		return Go.to(self, duration, new GoTweenConfig().position(endValue, isRelative));
	}

	public static GoTween localPositionTo(this Transform self, float duration, Vector3 endValue, bool isRelative = false)
	{
		return Go.to(self, duration, new GoTweenConfig().localPosition(endValue, isRelative));
	}

	public static GoTween scaleTo(this Transform self, float duration, float endValue, bool isRelative = false)
	{
		return self.scaleTo(duration, new Vector3(endValue, endValue, endValue), isRelative);
	}

	public static GoTween scaleTo(this Transform self, float duration, Vector3 endValue, bool isRelative = false)
	{
		return Go.to(self, duration, new GoTweenConfig().scale(endValue, isRelative));
	}

	public static GoTween shake(this Transform self, float duration, Vector3 shakeMagnitude, GoShakeType shakeType = GoShakeType.Position, int frameMod = 1, bool useLocalProperties = false)
	{
		return Go.to(self, duration, new GoTweenConfig().shake(shakeMagnitude, shakeType, frameMod, useLocalProperties));
	}

	public static GoTween eularAnglesFrom(this Transform self, float duration, Vector3 endValue, bool isRelative = false)
	{
		return Go.from(self, duration, new GoTweenConfig().eulerAngles(endValue, isRelative));
	}

	public static GoTween localEularAnglesFrom(this Transform self, float duration, Vector3 endValue, bool isRelative = false)
	{
		return Go.from(self, duration, new GoTweenConfig().localEulerAngles(endValue, isRelative));
	}

	public static GoTween positionFrom(this Transform self, float duration, Vector3 endValue, bool isRelative = false)
	{
		return Go.from(self, duration, new GoTweenConfig().position(endValue, isRelative));
	}

	public static GoTween localPositionFrom(this Transform self, float duration, Vector3 endValue, bool isRelative = false)
	{
		return Go.from(self, duration, new GoTweenConfig().localPosition(endValue, isRelative));
	}

	public static GoTween scaleFrom(this Transform self, float duration, Vector3 endValue, bool isRelative = false)
	{
		return Go.from(self, duration, new GoTweenConfig().scale(endValue, isRelative));
	}

	public static GoTween colorTo(this Material self, float duration, Color endValue, GoMaterialColorType colorType = GoMaterialColorType.Color)
	{
		return Go.to(self, duration, new GoTweenConfig().materialColor(endValue, colorType, false));
	}

	public static GoTween colorFrom(this Material self, float duration, Color endValue, GoMaterialColorType colorType = GoMaterialColorType.Color)
	{
		return Go.from(self, duration, new GoTweenConfig().materialColor(endValue, colorType, false));
	}
}
