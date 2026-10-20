using System;
using System.Collections.Generic;

public static class FEnum
{
	public static int GetLength<T>()
	{
		return Enum.GetValues(typeof(T)).Length;
	}

	public static int GetHighestValue<T>()
	{
		int highest = int.MinValue;
		foreach (object value in Enum.GetValues(typeof(T)))
		{
			highest = Math.Max(highest, Convert.ToInt32(value));
		}
		return highest;
	}

	public static bool TryParse<T>(string name, ref T result)
	{
		if (name != null && Enum.IsDefined(typeof(T), name))
		{
			result = (T)Enum.Parse(typeof(T), name);
			return true;
		}
		return false;
	}

	public static T FromInt<T>(int val) where T : struct, IConvertible
	{
		if (!Enum.IsDefined(typeof(T), val))
		{
			throw new System.ComponentModel.InvalidEnumArgumentException();
		}
		return (T)Enum.ToObject(typeof(T), val);
	}

	public static bool FromInt<T>(int val, out T result, T defaultResult = default(T)) where T : struct, IConvertible
	{
		if (Enum.IsDefined(typeof(T), val))
		{
			result = (T)Enum.ToObject(typeof(T), val);
			return true;
		}
		result = defaultResult;
		return false;
	}

	public static T GetEnumAttrib<T>(this Enum e) where T : Attribute
	{
		System.Reflection.MemberInfo[] members = e.GetType().GetMember(e.ToString());
		if (members != null && members.Length > 0)
		{
			object[] attributes = members[0].GetCustomAttributes(typeof(T), false);
			if (attributes != null && attributes.Length > 0)
			{
				return (T)attributes[0];
			}
		}
		return null;
	}

	public static Dictionary<Type, EnumType> BuildCreatesTypeToEnumDict<EnumType>()
	{
		Dictionary<Type, EnumType> dict = new Dictionary<Type, EnumType>();
		foreach (EnumType value in Enum.GetValues(typeof(EnumType)))
		{
			CreatesAttribute creates = (value as Enum).GetEnumAttrib<CreatesAttribute>();
			if (creates != null && creates.CreatesType != null)
			{
				dict[creates.CreatesType] = value;
			}
		}
		return dict;
	}

	public static T First<T>() where T : IComparable, IConvertible, IFormattable
	{
		if (!typeof(T).IsSubclassOf(typeof(Enum)))
		{
			throw new InvalidCastException(typeof(T).ToString() + " is not an enum");
		}
		return (T)Enum.GetValues(typeof(T)).GetValue(0);
	}

	public static T Last<T>() where T : IComparable, IConvertible, IFormattable
	{
		if (!typeof(T).IsSubclassOf(typeof(Enum)))
		{
			throw new InvalidCastException(typeof(T).ToString() + " is not an enum");
		}
		Array values = Enum.GetValues(typeof(T));
		return (T)values.GetValue(values.Length - 1);
	}
}
