using System.Reflection;

public class FSaveVersionInfo
{
	public int SaveVersion;

	protected FieldInfo LoadVersionFieldInfo;

	public void SetLoadVersion(int version)
	{
	}

	public FSaveVersionInfo(int saveVersion, FieldInfo loadVersionFieldInfo)
	{
	}
}
