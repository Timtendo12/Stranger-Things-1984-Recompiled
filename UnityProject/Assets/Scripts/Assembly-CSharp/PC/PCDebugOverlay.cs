using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// PC port (new): developer overlay for -debug (see PCDebug). F1 opens a window with tabs for teleporting to any world or entry
// point, giving currencies and collectibles, unlocking characters, boss / combat tests and the diagnostic dumps that used to sit
// on F1-F12 / Home / Insert. An FPS counter stays in the top-right corner while it is closed.
// While open, PCInput ignores the keyboard / gamepad and FTouchManager ignores the mouse (PCDisplay.PointerBlocked); the game
// keeps running unless paused here. Drawn with IMGUI in a 1280 x 720 virtual space scaled to the window height.
[DefaultExecutionOrder(50)] // before PCInput (100), so IsOpen is current when it reads input
public class PCDebugOverlay : MonoBehaviour
{
	private const float VirtualHeight = 720f;

	private enum Tab
	{
		Worlds,
		Player,
		Items,
		Characters,
		Combat,
		Tools
	}

	private static readonly string[] TabNames = Enum.GetNames(typeof(Tab));

	private static readonly FProtoCollectible.Types[] CollectibleTypes =
	{
		FProtoCollectible.Types.DungeonItem, FProtoCollectible.Types.Tool, FProtoCollectible.Types.CharacterUpgrade,
		FProtoCollectible.Types.HeartPiece, FProtoCollectible.Types.Character, FProtoCollectible.Types.Outfit,
		FProtoCollectible.Types.QuestItem, FProtoCollectible.Types.Gnome, FProtoCollectible.Types.Eggo, FProtoCollectible.Types.Film
	};

	private static readonly string[] CollectibleTypeNames = CollectibleTypes.Select((FProtoCollectible.Types t) => t.ToString()).ToArray();

	private static PCDebugOverlay sInstance;

	public static bool IsOpen { get; private set; }

	// diagnostics that used to be always on in -debug, now opt-in from the Tools tab
	public static bool LogMoves;

	public static bool LogAudio;

	private static bool sInvulnerable;

	private static readonly Dictionary<FProtoUnit, float> sDamageTakenBefore = new Dictionary<FProtoUnit, float>();

	private Tab mTab;

	private Rect mWindow = new Rect(24f, 24f, 600f, 660f);

	private Vector2 mScroll;

	private string mSearch = string.Empty;

	private string mExpandedWorld;

	private int mCollectibleType;

	private bool mPaused;

	private bool mBossBeating;

	private float mFps;

	private float mFpsTime;

	private int mFpsFrames;

	private float mNextAudioLog;

	private GUIStyle mFpsStyle;

	private GUIStyle mHeaderStyle;

	public static void Create(GameObject host)
	{
		if (sInstance == null)
		{
			sInstance = host.AddComponent<PCDebugOverlay>();
		}
	}

	private void Update()
	{
		mFpsFrames++;
		mFpsTime += Time.unscaledDeltaTime;
		if (mFpsTime >= 0.5f)
		{
			mFps = mFpsFrames / mFpsTime;
			mFpsFrames = 0;
			mFpsTime = 0f;
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			IsOpen = !IsOpen;
			GUIUtility.keyboardControl = 0;
		}
		PCDisplay.PointerBlocked = IsOpen;
		if (sInvulnerable && FWorld.Player != null && FWorld.Player.ProtoUnit != null)
		{
			MakeInvulnerable(FWorld.Player.ProtoUnit); // re-applied: the player's proto changes with the character
		}
		if (LogAudio)
		{
			LogPlayingAudio();
		}
	}

	private void OnGUI()
	{
		// GUI.matrix outlives this call and transforms Event.current.mousePosition, which Futile.OnGUI turns into world taps:
		// leaving it scaled made clicks land at (position / scale) whenever this ran first
		Matrix4x4 matrix = GUI.matrix;
		try
		{
			DrawOverlay();
		}
		finally
		{
			GUI.matrix = matrix;
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
		}
	}

	private void DrawOverlay()
	{
		float scale = Screen.height / VirtualHeight;
		GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
		float width = Screen.width / scale;
		if (mFpsStyle == null)
		{
			mFpsStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperRight, fontStyle = FontStyle.Bold };
			mHeaderStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
		}
		GUI.color = mFps >= 50f ? Color.green : (mFps >= 30f ? Color.yellow : Color.red);
		GUI.Label(new Rect(width - 210f, 4f, 200f, 22f), mFps.ToString("0") + " FPS" + (IsOpen ? string.Empty : "   F1: debug"), mFpsStyle);
		GUI.color = Color.white;
		if (!IsOpen)
		{
			return;
		}
		GUI.depth = -1000;
		GUI.backgroundColor = new Color(0.1f, 0.1f, 0.12f, 1f);
		mWindow = GUI.Window(0x5043, mWindow, DrawWindow, "Debug   (F1 to close)");
		GUI.backgroundColor = Color.white;
	}

	private void DrawWindow(int id)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(mFps.ToString("0") + " FPS", GUILayout.Width(60f));
		GUILayout.Label(Location());
		GUILayout.FlexibleSpace();
		if (GUILayout.Button(mPaused ? "Resume" : "Pause", GUILayout.Width(80f)))
		{
			mPaused = !mPaused;
			Time.timeScale = mPaused ? 0f : 1f;
		}
		GUILayout.EndHorizontal();
		int tab = GUILayout.Toolbar((int)mTab, TabNames);
		if (tab != (int)mTab)
		{
			mTab = (Tab)tab;
			mScroll = Vector2.zero;
			mSearch = string.Empty;
		}
		GUILayout.Space(4f);
		switch (mTab)
		{
		case Tab.Worlds:
			DrawWorlds();
			break;
		case Tab.Player:
			DrawPlayer();
			break;
		case Tab.Items:
			DrawItems();
			break;
		case Tab.Characters:
			DrawCharacters();
			break;
		case Tab.Combat:
			DrawCombat();
			break;
		case Tab.Tools:
			DrawTools();
			break;
		}
		GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
	}

	private static string Location()
	{
		FWorld world = FApp.World;
		if (world == null || world.ProtoWorld == null)
		{
			return "No world loaded";
		}
		string room = world.CurrentRoom != null ? world.CurrentRoom.ProtoRoom.Name : "-";
		string tile = FWorld.Player != null ? " @ " + FWorld.Player.TilePosition.x + "," + FWorld.Player.TilePosition.y : string.Empty;
		return world.ProtoWorld.Name + " / " + room + tile;
	}

	private void Search()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Search", GUILayout.Width(50f));
		mSearch = GUILayout.TextField(mSearch);
		if (GUILayout.Button("x", GUILayout.Width(24f)))
		{
			mSearch = string.Empty;
		}
		GUILayout.EndHorizontal();
	}

	private bool Matches(params string[] texts)
	{
		if (mSearch.Length == 0)
		{
			return true;
		}
		foreach (string text in texts)
		{
			if (text != null && text.IndexOf(mSearch, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	// ---- Worlds

	private void DrawWorlds()
	{
		if (FApp.WorldManager == null)
		{
			GUILayout.Label("Worlds are not loaded yet.");
			return;
		}
		if (GUILayout.Button("Library boss room (top entrance)"))
		{
			TeleportToLibraryBoss();
		}
		Search();
		GUILayout.Label(FApp.World == null
			? "Click a world to load it. Entry points are available once you are in a world."
			: "Click a world to load it at its start, or open it (>) to arrive at an entry point.");
		mScroll = GUILayout.BeginScrollView(mScroll);
		foreach (KeyValuePair<string, FProtoWorld> pair in FApp.WorldManager.ProtoWorlds.OrderBy((KeyValuePair<string, FProtoWorld> p) => p.Key, StringComparer.OrdinalIgnoreCase))
		{
			string known = PCDiscord.KnownWorldName(pair.Key);
			if (!Matches(pair.Key, known))
			{
				continue;
			}
			FProtoWorld proto = pair.Value;
			bool canExpand = FApp.World != null && proto.Initialized && proto.ProtoRooms.Any((FProtoRoom r) => r.EntryDatas.Count > 0);
			bool expanded = mExpandedWorld == pair.Key;
			GUILayout.BeginHorizontal();
			GUI.enabled = canExpand;
			if (GUILayout.Button(expanded ? "v" : ">", GUILayout.Width(24f)))
			{
				mExpandedWorld = expanded ? null : pair.Key;
			}
			GUI.enabled = true;
			if (GUILayout.Button(pair.Key + (known != null ? "  -  " + known : string.Empty), LeftButton()))
			{
				LoadWorld(pair.Key);
			}
			GUILayout.EndHorizontal();
			if (!expanded || !canExpand)
			{
				continue;
			}
			foreach (FProtoRoom room in proto.ProtoRooms)
			{
				foreach (FEntryData entry in room.EntryDatas)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space(32f);
					string leadsTo = entry.LeadsTo.Valid() ? "   (leads to " + entry.LeadsTo + ")" : string.Empty;
					if (GUILayout.Button(entry.Name + "   " + room.Name + " " + entry.Position.x + "," + entry.Position.y + leadsTo, LeftButton()))
					{
						EnterWorld(pair.Key, entry.Name);
					}
					GUILayout.EndHorizontal();
				}
			}
		}
		GUILayout.EndScrollView();
	}

	private GUIStyle mLeftButton;

	private GUIStyle LeftButton()
	{
		if (mLeftButton == null)
		{
			mLeftButton = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
		}
		return mLeftButton;
	}

	private static void LoadWorld(string worldName)
	{
		Debug.Log("PCDebug load world " + worldName);
		FConsoleCommands.Load(worldName);
	}

	private static void EnterWorld(string worldName, string entryPoint)
	{
		if (FApp.World == null)
		{
			LoadWorld(worldName);
			return;
		}
		Debug.Log("PCDebug enter " + worldName + "." + entryPoint);
		FApp.World.TryWorldTransition(worldName, null, new FPlayerStartParams { EntryPoint = entryPoint });
	}

	// Library boss room's top entrance (Entry4, 24,14), where the stealth run starts. Not inside the room: the nearest entrance
	// becomes the room's reset point, and from tile 24,3 that was the bottom exit (Exit2, 21,0) in a guard's beam, so every reset
	// was caught again. Switches rooms like FWorld's start-room path (ForceChangeCurrentRoom, then the tile), or the camera
	// stays behind. Loads the Library first when another world is current.
	public static void TeleportToLibraryBoss()
	{
		if (FApp.World == null || FWorld.Player == null)
		{
			return;
		}
		if (FApp.World.ProtoWorld.Name != "Library")
		{
			Debug.Log("PCDebug: load the Library first (Worlds tab), then use the boss room button");
			return;
		}
		FPoint2 target = new FPoint2(24, 14);
		FRoom targetRoom = FApp.World.Rooms.Find((FRoom r) => r != null && r.ContainsFPoint(target));
		FWorld.Player.AI.StopActions();
		FWorld.Player.GO.transform.position = new Vector3(target.x, target.y, FWorld.Player.GO.transform.position.z);
		if (targetRoom != null && targetRoom != FApp.World.CurrentRoom)
		{
			FApp.World.ForceChangeCurrentRoom(targetRoom, false);
		}
		FWorld.Player.SetTilePosition(target, true);
		FApp.World.CurrentRoom.PlayerEntranceComplete();
		Debug.Log("PCTeleport room=" + FApp.World.CurrentRoom.ProtoRoom.Name);
	}

	// ---- Player

	private void DrawPlayer()
	{
		FPlayerStats stats = FApp.PlayerStats;
		FUnit player = FWorld.Player;
		if (stats == null)
		{
			GUILayout.Label("No player data yet.");
			return;
		}
		GUILayout.Label("Character: " + stats.CurrentCharacter + "    Health: " + stats.Health + " / " + stats.MaxHealth, mHeaderStyle);
		GUILayout.BeginHorizontal();
		GUI.enabled = player != null;
		if (GUILayout.Button("Heal to full"))
		{
			stats.SetHealth(stats.MaxHealth);
			FApp.World.FillPlayerHealth();
		}
		if (GUILayout.Button("-1 HP"))
		{
			player.SetHP(Math.Max(player.HP - 1, 0));
		}
		if (GUILayout.Button("Kill (death / retry flow)"))
		{
			player.SetHP(0);
		}
		GUI.enabled = true;
		GUILayout.EndHorizontal();
		bool invulnerable = GUILayout.Toggle(sInvulnerable, " Invulnerable (no damage taken)");
		if (invulnerable != sInvulnerable)
		{
			sInvulnerable = invulnerable;
			if (!invulnerable)
			{
				foreach (KeyValuePair<FProtoUnit, float> pair in sDamageTakenBefore)
				{
					pair.Key.SetDamageTakenModifier(pair.Value);
				}
				sDamageTakenBefore.Clear();
			}
		}
		GUILayout.Space(8f);
		GUILayout.Label("Play as (unlocked or not) - the Characters tab unlocks them for the picker:");
		DrawCharacterButtons(false);
	}

	// zero the proto's damage-taken modifier, remembering the original for when invulnerability is switched off
	private static void MakeInvulnerable(FProtoUnit proto)
	{
		if (!sDamageTakenBefore.ContainsKey(proto))
		{
			sDamageTakenBefore[proto] = proto.DamageTakenModifier;
			proto.SetDamageTakenModifier(0f);
		}
	}

	// ---- Items

	private void DrawItems()
	{
		FPlayerStats stats = FApp.PlayerStats;
		if (stats == null || FApp.ProtoCollectibleManager == null)
		{
			GUILayout.Label("No player data yet.");
			return;
		}
		CurrencyRow("Coins", stats.Coins, stats.MaxCoins, stats.SetCoins);
		CurrencyRow("Pudding", stats.Pudding, stats.MaxPudding, stats.SetPudding);
		CurrencyRow("Key cards", stats.KeyCards, stats.MaxKeyCards, stats.SetKeyCards);
		CurrencyRow("Rocks", stats.Rocks, stats.MaxRocks, stats.SetRocks);
		GUILayout.Space(6f);
		mCollectibleType = GUILayout.SelectionGrid(mCollectibleType, CollectibleTypeNames, 5);
		FProtoCollectible.Types type = CollectibleTypes[mCollectibleType];
		Search();
		List<FProtoCollectible> list = FApp.ProtoCollectibleManager.ProtoCollectibles.Values
			.Where((FProtoCollectible c) => c.Type == type && Matches(c.Name, c.VisibleName))
			.OrderBy((FProtoCollectible c) => c.Name, StringComparer.OrdinalIgnoreCase)
			.ToList();
		int owned = list.Count((FProtoCollectible c) => stats.HasCollectible(c.Name));
		GUILayout.BeginHorizontal();
		GUILayout.Label(owned + " / " + list.Count + " owned");
		if (GUILayout.Button("Give all shown", GUILayout.Width(140f)))
		{
			foreach (FProtoCollectible c in list)
			{
				stats.AddCollectible(c.Name);
			}
			Save();
		}
		GUILayout.EndHorizontal();
		mScroll = GUILayout.BeginScrollView(mScroll);
		foreach (FProtoCollectible c in list)
		{
			bool has = stats.HasCollectible(c.Name);
			GUILayout.BeginHorizontal();
			GUILayout.Label((has ? "[x] " : "[  ] ") + c.Name + (c.VisibleName.Valid() && c.VisibleName != c.Name ? "  -  " + c.VisibleName : string.Empty) + "  (#" + c.DBID + ")");
			GUI.enabled = !has;
			if (GUILayout.Button("Give", GUILayout.Width(60f)))
			{
				stats.AddCollectible(c.Name);
				Save();
			}
			GUI.enabled = true;
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
	}

	private static void CurrencyRow(string label, int value, int max, Action<int> set)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label + ": " + value + " / " + max, GUILayout.Width(170f));
		if (GUILayout.Button("0"))
		{
			set(0);
		}
		if (GUILayout.Button("+1"))
		{
			set(value + 1);
		}
		if (GUILayout.Button("+10"))
		{
			set(value + 10);
		}
		if (GUILayout.Button("Max"))
		{
			set(max);
		}
		GUILayout.EndHorizontal();
	}

	private static void Save()
	{
		FApp.PlayerStats.SavePlayerState();
	}

	// ---- Characters

	private void DrawCharacters()
	{
		if (FApp.ProtoUnitManager == null || FApp.PlayerStats == null)
		{
			GUILayout.Label("No player data yet.");
			return;
		}
		GUILayout.Label("A character unlocks through its collectible (as in the shipped game), so this persists in the save.");
		if (GUILayout.Button("Unlock all characters"))
		{
			foreach (FProtoUnit proto in Characters())
			{
				UnlockCharacter(proto);
			}
			Save();
		}
		GUILayout.Space(4f);
		DrawCharacterButtons(true);
	}

	private static FProtoUnit[] Characters()
	{
		return FApp.ProtoUnitManager.GetProtoUnitsWithTag("PlayerCharacter") ?? new FProtoUnit[0];
	}

	private void DrawCharacterButtons(bool showUnlock)
	{
		if (FApp.ProtoUnitManager == null)
		{
			return;
		}
		foreach (FProtoUnit proto in Characters())
		{
			bool unlocked = proto.CheckUnlockRequirements();
			bool current = FApp.PlayerStats != null && FApp.PlayerStats.CurrentCharacter == proto.Name;
			GUILayout.BeginHorizontal();
			GUILayout.Label((unlocked ? "[x] " : "[  ] ") + proto.Name + (current ? "   (current)" : string.Empty));
			if (showUnlock)
			{
				GUI.enabled = !unlocked;
				if (GUILayout.Button("Unlock", GUILayout.Width(80f)))
				{
					UnlockCharacter(proto);
					Save();
				}
			}
			GUI.enabled = FApp.World != null && !current;
			if (GUILayout.Button("Play as", GUILayout.Width(80f)))
			{
				FConsoleCommands.ChangeCharacter(proto.Name);
			}
			GUI.enabled = true;
			GUILayout.EndHorizontal();
		}
	}

	private static void UnlockCharacter(FProtoUnit proto)
	{
		if (proto.UnlockRequirements == null)
		{
			return;
		}
		foreach (FPlayerStatsRequirement requirement in proto.UnlockRequirements)
		{
			if (requirement is FRequirementCollectible collectible && !collectible.FailIfCollected)
			{
				FApp.PlayerStats.AddCollectible(collectible.DBID);
			}
		}
	}

	// ---- Combat

	private void DrawCombat()
	{
		bool inWorld = FApp.World != null && FWorld.Player != null && FApp.World.CurrentRoom != null;
		FUnit boss = inWorld ? RoomBoss() : null;
		GUILayout.Label(boss != null ? "Boss in this room: " + boss.ProtoUnit.Name + "  HP " + boss.HP : "No living boss in this room.", mHeaderStyle);
		GUILayout.BeginHorizontal();
		GUI.enabled = boss != null && !mBossBeating;
		if (GUILayout.Button("Hit boss once"))
		{
			StartCoroutine(BeatBoss(1));
		}
		if (GUILayout.Button("Hit boss until gone (every 0.7 s)"))
		{
			StartCoroutine(BeatBoss(120));
		}
		GUI.enabled = true;
		GUILayout.EndHorizontal();
		GUILayout.Space(6f);
		GUI.enabled = inWorld;
		if (GUILayout.Button("Knock the player one tile left (thrown-vial knockback)"))
		{
			FWorld.Player.OnAttacked(FWorld.Player, 1, Vector2.left, 1, true);
		}
		if (GUILayout.Button("Step the player one tile left into whatever is there (e.g. a laser)"))
		{
			FPoint2 tp = FWorld.Player.TilePosition;
			FWorld.Player.AI.StopActions();
			FWorld.Player.AI.DoMoveToPoint(new FPoint2(tp.x - 1, tp.y), 0, false, true, true, string.Empty);
		}
		GUI.enabled = true;
	}

	private static FUnit RoomBoss()
	{
		FUnit boss = null;
		List<FUnit> units = FApp.World.CurrentRoom.Units;
		for (int u = 0; u < units.Count; u++)
		{
			if (units[u] != null && units[u].ProtoUnit.IsTag("Boss") && units[u].AliveAndWell)
			{
				boss = units[u];
			}
		}
		return boss;
	}

	// hit the boss of the current room with the player's weapon, every 0.7 s, until it is gone or maxHits is reached
	private IEnumerator BeatBoss(int maxHits)
	{
		mBossBeating = true;
		for (int i = 0; i < maxHits; i++)
		{
			FUnit boss = FApp.World != null && FApp.World.CurrentRoom != null ? RoomBoss() : null;
			if (boss == null || FWorld.Player == null)
			{
				break;
			}
			boss.OnAttacked(FWorld.Player, null, true, null);
			Debug.Log("PCBoss hit hp=" + boss.HP);
			yield return new WaitForSeconds(0.7f);
		}
		mBossBeating = false;
	}

	// ---- Tools

	private void DrawTools()
	{
		bool inWorld = FApp.World != null && FWorld.Player != null && FApp.World.CurrentRoom != null;
		GUILayout.Label("Overlays and logging", mHeaderStyle);
		PCDebug.ShowColliders = GUILayout.Toggle(PCDebug.ShowColliders, " Collider overlay (green unit, red wall, yellow trigger)");
		LogMoves = GUILayout.Toggle(LogMoves, " Log keyboard moves (issued / blocked)");
		LogAudio = GUILayout.Toggle(LogAudio, " Log playing music every 4 s");
		GUILayout.Space(6f);
		GUILayout.Label("Dumps to the player log (dist/logs/player-<os>.log)", mHeaderStyle);
		GUI.enabled = FApp.World != null;
		if (GUILayout.Button("Every room's tiles  (. passable, # wall, ~ no tile)"))
		{
			DumpRooms();
		}
		GUI.enabled = inWorld;
		if (GUILayout.Button("Tiles and units around the player (13 x 10)"))
		{
			DumpAroundPlayer();
		}
		if (GUILayout.Button("Guard sight-line rays from the player in four directions"))
		{
			CastGuardRays();
		}
		GUI.enabled = true;
		GUILayout.Space(6f);
		GUILayout.Label("Other", mHeaderStyle);
		GUI.enabled = FApp.PlayerData != null;
		if (GUILayout.Button("Switch to 1000 x 800 (resolution keep / revert dialog)"))
		{
			FApp.PlayerData.SetResolution(1000, 800, 60, false, true);
		}
		GUI.enabled = true;
		if (GUILayout.Button("Force a Discord presence update"))
		{
			PCDiscord.ForceUpdate();
		}
	}

	private static void DumpRooms()
	{
		for (int r = 0; r < FApp.World.Rooms.Count; r++)
		{
			FRoom room = FApp.World.Rooms[r];
			Rect b = room.ProtoRoom.Bounds;
			Debug.Log("PCRoom " + room.ProtoRoom.Name + " bounds " + b + " encounter=" + room.ProtoRoom.EnemyEncounterRoom);
			for (int ty = (int)(b.y + b.height); ty >= (int)b.y - 1; ty--)
			{
				string row = "PCRow y=" + ty.ToString("00") + " ";
				for (int tx = (int)b.x - 1; tx <= (int)(b.x + b.width); tx++)
				{
					FTile t = FApp.World.GetTile(new FPoint2(tx, ty));
					row += t == null ? "~" : (t.TerrainPassable ? "." : "#");
				}
				Debug.Log(row);
			}
		}
	}

	private static void DumpAroundPlayer()
	{
		FPoint2 pp = FWorld.Player.TilePosition;
		FRoom room = FApp.World.CurrentRoom;
		for (int ty = pp.y + 6; ty >= pp.y - 3; ty--)
		{
			string row = "PCDump y=" + ty + ":";
			for (int tx = pp.x - 6; tx <= pp.x + 6; tx++)
			{
				FTile t = room.GetTile(new FPoint2(tx, ty));
				if (t == null)
				{
					row += " ##";
					continue;
				}
				row += t.TerrainPassable ? (t.GetPassable(FWorld.Player, new List<int>()) ? " . " : " X ") : " # ";
			}
			Debug.Log(row);
		}
		for (int ty = pp.y + 6; ty >= pp.y - 3; ty--)
		{
			for (int tx = pp.x - 6; tx <= pp.x + 6; tx++)
			{
				FTile t = room.GetTile(new FPoint2(tx, ty));
				if (t == null)
				{
					continue;
				}
				for (int u = 0; u < t.Units.Count; u++)
				{
					Debug.Log("PCDump unit " + t.Units[u].ProtoUnit.Name + " @" + tx + "," + ty + " obstructs=" + t.Units[u].Obstructs);
				}
			}
		}
	}

	// cast the guard-cone ray from the player in the four directions and report every hit (the first non-passable tile stops a cone)
	private static void CastGuardRays()
	{
		Vector3 pp = FWorld.Player.Position;
		Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
		RaycastHit2D[] hits = new RaycastHit2D[20];
		for (int d = 0; d < 4; d++)
		{
			int n = Physics2D.RaycastNonAlloc(new Vector2(pp.x, pp.y), dirs[d], hits, 6f);
			string info = "PCRay " + dirs[d] + " from " + pp.x.ToString("0.0") + "," + pp.y.ToString("0.0") + " n=" + n;
			for (int h = 0; h < n; h++)
			{
				FTile t = FApp.World.GetTileFromGO(hits[h].collider.gameObject);
				FUnit u = FApp.World.GetUnitFromGO(hits[h].collider.gameObject);
				info += " [" + (t != null ? "tile " + t.Position.x + "," + t.Position.y + " pass=" + t.TerrainPassable : (u != null ? "unit " + u.ProtoUnit.Name : "other " + hits[h].collider.name)) + " d=" + hits[h].distance.ToString("0.00") + (hits[h].collider.isTrigger ? " trig" : "") + "]";
			}
			Debug.Log(info);
		}
	}

	private void LogPlayingAudio()
	{
		if (Time.unscaledTime < mNextAudioLog)
		{
			return;
		}
		mNextAudioLog = Time.unscaledTime + 4f;
		System.Text.StringBuilder sb = new System.Text.StringBuilder("PCAudio playing:");
		AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		for (int i = 0; i < sources.Length; i++)
		{
			if (sources[i].isPlaying && sources[i].clip != null && sources[i].clip.length > 15f)
			{
				sb.Append(' ').Append(sources[i].clip.name).Append('(').Append(sources[i].volume.ToString("0.00")).Append(sources[i].mute ? ",MUTED" : "").Append(')');
			}
		}
		Debug.Log(sb.ToString());
	}
}
