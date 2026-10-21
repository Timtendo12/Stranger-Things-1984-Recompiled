using System;
using UnityEngine;

// PC port (new): developer mode, enabled by launching with -debug (or --debug), or index.html?debug on the web.
// Draws every active 2D physics collider through FDebugLineRenderer: green = unit, red = other solid collider
// (walls, obstructing tiles), yellow = trigger. PCInput (F1-F12 / Home test keys, move and audio logging) and
// PCDiscord (Insert forces a presence update, error logging) check PCDebug.Enabled too.
[DefaultExecutionOrder(200)]
public class PCDebug : MonoBehaviour
{
	public static readonly bool Enabled = HasArg("-debug") || HasArg("--debug");

	private static readonly Color UnitColor = new Color(0.2f, 1f, 0.2f, 0.9f);

	private static readonly Color SolidColor = new Color(1f, 0.25f, 0.25f, 0.9f);

	private static readonly Color TriggerColor = new Color(1f, 0.9f, 0.2f, 0.6f);

	private readonly Vector3[] mRect = new Vector3[5];

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Bootstrap()
	{
		if (!Application.isPlaying || !Enabled)
		{
			return;
		}
		GameObject go = new GameObject("PCDebug");
		DontDestroyOnLoad(go);
		go.AddComponent<PCDebug>();
	}

	private static bool HasArg(string name)
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		// the web build has no command line: index.html?debug enables it
		string url = Application.absoluteURL;
		int query = url.IndexOf('?');
		return query >= 0 && Array.IndexOf(url.Substring(query + 1).Split('&', '#'), name.TrimStart('-')) >= 0;
#else
		return Array.IndexOf(Environment.GetCommandLineArgs(), name) >= 0;
#endif
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
