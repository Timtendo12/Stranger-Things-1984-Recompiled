using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FConsoleCommands @ 0x16f51e4..0x16f5ba0, Ghidra vaddr).
public static class FConsoleCommands
{
	[ConsoleCommand("Load")]
	public static void Load(string worldName)
	{
		bool flag = FApp.World != null;
		if (flag)
		{
			FApp.DestroyWorld(true);
		}
		FProtoWorld fProtoWorld = null;
		foreach (KeyValuePair<string, FProtoWorld> protoWorld in FApp.WorldManager.ProtoWorlds)
		{
			if (string.Equals(protoWorld.Key, worldName, StringComparison.OrdinalIgnoreCase))
			{
				fProtoWorld = protoWorld.Value;
				break;
			}
		}
		if (fProtoWorld == null)
		{
			FConsoleCore.Error("Tried to load a world that does not exist: " + worldName);
			return;
		}
		if (FApp.HUD.PageLegendWorld != null)
		{
			FApp.HUD.PageLegendWorld.DimWorld(1f, 0f);
		}
		if (!flag)
		{
			FApp.HUD.LoadLevel(worldName, null, null, false);
		}
		else
		{
			Futile.Instance.StartCoroutine(FWorld.Init(worldName));
		}
		// port guard: the debug page does not exist in the PC player
		if (FApp.HUD.PageDebug != null)
		{
			FApp.HUD.PageDebug.ToggleConsole();
		}
	}

	[ConsoleCommand("ChangeCharacter")]
	public static void ChangeCharacter(string charName)
	{
		if (FApp.World == null)
		{
			return;
		}
		FProtoUnit protoUnit = FApp.ProtoUnitManager.GetProtoUnit(charName, false);
		if (protoUnit == null)
		{
			FConsoleCore.Error("Invalid character name.");
		}
		else
		{
			FApp.World.SetPlayerCharacter(protoUnit, false);
		}
	}

	[ConsoleCommand("Delete All Cloud Save Data And Quit")]
	public static void DeleteAllCloudSaveDataAndQuit()
	{
		if (CloudSaveManager.Instance != null)
		{
			CloudSaveManager.Instance.DeletePlayerDataFromCloudSave();
			Application.Quit();
		}
	}
}
