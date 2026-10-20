using System.Text;

// Rewritten from libil2cpp.so 1.0.605 (ThaiFontAdjuster @ 0x16792d4..0x167989c, Ghidra vaddr).
public static class ThaiFontAdjuster
{
	public static bool IsThaiString(string s)
	{
		for (int i = 0; i < s.Length; i++)
		{
			if (s[i] >= '฀' && s[i] <= '๿')
			{
				return true;
			}
		}
		return false;
	}

	public static string Adjust(string s)
	{
		int length = s.Length;
		StringBuilder sb = new StringBuilder(length);
		for (int i = 0; i < length; i++)
		{
			char c = s[i];
			if (i >= 1 && IsTop(c))
			{
				char prev = s[i - 1];
				if (i > 1 && IsLower(prev))
				{
					prev = s[i - 2];
				}
				char mapped = c;
				if (IsBase(prev))
				{
					bool nextIsAm = i < length - 1 && (s[i + 1] == 'ำ' || s[i + 1] == 'ํ');
					if (!IsBaseAsc(prev))
					{
						mapped = (nextIsAm ? c : (char)(c - 0x173e));
					}
					else
					{
						if (nextIsAm)
						{
							sb.Append('');
							sb.Append((char)(c - 0x1735));
							i++;
							if (s[i] == 'ำ')
							{
								sb.Append('า');
							}
							continue;
						}
						mapped = (char)(c - 0x1743);
					}
				}
				if (i > 1 && IsUpper(s[i - 1]) && IsBaseAsc(s[i - 2]))
				{
					sb.Append((char)(mapped - 0x1735));
				}
				else
				{
					sb.Append(mapped);
				}
				continue;
			}
			if (IsUpper(c))
			{
				if (i >= 1)
				{
					char prev2 = s[i - 1];
					if (IsBaseAsc(prev2))
					{
						switch (c)
						{
						case 'ั':
							c = '';
							break;
						case 'ิ':
							c = '';
							break;
						case 'ี':
							c = '';
							break;
						case 'ึ':
							c = '';
							break;
						case 'ื':
							c = '';
							break;
						case '็':
							c = '';
							break;
						case 'ํ':
							c = '';
							break;
						}
						sb.Append(c);
						continue;
					}
				}
			}
			else if (i >= 1 && IsLower(c) && IsBaseDesc(s[i - 1]))
			{
				sb.Append((char)(c - 0x1720));
				continue;
			}
			if (c == 'ฐ')
			{
				if (i < length - 1)
				{
					c = (IsLower(s[i + 1]) ? '' : 'ฐ');
				}
			}
			else if (c == 'ญ')
			{
				c = ((i < length - 1 && IsLower(s[i + 1])) ? '' : 'ญ');
			}
			sb.Append(c);
		}
		return sb.ToString();
	}

	private static bool IsBase(char c)
	{
		if (c >= 'ก' && c <= 'ฯ')
		{
			return true;
		}
		return c == 'ะ' || c == 'เ' || c == 'แ';
	}

	private static bool IsBaseDesc(char c)
	{
		return (c & 0xFFFE) == 0xe0e;
	}

	private static bool IsBaseAsc(char c)
	{
		return c == 'ป' || c == 'ฝ' || c == 'ฟ' || c == 'ฬ';
	}

	private static bool IsTop(char c)
	{
		return c >= '่' && c <= '์';
	}

	private static bool IsLower(char c)
	{
		return c >= 'ุ' && c <= 'ฺ';
	}

	private static bool IsUpper(char c)
	{
		return c == 'ั' || c == 'ิ' || c == 'ี' || c == 'ึ' || c == 'ื' || c == '็' || c == 'ํ';
	}
}
