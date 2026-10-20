using System;
using UnityEngine;

// PC port (new): launch with -debug (or --debug) to draw every active 2D physics collider through FDebugLineRenderer.
// Green = unit, red = other solid collider (walls, obstructing tiles), yellow = trigger.
[DefaultExecutionOrder(200)]
public class PCDebugOverlay : MonoBehaviour
{
	private static readonly Color UnitColor = new Color(0.2f, 1f, 0.2f, 0.9f);

	private static readonly Color SolidColor = new Color(1f, 0.25f, 0.25f, 0.9f);

	private static readonly Color TriggerColor = new Color(1f, 0.9f, 0.2f, 0.6f);

	private readonly Vector3[] mRect = new Vector3[5];

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Bootstrap()
	{
		string[] args = Environment.GetCommandLineArgs();
		if (!Application.isPlaying || (Array.IndexOf(args, "-debug") < 0 && Array.IndexOf(args, "--debug") < 0))
		{
			return;
		}
		GameObject go = new GameObject("PCDebugOverlay");
		DontDestroyOnLoad(go);
		go.AddComponent<PCDebugOverlay>();
	}

	private void LateUpdate()
	{
		FDebugLineRenderer lines = FDebugLineRenderer.Temp;
		lines.Clear();
		FWorld world = FApp.World;
		Collider2D[] colliders = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
		for (int i = 0; i < colliders.Length; i++)
		{
			Collider2D c = colliders[i];
			if (!c.enabled || !c.gameObject.activeInHierarchy)
			{
				continue;
			}
			Color color = c.isTrigger ? TriggerColor : (world != null && world.GetUnitFromGO(c.gameObject) != null ? UnitColor : SolidColor);
			AddRect(lines, c.bounds, color);
		}
		// Draw on top of the sprites (they share the transparent queue).
		foreach (MeshRenderer r in lines.GetComponentsInChildren<MeshRenderer>(true))
		{
			r.sortingOrder = short.MaxValue;
		}
	}

	private void AddRect(FDebugLineRenderer lines, Bounds b, Color color)
	{
		float z = b.center.z;
		mRect[0] = new Vector3(b.min.x, b.min.y, z);
		mRect[1] = new Vector3(b.max.x, b.min.y, z);
		mRect[2] = new Vector3(b.max.x, b.max.y, z);
		mRect[3] = new Vector3(b.min.x, b.max.y, z);
		mRect[4] = mRect[0];
		lines.AddLineStrip(mRect, color);
	}
}
