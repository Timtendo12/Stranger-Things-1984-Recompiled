using System.Collections.Generic;
using UnityEngine;

public class FDebugLineRendererMeshObject
{
	protected List<Vector3> mPoints;

	protected List<Color> mColors;

	protected List<int> mIndices;

	protected bool mDirty;

	protected GameObject mGO;

	public int NumVerts => 0;

	public FDebugLineRendererMeshObject(GameObject parent)
	{
	}

	public void Destroy()
	{
	}

	public void AddLine(Vector3 start, Vector3 end, Color startColor, Color endColor)
	{
	}

	public void AddLineStrip(Vector3[] points, Color color)
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
}
