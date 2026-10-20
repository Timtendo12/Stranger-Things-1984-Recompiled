using System;
using System.Globalization;

// Rewritten from libil2cpp.so 1.0.605 (FParse @ 0x188bd70..0x188c23c, Ghidra vaddr).
public static class FParse
{
	private const NumberStyles cFloatStyle = NumberStyles.Float | NumberStyles.AllowThousands;

	private const NumberStyles cIntegerStyle = NumberStyles.Integer | NumberStyles.AllowThousands | NumberStyles.AllowExponent;

	public static float ParseFloat(this string str, float defaultVal = 0f)
	{
		str.TryParseFloat(out float val, defaultVal);
		return val;
	}

	public static DateTime ParseDateTime(this string str)
	{
		str.TryParseDateTime(out DateTime val);
		return val;
	}

	public static DateTime ParseDateTime(this string str, DateTime defaultVal)
	{
		str.TryParseDateTime(out DateTime val, defaultVal);
		return val;
	}

	public static double ParseDouble(this string str, double defaultVal = 0.0)
	{
		str.TryParseDouble(out double val, defaultVal);
		return val;
	}

	public static int ParseInt(this string str, int defaultVal = 0)
	{
		str.TryParseInt(out int val, defaultVal);
		return val;
	}

	public static long ParseLong(this string str, long defaultVal = 0L)
	{
		str.TryParseLong(out long val, defaultVal);
		return val;
	}

	public static ulong ParseUlong(this string str, ulong defaultVal = 0uL)
	{
		str.TryParseUlong(out ulong val, defaultVal);
		return val;
	}

	public static bool ParseBool(this string str, bool defaultVal = false)
	{
		str.TryParseBool(out bool val, defaultVal);
		return val;
	}

	public static bool TryParseFloat(this string str, out float val, float defaultVal = 0f)
	{
		bool ok = float.TryParse(str, cFloatStyle, CultureInfo.InvariantCulture, out val);
		if (!ok)
		{
			val = defaultVal;
		}
		return ok;
	}

	public static bool TryParseDateTime(this string str, out DateTime val)
	{
		return str.TryParseDateTime(out val, default(DateTime));
	}

	public static bool TryParseDateTime(this string str, out DateTime val, DateTime defaultVal)
	{
		bool ok = DateTime.TryParse(str, out val);
		if (!ok)
		{
			val = defaultVal;
		}
		return ok;
	}

	public static bool TryParseDouble(this string str, out double val, double defaultVal = 0.0)
	{
		bool ok = double.TryParse(str, cFloatStyle, CultureInfo.InvariantCulture, out val);
		if (!ok)
		{
			val = defaultVal;
		}
		return ok;
	}

	public static bool TryParseInt(this string str, out int val, int defaultVal = 0)
	{
		bool ok = int.TryParse(str, cIntegerStyle, CultureInfo.InvariantCulture, out val);
		if (!ok)
		{
			val = defaultVal;
		}
		return ok;
	}

	public static bool TryParseLong(this string str, out long val, long defaultVal = 0L)
	{
		bool ok = long.TryParse(str, cIntegerStyle, CultureInfo.InvariantCulture, out val);
		if (!ok)
		{
			val = defaultVal;
		}
		return ok;
	}

	public static bool TryParseUlong(this string str, out ulong val, ulong defaultVal = 0uL)
	{
		bool ok = ulong.TryParse(str, cIntegerStyle, CultureInfo.InvariantCulture, out val);
		if (!ok)
		{
			val = defaultVal;
		}
		return ok;
	}

	public static bool TryParseBool(this string str, out bool val, bool defaultVal = false)
	{
		if (bool.TryParse(str, out val))
		{
			return true;
		}
		if (int.TryParse(str, out int number) && (number == 0 || number == 1))
		{
			val = number != 0;
			return true;
		}
		val = defaultVal;
		return false;
	}
}
