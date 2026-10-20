using System;
using System.Reflection;
using System.Text;

// Rewritten from libil2cpp.so 1.0.605 (FConsoleCommandEntry @ 0x1861b84..0x1861f80, Ghidra vaddr).
public class FConsoleCommandEntry
{
	public ConsoleCommand CommandAttrib;

	public MethodInfo Method;

	public string GetHelpString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Method.Name);
		ParameterInfo[] parameters = Method.GetParameters();
		if (parameters.Length != 0)
		{
			stringBuilder.Append("(");
			for (int i = 0; i < parameters.Length; i++)
			{
				stringBuilder.Append(parameters[i].Name);
				if (parameters[i].RawDefaultValue != DBNull.Value)
				{
					stringBuilder.Append("=");
					if (parameters[i].RawDefaultValue == null)
					{
						stringBuilder.Append("null");
					}
					else
					{
						if (parameters[i].ParameterType == typeof(string))
						{
							stringBuilder.Append("\"");
						}
						stringBuilder.Append(parameters[i].RawDefaultValue.ToString());
						if (parameters[i].ParameterType == typeof(string))
						{
							stringBuilder.Append("\"");
						}
					}
				}
				if (i < parameters.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.Append(")");
		}
		stringBuilder.Append(": ");
		stringBuilder.Append(CommandAttrib.Description);
		return stringBuilder.ToString();
	}
}
