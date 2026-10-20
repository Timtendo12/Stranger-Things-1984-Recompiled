using System;

public class SaveVersion : Attribute
{
	public int Version;

	public SaveVersion(int version)
	{
	}
}
