using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Rewritten from libil2cpp.so 1.0.605 (FDebugLineRendererMeshObject @ 0x16f6a90..0x16f848c, Ghidra vaddr).
public class FDebugLineRendererMeshObject
{
	protected List<Vector3> mPoints;

	protected List<Color> mColors;

	protected List<int> mIndices;

	protected bool mDirty;

	protected GameObject mGO;

	public int NumVerts => mPoints.Count;

	public FDebugLineRendererMeshObject(GameObject parent)
	{
		GameObject go = new GameObject("DebugLineMesh", typeof(MeshFilter), typeof(MeshRenderer));
		go.transform.parent = parent.transform;
		mGO = go;
		Mesh mesh = new Mesh();
		MeshFilter filter = go.GetComponent<MeshFilter>();
		if (filter != null)
		{
			filter.sharedMesh = mesh;
		}
		MeshRenderer renderer = go.GetComponent<MeshRenderer>();
		Material material = new Material(Shader.Find("VertexColorOnly"));
		if (renderer != null)
		{
			renderer.sharedMaterial = material;
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
		}
		mPoints = new List<Vector3>();
		mColors = new List<Color>();
		mIndices = new List<int>();
		mDirty = false;
	}

	public void Destroy()
	{
		if (mGO == null)
		{
			return;
		}
		MeshFilter filter = mGO.GetComponent<MeshFilter>();
		if (filter != null && filter.sharedMesh != null)
		{
			Object.Destroy(filter.sharedMesh);
			filter.sharedMesh = null;
		}
		Object.Destroy(mGO);
		mGO = null;
	}

	public void AddLine(Vector3 start, Vector3 end, Color startColor, Color endColor)
	{
		int first = mPoints.Count;
		mPoints.Add(start);
		mPoints.Add(end);
		mColors.Add(startColor);
		mColors.Add(endColor);
		mIndices.Add(first);
		mIndices.Add(first + 1);
		mDirty = true;
	}

	public void AddLineStrip(Vector3[] points, Color color)
	{
		if (points.Length < 2)
		{
			FLog.UnityError("Invalid AddLineStrip length");
			return;
		}
		int index = mPoints.Count;
		for (int i = 0; i < points.Length; i++)
		{
			mPoints.Add(points[i]);
			mColors.Add(color);
			if (i != 0)
			{
				mIndices.Add(index);
				index++;
				mIndices.Add(index);
			}
		}
		mDirty = true;
	}

	public void ChangePos(int vertIndex, Vector3 pos)
	{
		if (vertIndex < mPoints.Count)
		{
			mPoints[vertIndex] = pos;
			mDirty = true;
		}
	}

	// The original stops one short of the last vertex in the list; kept as shipped.
	public void ChangeColorsAtRange(int startVertIndex, int numVerts, Color color)
	{
		int count = mColors.Count;
		if (startVertIndex >= count)
		{
			return;
		}
		int end = Mathf.Min(startVertIndex + numVerts, count - 1);
		for (int i = startVertIndex; i < end; i++)
		{
			mColors[i] = color;
		}
		mDirty = true;
	}

	public void Clear()
	{
		mPoints.Clear();
		mColors.Clear();
		mIndices.Clear();
		mDirty = true;
	}

	public void Update()
	{
		if (!mDirty || mGO == null)
		{
			return;
		}
		MeshFilter filter = mGO.GetComponent<MeshFilter>();
		if (filter == null || filter.sharedMesh == null)
		{
			return;
		}
		Mesh mesh = filter.sharedMesh;
		mesh.Clear();
		mesh.vertices = mPoints.ToArray();
		mesh.colors = mColors.ToArray();
		mesh.SetIndices(mIndices.ToArray(), MeshTopology.Lines, 0);
		mDirty = false;
	}
}
