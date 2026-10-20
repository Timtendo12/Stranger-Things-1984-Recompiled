using System.Collections.Generic;

public class DeferredDecalSystem
{
	private static DeferredDecalSystem m_Instance;

	internal Dictionary<Decal.Kind, HashSet<Decal>> mDecalLists;

	public static DeferredDecalSystem instance => null;

	public void AddDecal(Decal d)
	{
	}

	public void RemoveDecal(Decal d)
	{
	}
}
