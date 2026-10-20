using System.Collections.Generic;
using UnityEngine;

public class FDebugCollisionVolumeRenderer : MonoBehaviour
{
	protected Dictionary<int, FDebugCollisionVolumeItem> mItems;

	protected GameObject mGO;

	private const float cSphereAlpha = 0.5f;

	public static FDebugCollisionVolumeRenderer Create(GameObject parent, string name)
	{
		return null;
	}

	public void Destroy()
	{
	}

	public bool IsEnabled()
	{
		return false;
	}

	public void Enable(bool enable)
	{
	}

	public void Update()
	{
	}

	protected GameObject MakeSphereGameObject(Color color)
	{
		return null;
	}
}
