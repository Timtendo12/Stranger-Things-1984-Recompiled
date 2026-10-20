// Rewritten from libil2cpp.so 1.0.605 (FConsoleCommandsConfig @ 0x1861454..0x1861b1c, Ghidra vaddr).
public static class FConsoleCommandsConfig
{
	[ConsoleCommand("Process a config statement.")]
	public static void Config(string configStatement)
	{
		FConfigSystem.ProcessString(configStatement);
	}

	[ConsoleCommand("Toggle a boolean config.")]
	public static void ConfigToggle(string configName)
	{
		System.Type type = FConfigSystem.GetType(configName);
		string text = " not found.";
		if (type != null)
		{
			text = " is not boolean, so cannot be toggled.";
			if (type == typeof(bool))
			{
				bool flag = !(bool)FConfigSystem.Get(configName);
				FConfigSystem.Set(configName, flag);
				FConsoleCore.Output(configName + " is now: " + flag.ToString());
				return;
			}
		}
		FConsoleCore.Error("Config " + configName + text);
	}

	[ConsoleCommand("Set the value of a config.")]
	public static void ConfigSet(string configName, string newValue)
	{
		FConfigSystem.SetFromString(configName, newValue);
	}

	[ConsoleCommand("Print value of config.")]
	public static void ConfigGet(string configName)
	{
		object obj = FConfigSystem.Get(configName);
		if (obj != null)
		{
			FConsoleCore.Output(configName + " = " + obj.ToString());
			return;
		}
		FConsoleCore.Error("Config " + configName + " not found.");
	}
}
