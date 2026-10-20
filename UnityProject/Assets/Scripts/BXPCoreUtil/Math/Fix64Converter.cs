using System;
using System.ComponentModel;
using System.Globalization;

// Rewritten from libil2cpp.so 1.0.605 (Fix64Converter @ 0x187541c..0x1875578, Ghidra vaddr).
public class Fix64Converter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (base.CanConvertFrom(context, sourceType))
		{
			return true;
		}
		return sourceType == typeof(string);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (Fix64.TryParse(value as string, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return null;
	}
}
