using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FDebugLineRenderer @ 0x16f6514..0x16f859c, Ghidra vaddr).
// Developer overlay: batches colored lines into "DebugLineMesh" children, 64000 verts per mesh.
public class FDebugLineRenderer : MonoBehaviour
{
	protected GameObject mGO;

	protected List<FDebugLineRendererMeshObject> mMeshGOs = new List<FDebugLineRendererMeshObject>();

	private const int cMaxVerts = 64000;

	protected static FDebugLineRenderer mTemp;

	public static FDebugLineRenderer Temp
	{
		get
		{
			if (mTemp == null)
			{
				mTemp = Create(null, "Temp");
				mTemp.Enable(true);
			}
			return mTemp;
		}
	}

	public static FDebugLineRenderer Create(GameObject parent, string name)
	{
		GameObject go = new GameObject(name, typeof(FDebugLineRenderer));
		Object.DontDestroyOnLoad(go);
		if (parent != null)
		{
			go.transform.parent = parent.transform;
		}
		FDebugLineRenderer renderer = go.GetComponent<FDebugLineRenderer>();
		if (renderer != null)
		{
			renderer.mGO = go;
		}
		renderer.AddMeshObject();
		renderer.Enable(false);
		return renderer;
	}

	public void Destroy()
	{
		for (int i = 0; i < mMeshGOs.Count; i++)
		{
			mMeshGOs[i].Destroy();
		}
		if (mGO != null)
		{
			Object.Destroy(mGO);
			mGO = null;
		}
	}

	protected void AddMeshObject()
	{
		mMeshGOs.Add(new FDebugLineRendererMeshObject(mGO));
	}

	protected FDebugLineRendererMeshObject GetOrCreateMeshObject(int newNumVerts)
	{
		for (int i = 0; i < mMeshGOs.Count; i++)
		{
			if (mMeshGOs[i].NumVerts + newNumVerts < cMaxVerts)
			{
				return mMeshGOs[i];
			}
		}
		AddMeshObject();
		return mMeshGOs[mMeshGOs.Count - 1];
	}

	protected FDebugLineRendererMeshObject GetMeshObjectForVertIndex(int vertIndex, out int localVertIndex)
	{
		localVertIndex = -1;
		int firstVert = 0;
		for (int i = 0; i < mMeshGOs.Count; i++)
		{
			int numVerts = mMeshGOs[i].NumVerts;
			if (vertIndex < firstVert + numVerts)
			{
				localVertIndex = vertIndex - firstVert;
				return mMeshGOs[i];
			}
			firstVert += numVerts;
		}
		return null;
	}

	public void AddLine(Vector3 start, Vector3 end, Color startColor, Color endColor)
	{
		GetOrCreateMeshObject(2).AddLine(start, end, startColor, endColor);
	}

	public void AddLineStrip(Vector3[] points, Color color)
	{
		GetOrCreateMeshObject(points.Length).AddLineStrip(points, color);
	}

	// Empty in the shipped build.
	public void AddCircle(Vector3 center, float radius, int numSegments, Color color, bool clampToTerrain)
	{
	}

	public void AddPoint(Vector3 point, float size, Color color, bool clampToTerrain)
	{
		float half = size * 0.5f;
		Vector3 up = Vector3.up * half;
		Vector3 left = Vector3.left * half;
		Vector3 forward = Vector3.forward * half;
		AddLine(point + up, point - up, color, color);
		AddLine(point + left, point - left, color, color);
		AddLine(point + forward, point - forward, color, color);
	}

	public void AddAABB(Bounds bounds, Color color)
	{
		Vector3 min = bounds.center - bounds.extents;
		Vector3 max = bounds.center + bounds.extents;
		// Bottom face, four verticals, top face.
		AddLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, min.y, max.z), color, color);
		AddLine(new Vector3(min.x, min.y, max.z), new Vector3(max.x, min.y, max.z), color, color);
		AddLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, min.y, min.z), color, color);
		AddLine(new Vector3(max.x, min.y, min.z), new Vector3(min.x, min.y, min.z), color, color);
		AddLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, max.y, min.z), color, color);
		AddLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, max.y, max.z), color, color);
		AddLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, max.y, max.z), color, color);
		AddLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z), color, color);
		AddLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z), color, color);
		AddLine(new Vector3(min.x, max.y, max.z), new Vector3(max.x, max.y, max.z), color, color);
		AddLine(new Vector3(max.x, max.y, max.z), new Vector3(max.x, max.y, min.z), color, color);
		AddLine(new Vector3(max.x, max.y, min.z), new Vector3(min.x, max.y, min.z), color, color);
	}

	public void AddQuadXZ(float startX, float startZ, float endX, float endZ, Color color)
	{
		AddLineStrip(new Vector3[]
		{
			new Vector3(startX, 0f, startZ),
			new Vector3(endX, 0f, startZ),
			new Vector3(endX, 0f, endZ),
			new Vector3(startX, 0f, endZ),
			new Vector3(startX, 0f, startZ)
		}, color);
	}

	public void ChangePos(int vertIndex, Vector3 pos)
	{
		GetMeshObjectForVertIndex(vertIndex, out int localVertIndex)?.ChangePos(localVertIndex, pos);
	}

	public void ChangeColorsAtRange(int startVertIndex, int numVerts, Color color)
	{
		for (int i = startVertIndex; i < startVertIndex + numVerts; i++)
		{
			GetMeshObjectForVertIndex(i, out int localVertIndex)?.ChangeColorsAtRange(localVertIndex, 1, color);
		}
	}

	public void Clear()
	{
		for (int i = 0; i < mMeshGOs.Count; i++)
		{
			mMeshGOs[i].Clear();
		}
	}

	public void Update()
	{
		for (int i = 0; i < mMeshGOs.Count; i++)
		{
			mMeshGOs[i].Update();
		}
	}

	public void Enable(bool enable)
	{
		if (mGO != null)
		{
			mGO.SetActive(enable);
		}
	}

	public bool IsEnabled()
	{
		return mGO != null && mGO.activeSelf;
	}
}
