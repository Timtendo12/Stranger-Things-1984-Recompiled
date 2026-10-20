using System.Collections.Generic;
using UnityEngine;

public class FShaderReplacementManager
{
	protected static Dictionary<Shader, Shader> mOpaqueToAlphaShaders;

	protected static string[,] cShaderOpaqueToAlphaNames;

	protected static string[] cStandardShaderFixupNames;

	protected static HashSet<Shader> cStandardShaderFixups;

	static FShaderReplacementManager()
	{
	}

	public static Shader FindAlphaReplacement(Shader opaqueShader)
	{
		return null;
	}

	public static bool NeedsStandardShaderFixup(Shader opaqueShader)
	{
		return false;
	}
}
