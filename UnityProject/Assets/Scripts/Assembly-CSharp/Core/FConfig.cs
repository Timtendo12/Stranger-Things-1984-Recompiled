public class FConfig : FConfigCore
{
	private static bool mMusic;

	public static bool BugButtonOpensForum;

	protected static int mGetScoresCacheSeconds;

	public static int mAntialias;

	public static int mBitDepth;

	public static bool UseSteam;

	public static float MatchmakingWaitTime;

	public static float JoinServerWaitTime;

	protected static float mConsoleAlpha;

	protected static float mConsoleHeight;

	protected static bool mAllConsoleOutput;

	public static bool EnableLeaderboards;

	public static string LeaderboardsUri;

	public static bool PNGScreenshots;

	public static string DefaultServerSteamID;

	public static bool FinalCrashDialog;

	public static bool DropZonePrototype;

	public static bool LoadGamePaused;

	protected static string mPlayerColor0;

	protected static string mPlayerColor1;

	protected static string mPlayerColor2;

	protected static string mPlayerColor3;

	protected static string mPlayerColorB0;

	protected static string mPlayerColorB1;

	protected static string mPlayerColorB2;

	protected static string mPlayerColorB3;

	protected static string mPlayerColor0UIColor1;

	protected static string mPlayerColor1UIColor1;

	protected static string mPlayerColor2UIColor1;

	protected static string mPlayerColor3UIColor1;

	protected static string mPlayerColor0UIColor2;

	protected static string mPlayerColor1UIColor2;

	protected static string mPlayerColor2UIColor2;

	protected static string mPlayerColor3UIColor2;

	public static bool BowtiesAreCool;

	public static bool EnsureDefeat;

	public static bool KillCam;

	public static int FacilitatorPort;

	public static string FacilitatorIP;

	public static int MasterServerPort;

	public static string MasterServerIP;

	public static bool AlwaysUseNAT;

	public static bool TestNewUnitSelection;

	public static string BugReportEmail;

	public static string BugReportBCC;

	public static string HackyMetaAddress;

	public static string GameServerAddress;

	public static int GameServerPort;

	public static string GameServerClientGuid;

	public static bool Pregame3DTest;

	public static int CommandQueueLimit;

	public static bool AttacksClearsAP;

	private static bool sMusic = true;

	public static bool Music
	{
		get
		{
			return sMusic;
		}
		set
		{
			sMusic = value;
		}
	}

	public static Fix64 AutoObjectDiffInterval { get; set; }

	public static bool LogFileOn { get; set; }

	public static bool ColorBlindFPSGraph { get; set; }

	public static string ForceLocale { get; set; }

	public static string ForceLanguage { get; set; }

	public static bool ShowRestart { get; set; }

	public static int GetScoresCacheSeconds
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public static int Antialias
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public static int BitDepth
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public static float ConsoleAlpha
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public static float ConsoleHeight
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public static bool AllConsoleOutput
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public static string PlayerColor0
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColor1
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColor2
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColor3
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColorB0
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColorB1
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColorB2
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColorB3
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColor0UIColor1
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColor1UIColor1
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColor2UIColor1
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColor3UIColor1
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColor0UIColor2
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColor1UIColor2
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColor2UIColor2
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public static string PlayerColor3UIColor2
	{
		get
		{
			return null;
		}
		set
		{
		}
	}
}
