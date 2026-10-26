using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// PC port (new): Discord Rich Presence. Polls the game state once a second, builds the activity (location, character, stats,
// boss / pause / menu states) and hands it to DiscordIpc only when the text changed (stats refresh at most every 15 s).
// Disabled (no-op) unless DISCORD_CLIENT_ID was set in work/.env at build time.
public class PCDiscord : MonoBehaviour
{
	private static readonly Dictionary<string, string> WorldFallback = new Dictionary<string, string>
	{
		{ "HopperHome", "Hopper's Home" }, { "HopperIntroInside", "Hopper's Home" }, { "HopperIntroInside_ReEnter", "Hopper's Home" }, { "HopperIntroOutside", "Outside Hopper's Home" },
		{ "HawkinsLabNight", "Hawkins National Laboratory (night)" }, { "HawkinsLab", "Hawkins National Laboratory" },
		{ "Library", "Hawkins Public Library" }, { "LibraryLit", "Hawkins Public Library" }, { "LibraryPrototype", "Hawkins Public Library" },
		{ "Arcade", "Palace Arcade" }, { "MelvaldsStore", "Melvald's General Store" }, { "HuntingStore", "Hunting & Camping" }, { "GroceryStore", "Bradley's Big Buy" },
		{ "FlowerShop", "Florist" }, { "HawkInside", "Hawk Movie Theater" }, { "ByersHouse", "Byers' Home" }, { "ByersGarage", "Byers' Garage" }, { "CastleByers", "Castle Byers" },
		{ "PhilLarsonGarage", "Phil Larson's Garage" }, { "PoliceStation", "Hawkins Police Station" }, { "House04", "A house in Hawkins" }, { "House05", "A house in Hawkins" },
		{ "WheelerHouse", "Wheelers' Home" }, { "ForestMaze", "Forest Maze" }, { "ForestLockBox", "The Forest" }, { "ForestTest", "The Forest" },
		{ "Sewers", "The Sewers" }, { "BunkerSewers", "Bunker Sewers" }, { "SewersUD", "The Sewers" },
		{ "MiddleSchoolDungeon", "Hawkins Middle School" }, { "MiddleSchool", "Hawkins Middle School" }, { "MiddleSchoolVertical", "Hawkins Middle School" }, { "MiddleSchoolLarge", "Hawkins Middle School" },
		{ "MiddleSchoolLargeUD", "Hawkins Middle School" }, { "MiddleSchoolUD", "Hawkins Middle School" }, { "MiddleSchoolBossPresent", "Hawkins Middle School" }, { "SchoolOutdoors", "Hawkins Middle School" },
		{ "HighSchool", "Hawkins High School" }, { "Darkroom", "Darkroom" },
		{ "LabBunker", "Laboratory Bunker (Level 1)" }, { "LabBunker01", "Laboratory Bunker (Level 1)" }, { "LabBunker02", "Laboratory Bunker (Level 2)" }, { "LabBunker03", "Laboratory Bunker (Level 3)" },
		{ "LabBunker04", "Laboratory Bunker (Level 4)" }, { "LabBunker05", "Secret Bunker" }, { "LabUpsideDown", "Hawkins National Laboratory" },
		{ "QuarryCave01", "Sattler Quarry Caves" }, { "QuarryCave02", "Sattler Quarry Caves" }, { "BearCave", "Bear Cave" }, { "Morgue", "Morgue" },
		{ "MDLab01", "Hawkins National Laboratory" }, { "MDLab02", "Hawkins National Laboratory" }, { "MDLab03", "Hawkins National Laboratory" }, { "MDLab04", "Hawkins National Laboratory" },
		{ "MDLoop01", "Hawkins" }, { "MDLoop02", "Hawkins" }, { "MDHI01", "Hawkins" }, { "MDHI02", "Hawkins" }, { "MDHI03", "Hawkins" },
		{ "VanChaseTest", "Bike chase" }, { "BikeRaceTest", "Bike chase" }, { "Theater1", "Hawk Movie Theater" },
		{ "BasementEnding", "Ending" }, { "TVEnding", "Ending" }, { "MovieEnding", "Ending" }, { "LockBoxEnding", "Ending" },
		{ "CheatMap", "Debug level" }, { "CombatTest", "Debug level" }, { "PatrolTest", "Debug level" }, { "IADTest", "Debug level" }, { "HugeSchoolTest", "Debug level" }, { "MapDCP", "Debug level" }, { "MapDCP2", "Debug level" }
	};

	private static PCDiscord sInstance;

	private static bool sForceUpdate;

	// debug overlay (Tools tab): poll and send the presence on the next frame, logging the result
	public static void ForceUpdate()
	{
		sForceUpdate = true;
	}

	private DiscordIpc mIpc;

	private bool mEnabled;

	private bool mShowStats = true;

	private string mLargeImage = "";

	private string mClientId = "";

	private long mStart;

	private string mLastKey = "";

	private string mLastJson;

	private float mNextPoll;

	private float mNextStats;

	private float mNextSend;

	private bool mDebugLog;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Bootstrap()
	{
#if !(UNITY_WEBGL && !UNITY_EDITOR) // a browser cannot reach the Discord client's local IPC pipe/socket
		if (!Application.isPlaying || sInstance != null)
		{
			return;
		}
		GameObject go = new GameObject("PCDiscord");
		DontDestroyOnLoad(go);
		sInstance = go.AddComponent<PCDiscord>();
#endif
	}

	private void Awake()
	{
		mDebugLog = PCDebug.Enabled;
		mStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		try
		{
			LoadConfig();
			if (mEnabled)
			{
				mIpc = new DiscordIpc(mClientId);
				mIpc.Log = delegate(string t)
				{
					Debug.Log(t);
				};
				mIpc.Start();
			}
		}
		catch (Exception e)
		{
			Debug.Log("Discord presence disabled: " + e.Message);
			mEnabled = false;
		}
	}

	private void LoadConfig()
	{
		// the id and image key are compiled in from .env (DiscordConfig.Generated.cs); nothing is read at runtime
		mClientId = DiscordConfig.ClientId;
		mLargeImage = DiscordConfig.LargeImageKey;
		mShowStats = true;
		mEnabled = mClientId.Length > 0;
	}

	private void OnApplicationQuit()
	{
		if (mIpc != null)
		{
			mIpc.Shutdown();
		}
	}

	private void Update()
	{
		bool debugKey = mDebugLog && sForceUpdate;
		sForceUpdate = false;
		if (!mEnabled && !debugKey)
		{
			return;
		}
		if (Time.unscaledTime < mNextPoll && !debugKey)
		{
			return;
		}
		mNextPoll = Time.unscaledTime + 1f;
		try
		{
			string stateKey;
			string json = BuildActivity(out stateKey);
			if (debugKey)
			{
				Debug.Log("PCDiscord activity: " + json);
			}
			if (!mEnabled || json == null)
			{
				return;
			}
			bool statsDue = Time.unscaledTime >= mNextStats;
			// the key holds the stable parts; stats only trigger a refresh every ~15 s
			if (stateKey == mLastKey && !statsDue)
			{
				return;
			}
			if (json == mLastJson || Time.unscaledTime < mNextSend)
			{
				return;
			}
			mLastKey = stateKey;
			mLastJson = json;
			mNextSend = Time.unscaledTime + 4f;
			mNextStats = Time.unscaledTime + 15f;
			mIpc.SetActivity(json);
		}
		catch (Exception e)
		{
			if (mDebugLog)
			{
				Debug.Log("PCDiscord update failed: " + e.Message);
			}
		}
	}

	private static string Loc(string key)
	{
		string text = FLoc.GetString(key, true);
		return string.IsNullOrEmpty(text) ? null : text.Replace("\n", " ");
	}

	// Readable place name for a world map name (e.g. "Library" -> "Hawkins Public Library"); null when there is none.
	public static string KnownWorldName(string worldName)
	{
		return WorldFallback.TryGetValue(worldName, out string name) ? name : null;
	}

	private static string Pretty(string worldName)
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < worldName.Length; i++)
		{
			if (i > 0 && char.IsUpper(worldName[i]) && !char.IsUpper(worldName[i - 1]))
			{
				sb.Append(' ');
			}
			sb.Append(worldName[i]);
		}
		return sb.ToString();
	}

	public static string LocationName(FWorld world)
	{
		string name = null;
		FRoom room = world.CurrentRoom;
		if (room != null && room.ProtoRoom != null && !string.IsNullOrEmpty(room.ProtoRoom.AreaName))
		{
			name = Loc(room.ProtoRoom.AreaName);
		}
		string worldName = world.ProtoWorld != null ? world.ProtoWorld.Name : "";
		if (name == null)
		{
			name = Loc("World." + worldName) ?? Loc("Dungeon." + worldName);
		}
		if (name == null)
		{
			string fallback;
			if (WorldFallback.TryGetValue(worldName, out fallback))
			{
				name = fallback;
			}
			else if (worldName.StartsWith("WorldMap") || worldName.StartsWith("UpsidedownWorldMap"))
			{
				name = "Exploring Hawkins";
			}
			else
			{
				name = Pretty(worldName);
			}
		}
		if (world.ProtoWorld != null && world.ProtoWorld.UpsideDown && name.IndexOf("Upside", StringComparison.OrdinalIgnoreCase) < 0)
		{
			name += " - " + (Loc("World.UpsideDown") ?? "The Upside Down");
		}
		return name;
	}

	private string BuildActivity(out string stateKey)
	{
		stateKey = "";
		FHUD hud = FApp.HUD;
		if (hud == null || !FApp.InitComplete)
		{
			return null;
		}
		FWorld world = FApp.World;
		FPlayerStats stats = FApp.PlayerStats;
		string details;
		string state = "";
		string character = null;
		if (world == null || world.ProtoWorld == null)
		{
			details = "In the main menu";
			stateKey = "menu";
		}
		else
		{
			string location = LocationName(world);
			details = location;
			if (stats != null && !string.IsNullOrEmpty(stats.CurrentCharacter))
			{
				character = Loc(stats.CurrentCharacter + ".Name") ?? stats.CurrentCharacter;
				state = "Playing as " + character;
			}
			if (world is FWorldTheater)
			{
				details = "Watching a movie at the Hawk Theater";
			}
			else if (world is FWorldVanChase)
			{
				details = "Chasing the Conspicuous Van";
			}
			else if (world is FWorldCinematic || world.ProtoWorld.Name.EndsWith("Ending"))
			{
				details = "Watching a cutscene";
			}
			string boss = BossName(world);
			if (hud.PagePause != null && hud.PagePause.OnStage)
			{
				details = "Paused - " + location;
			}
			else if (boss != null)
			{
				details = "Fighting the " + boss;
				state = location + (character != null ? " - " + character : "");
			}
			if (stats != null && mShowStats)
			{
				List<string> parts = new List<string>();
				if (stats.CurrentChapter > 0)
				{
					string subtitle = Loc("Chapter." + stats.CurrentChapter + ".SubTitle");
					parts.Add(subtitle != null ? "Ch. " + stats.CurrentChapter + ": " + subtitle : "Chapter " + stats.CurrentChapter);
				}
				parts.Add("HP " + stats.Health + "/" + stats.MaxHealth);
				parts.Add("$" + stats.Coins);
				parts.Add(Mathf.RoundToInt(stats.GetPercentComplete()) + "%");
				state += (state.Length > 0 ? " | " : "") + string.Join(" | ", parts.ToArray());
			}
			stateKey = details + "|" + (character ?? "") + "|" + (hud.PagePause != null && hud.PagePause.OnStage);
		}
		return Json(details, state, character);
	}

	private static string BossName(FWorld world)
	{
		if (!world.InBossEncounter || world.CurrentRoom == null)
		{
			return null;
		}
		List<FUnit> units = world.CurrentRoom.Units;
		for (int i = 0; i < units.Count; i++)
		{
			if (units[i] != null && units[i].ProtoUnit != null && units[i].ProtoUnit.IsTag("Boss") && units[i].AliveAndWell)
			{
				string name = Loc(units[i].ProtoUnit.Name + ".Name");
				return name ?? units[i].ProtoUnit.Name;
			}
		}
		return null;
	}

	public static string Escape(string s)
	{
		StringBuilder sb = new StringBuilder();
		foreach (char c in s)
		{
			if (c == '"' || c == '\\')
			{
				sb.Append('\\').Append(c);
			}
			else if (c < 0x20)
			{
				sb.Append(' ');
			}
			else
			{
				sb.Append(c);
			}
		}
		return sb.ToString();
	}

	private string Json(string details, string state, string character)
	{
		StringBuilder sb = new StringBuilder("{");
		sb.Append("\"details\":\"").Append(Escape(Clip(details))).Append('"');
		if (!string.IsNullOrEmpty(state))
		{
			sb.Append(",\"state\":\"").Append(Escape(Clip(state))).Append('"');
		}
		sb.Append(",\"timestamps\":{\"start\":").Append(mStart.ToString()).Append('}');
		if (mLargeImage.Length > 0)
		{
			sb.Append(",\"assets\":{");
			bool first = true;
			if (mLargeImage.Length > 0)
			{
				sb.Append("\"large_image\":\"").Append(Escape(mLargeImage)).Append("\",\"large_text\":\"Stranger Things: 1984\"");
				first = false;
			}
			if (character != null && mLargeImage.Length > 0)
			{
				sb.Append(first ? "" : ",").Append("\"small_image\":\"").Append(Escape(character.ToLowerInvariant().Replace(" ", ""))).Append("\",\"small_text\":\"").Append(Escape(character)).Append('"');
			}
			sb.Append('}');
		}
		sb.Append('}');
		return sb.ToString();
	}

	// Discord limits details / state to 128 characters
	private static string Clip(string s)
	{
		return s.Length > 120 ? s.Substring(0, 120) : s;
	}
}
