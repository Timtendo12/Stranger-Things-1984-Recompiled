using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// PC port build entry points. tools/build.py runs:
//   Unity -batchmode -executeMethod PortBuild.Build -portTarget windows|macos|linux -portOut <dir> [-portDev]
public static class PortBuild
{
	private const string ProductFile = "StrangerThings1984";

	[MenuItem("Port/Build Windows (development)")]
	public static void BuildWindowsMenu() => Build("windows", DefaultOut("windows"), true);

	[MenuItem("Port/Build macOS (development)")]
	public static void BuildMacMenu() => Build("macos", DefaultOut("macos"), true);

	[MenuItem("Port/Build Linux (development)")]
	public static void BuildLinuxMenu() => Build("linux", DefaultOut("linux"), true);

	public static void Build()
	{
		string[] args = Environment.GetCommandLineArgs();
		string target = Arg(args, "-portTarget") ?? "windows";
		string outDir = Arg(args, "-portOut") ?? DefaultOut(target);
		bool dev = Array.IndexOf(args, "-portDev") >= 0;
		Build(target, outDir, dev);
	}

	private static string DefaultOut(string target) => Path.GetFullPath(Path.Combine("..", "dist", target));

	private static string Arg(string[] args, string name)
	{
		int i = Array.IndexOf(args, name);
		return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
	}

	private static void Build(string target, string outDir, bool dev)
	{
		BuildTarget buildTarget;
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
				SetMacUniversal();
				break;
			case "linux":
				buildTarget = BuildTarget.StandaloneLinux64;
				location = Path.Combine(outDir, ProductFile + ".x86_64");
				break;
			default:
				Fail($"PortBuild: unknown -portTarget '{target}' (use windows, macos or linux)");
				return;
		}
		if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, buildTarget))
		{
			Fail($"PortBuild: build support for {target} is not installed in this Unity editor. " +
				$"Add the '{ModuleName(target)}' module with Unity Hub (Installs > ... > Add modules).");
			return;
		}
		PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
		PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
		PlayerSettings.defaultScreenWidth = 1280;
		PlayerSettings.defaultScreenHeight = 720;
		PlayerSettings.resizableWindow = true;
		PlayerSettings.allowFullscreenSwitch = false; // PCDisplay handles F11 and Alt+Enter
		PlayerSettings.usePlayerLog = true;
		Directory.CreateDirectory(outDir);
		BuildPlayerOptions options = new BuildPlayerOptions
		{
			scenes = new[] { "Assets/Scenes/Default.unity" },
			locationPathName = location,
			target = buildTarget,
			targetGroup = BuildTargetGroup.Standalone,
			options = dev ? BuildOptions.Development | BuildOptions.AllowDebugging : BuildOptions.None
		};
		BuildReport report = BuildPipeline.BuildPlayer(options);
		Debug.Log($"PortBuild: {report.summary.result} target={target} dev={dev} errors={report.summary.totalErrors} size={report.summary.totalSize} -> {location}");
		if (Application.isBatchMode)
		{
			EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
		}
	}

	private static string ModuleName(string target)
	{
		switch (target)
		{
			case "macos": return "Mac Build Support (Mono)";
			case "linux": return "Linux Build Support (Mono)";
			default: return "Windows Build Support (Mono)";
		}
	}

	// Universal (Intel + Apple silicon) app. The settings type lives in the Mac build-support
	// module, so it is looked up by name to keep this file compiling without that module.
	private static void SetMacUniversal()
	{
		Type t = Type.GetType("UnityEditor.OSXStandalone.UserBuildSettings, UnityEditor.OSXStandalone.Extensions");
		var prop = t?.GetProperty("architecture", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
		if (prop == null)
		{
			Debug.LogWarning("PortBuild: macOS architecture setting not found; using the editor default.");
			return;
		}
		try
		{
			prop.SetValue(null, Enum.Parse(prop.PropertyType, "x64ARM64"));
		}
		catch (Exception e)
		{
			Debug.LogWarning("PortBuild: could not select a universal macOS build: " + e.Message);
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
