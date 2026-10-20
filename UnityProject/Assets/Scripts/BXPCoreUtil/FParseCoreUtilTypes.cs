using System;
using System.Globalization;

// Rewritten from libil2cpp.so 1.0.605 (FParseCoreUtilTypes TryParseFix64 @ 0x188c274..0x188c334,
// TryParseFPoint2 @ 0x188c7e0..0x188c970, Ghidra vaddr).
// Only the Fix64 and FPoint2 parsers are kept; the FixVector2/FixVector3 ones were never called.
public static class FParseCoreUtilTypes
{
	private const NumberStyles cFloatStyle = NumberStyles.Float | NumberStyles.AllowThousands;

	public static bool TryParseFix64(this string str, out Fix64 val)
	{
		return str.TryParseFix64(out val, Fix64.Zero);
	}

	public static bool TryParseFix64(this string str, out Fix64 val, Fix64 defaultVal)
	{
		if (Fix64.TryParse(str, cFloatStyle, CultureInfo.InvariantCulture, out val))
		{
			return true;
		}
		val = defaultVal;
		return false;
	}

	public static bool TryParseFPoint2(this string str, out FPoint2 val)
	{
		return str.TryParseFPoint2(out val, default);
	}

	// "x,y" or "x y"; on a bad component val keeps the components parsed so far.
	public static bool TryParseFPoint2(this string str, out FPoint2 val, FPoint2 defaultVal)
	{
		val = defaultVal;
		if (str == null)
		{
			return false;
		}
		string[] parts = str.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length != 2)
		{
			return false;
		}
		for (int i = 0; i < parts.Length; i++)
		{
			if (!int.TryParse(parts[i], cFloatStyle, CultureInfo.InvariantCulture, out int component))
			{
				return false;
			}
			val[i] = component;
		}
		return true;
	}
}
