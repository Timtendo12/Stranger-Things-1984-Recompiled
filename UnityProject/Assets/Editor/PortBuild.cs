using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// PC port build entry points. tools/build.py runs:
//   Unity -batchmode -executeMethod PortBuild.Build -portTarget windows|macos|linux|web -portOut <dir> [-portDev]
//         [-portMacArch universal|arm64|x64]
public static class PortBuild
{
	private const string ProductFile = "StrangerThings1984";

	private const string IconPath = "Assets/PCIcon/GameIcon.png";

	// Web build only: files moved for the duration of the build (project-relative source -> destination).
	// A destination outside Assets/ leaves the file out of the build. The CJK fonts are only used by
	// zh-cn / zh-tw (hidden on web) and are ~66 MB; WebGL cannot play a VideoClip, so the video is
	// streamed from StreamingAssets instead (FVideoPlayer).
	private static readonly string[,] WebMoves =
	{
		{ "Assets/Resources/fonts/NotoSansCJKsc-Regular.otf", "../.cache/web-excluded/NotoSansCJKsc-Regular.otf" },
		{ "Assets/Resources/fonts/NotoSansCJKsc-Bold.otf", "../.cache/web-excluded/NotoSansCJKsc-Bold.otf" },
		{ "Assets/Resources/fonts/NotoSansCJKtc-Regular.otf", "../.cache/web-excluded/NotoSansCJKtc-Regular.otf" },
		{ "Assets/Resources/fonts/NotoSansCJKtc-Bold.otf", "../.cache/web-excluded/NotoSansCJKtc-Bold.otf" },
		{ "Assets/Resources/video/ExclusiveVideo.mp4", "Assets/StreamingAssets/ExclusiveVideo.mp4" },
	};

	[MenuItem("Port/Build Windows (development)")]
	public static void BuildWindowsMenu() => Build("windows", DefaultOut("windows"), true);

	[MenuItem("Port/Build macOS (development)")]
	public static void BuildMacMenu() => Build("macos", DefaultOut("macos"), true);

	[MenuItem("Port/Build Linux (development)")]
	public static void BuildLinuxMenu() => Build("linux", DefaultOut("linux"), true);

	[MenuItem("Port/Build Web (development)")]
	public static void BuildWebMenu() => Build("web", DefaultOut("web"), true);

	public static void Build()
	{
		string[] args = Environment.GetCommandLineArgs();
		string target = Arg(args, "-portTarget") ?? "windows";
		string outDir = Arg(args, "-portOut") ?? DefaultOut(target);
		bool dev = Array.IndexOf(args, "-portDev") >= 0;
		string macArch = Arg(args, "-portMacArch") ?? "universal";
		Build(target, outDir, dev, macArch);
	}

	private static string DefaultOut(string target) => Path.GetFullPath(Path.Combine("..", "dist", target));

	private static string Arg(string[] args, string name)
	{
		int i = Array.IndexOf(args, name);
		return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
	}

	private static void Build(string target, string outDir, bool dev, string macArch = "universal")
	{
		RestoreWebMoves();
		BuildTarget buildTarget;
		BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
		string location;
		switch (target)
		{
			case "windows":
				buildTarget = BuildTarget.StandaloneWindows64;
				location = Path.Combine(outDir, ProductFile + ".exe");
				break;
			case "macos":
				buildTarget = BuildTarget.StandaloneOSX;
				location = Path.Combine(outDir, ProductFile + ".app");
				if (!SetMacArchitecture(macArch))
				{
					return;
				}
				break;
			case "linux":
				buildTarget = BuildTarget.StandaloneLinux64;
				location = Path.Combine(outDir, ProductFile + ".x86_64");
				break;
			case "web":
				buildTarget = BuildTarget.WebGL;
				targetGroup = BuildTargetGroup.WebGL;
				location = outDir;
				break;
			default:
				Fail($"PortBuild: unknown -portTarget '{target}' (use windows, macos, linux or web)");
				return;
		}
		if (!BuildPipeline.IsBuildTargetSupported(targetGroup, buildTarget))
		{
			Fail($"PortBuild: build support for {target} is not installed in this Unity editor. " +
				$"Add the '{ModuleName(target)}' module with Unity Hub (Installs > ... > Add modules).");
			return;
		}
		if (target == "web")
		{
			SetWebSettings();
		}
		else
		{
			PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
			PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
			PlayerSettings.defaultScreenWidth = 1280;
			PlayerSettings.defaultScreenHeight = 720;
			PlayerSettings.resizableWindow = true;
			PlayerSettings.allowFullscreenSwitch = false; // PCDisplay handles F11 and Alt+Enter
			PlayerSettings.usePlayerLog = true;
		}
		SetIcon();
		Directory.CreateDirectory(outDir);
		BuildPlayerOptions options = new BuildPlayerOptions
		{
			scenes = new[] { "Assets/Scenes/Default.unity" },
			locationPathName = location,
			target = buildTarget,
			targetGroup = targetGroup,
			options = dev ? BuildOptions.Development | BuildOptions.AllowDebugging : BuildOptions.None
		};
		BuildReport report;
		try
		{
			if (target == "web")
			{
				ApplyWebMoves();
			}
			report = BuildPipeline.BuildPlayer(options);
		}
		finally
		{
			RestoreWebMoves();
		}
		Debug.Log($"PortBuild: {report.summary.result} target={target} dev={dev} errors={report.summary.totalErrors} size={report.summary.totalSize} -> {location}");
		if (Application.isBatchMode)
		{
			EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
		}
	}

	// The default icon is what Unity puts in the Windows exe (and so the title bar and taskbar), the macOS .app and the
	// Linux window.
	private static void SetIcon()
	{
		Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
		if (icon == null)
		{
			Debug.LogWarning($"PortBuild: {IconPath} not found; building with Unity's default icon.");
			return;
		}
		PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new[] { icon }, IconKind.Any);
	}

	private static string ModuleName(string target)
	{
		switch (target)
		{
			case "macos": return "Mac Build Support (Mono)";
			case "linux": return "Linux Build Support (Mono)";
			case "web": return "Web Build Support";
			default: return "Windows Build Support (Mono)";
		}
	}

	// WebGL has no Mono. Gzip with the decompression fallback runs from any static web server without
	// special Content-Encoding headers. Stripping stays minimal because saves and config go through
	// XmlSerializer and reflection (see Assets/link.xml).
	private static void SetWebSettings()
	{
		PlayerSettings.SetScriptingBackend(NamedBuildTarget.WebGL, ScriptingImplementation.IL2CPP);
		PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL, ManagedStrippingLevel.Minimal);
		PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
		PlayerSettings.WebGL.decompressionFallback = true;
		PlayerSettings.WebGL.dataCaching = true;
		// the project settings come from the Android export, where every WebGL memory field is 0
		PlayerSettings.WebGL.initialMemorySize = 256;
		PlayerSettings.WebGL.memoryGrowthMode = WebGLMemoryGrowthMode.Geometric;
		PlayerSettings.WebGL.geometricMemoryGrowthStep = 0.2f;
		PlayerSettings.WebGL.memoryGeometricGrowthCap = 96;
		PlayerSettings.WebGL.maximumMemorySize = 4096;
		PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
		PlayerSettings.WebGL.template = "PROJECT:StrangerThings";
		PlayerSettings.defaultWebScreenWidth = 1280;
		PlayerSettings.defaultWebScreenHeight = 720;
		PlayerSettings.runInBackground = true;
	}

	private static void ApplyWebMoves()
	{
		for (int i = 0; i < WebMoves.GetLength(0); i++)
		{
			MoveWithMeta(WebMoves[i, 0], WebMoves[i, 1]);
		}
		AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
	}

	// Also run before every build, so files left behind by an interrupted web build come back.
	private static void RestoreWebMoves()
	{
		bool moved = false;
		for (int i = 0; i < WebMoves.GetLength(0); i++)
		{
			string original = WebMoves[i, 0];
			string parked = WebMoves[i, 1];
			if (File.Exists(original) || !File.Exists(parked))
			{
				continue;
			}
			Directory.CreateDirectory(Path.GetDirectoryName(original));
			File.Move(parked, original);
			File.Delete(parked + ".meta"); // generated by Unity while the file sat in StreamingAssets
			if (File.Exists(ParkedMeta(original)))
			{
				File.Delete(original + ".meta");
				File.Move(ParkedMeta(original), original + ".meta");
			}
			moved = true;
		}
		if (moved)
		{
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
		}
	}

	// The .meta is parked outside Assets/ (never next to a StreamingAssets copy, where Unity would read
	// it), so the asset keeps its GUID and import settings when it comes back.
	private static void MoveWithMeta(string from, string to)
	{
		if (!File.Exists(from))
		{
			Debug.LogWarning($"PortBuild: {from} not found, not moved.");
			return;
		}
		Directory.CreateDirectory(Path.GetDirectoryName(to));
		File.Move(from, to);
		if (File.Exists(from + ".meta"))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(ParkedMeta(from)));
			File.Delete(ParkedMeta(from));
			File.Move(from + ".meta", ParkedMeta(from));
		}
	}

	private static string ParkedMeta(string original) => "../.cache/web-excluded/meta/" + Path.GetFileName(original) + ".meta";

	// universal (Intel + Apple silicon, the default), arm64 (Apple silicon only) or x64 (Intel only;
	// runs under Rosetta on Apple silicon). The settings type lives in the Mac build-support module,
	// so it is looked up by name to keep this file compiling without that module.
	private static bool SetMacArchitecture(string macArch)
	{
		string value;
		switch (macArch)
		{
			case "universal": value = "x64ARM64"; break;
			case "arm64": value = "ARM64"; break;
			case "x64": value = "x64"; break;
			default:
				Fail($"PortBuild: unknown -portMacArch '{macArch}' (use universal, arm64 or x64)");
				return false;
		}
		Type t = Type.GetType("UnityEditor.OSXStandalone.UserBuildSettings, UnityEditor.OSXStandalone.Extensions");
		var prop = t?.GetProperty("architecture", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
		try
		{
			if (prop == null)
			{
				throw new MissingMemberException("UserBuildSettings.architecture not found");
			}
			prop.SetValue(null, Enum.Parse(prop.PropertyType, value));
			return true;
		}
		catch (Exception e)
		{
			if (macArch != "universal")
			{
				Fail($"PortBuild: could not select a {macArch} macOS build: " + e.Message);
				return false;
			}
			Debug.LogWarning("PortBuild: could not select a universal macOS build, using the editor default: " + e.Message);
			return true;
		}
	}

	private static void Fail(string message)
	{
		Debug.LogError(message);
		if (Application.isBatchMode)
		{
			EditorApplication.Exit(2);
		}
	}
}
