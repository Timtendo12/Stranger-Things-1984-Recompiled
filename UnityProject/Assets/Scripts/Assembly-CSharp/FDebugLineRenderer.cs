using System.Collections.Generic;
using UnityEngine;

public class FDebugLineRenderer : MonoBehaviour
{
	protected GameObject mGO;

	protected List<FDebugLineRendererMeshObject> mMeshGOs;

	private const int cMaxVerts = 64000;

	protected static FDebugLineRenderer mTemp;

	public static FDebugLineRenderer Temp => null;

	public static FDebugLineRenderer Create(GameObject parent, string name)
	{
		return null;
	}

	public void Destroy()
	{
	}

	protected void AddMeshObject()
	{
	}

	protected FDebugLineRendererMeshObject GetOrCreateMeshObject(int newNumVerts)
	{
		return null;
	}

	protected FDebugLineRendererMeshObject GetMeshObjectForVertIndex(int vertIndex, out int localVertIndex)
	{
		localVertIndex = default;
		return null;
	}

	public void AddLine(Vector3 start, Vector3 end, Color startColor, Color endColor)
	{
	}

	public void AddLineStrip(Vector3[] points, Color color)
	{
	}

	public void AddCircle(Vector3 center, float radius, int numSegments, Color color, bool clampToTerrain)
	{
	}

	public void AddPoint(Vector3 point, float size, Color color, bool clampToTerrain)
	{
	}

	public void AddAABB(Bounds bounds, Color color)
	{
	}

	public void AddQuadXZ(float startX, float startZ, float endX, float endZ, Color color)
	{
	}

	public void ChangePos(int vertIndex, Vector3 pos)
	{
	}

	public void ChangeColorsAtRange(int startVertIndex, int numVerts, Color color)
	{
	}

	public void Clear()
	{
	}

	public void Update()
	{
	}

	public void Enable(bool enable)
	{
	}

	public bool IsEnabled()
	{
		return false;
	}
}
