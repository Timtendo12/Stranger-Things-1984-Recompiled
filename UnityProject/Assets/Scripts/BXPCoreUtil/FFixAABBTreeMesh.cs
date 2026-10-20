using System;

public class FFixAABBTreeMesh
{
	protected FixVector3[] mVertices;

	protected FFixAABBNode mRoot;

	protected Action<float> mProgressCallback;

	protected int mIndexCount;

	protected int mIndicesAdded;

	protected bool mSimpleXZSplit;

	public FixVector3[] Vertices => null;

	public FFixAABBNode Root => null;

	public bool SimpleXZSplit => false;

	public FFixAABBTreeMesh()
	{
	}

	public FFixAABBTreeMesh(FixVector3[] vertices, int[] indices, int maxLeafTriangles, int maxTreeDepth, bool simpleXZSplit, Action<float> progressCallback = null)
	{
	}

	public void Init(FixVector3[] vertices, int[] indices, int maxLeafTriangles, int maxTreeDepth, bool simpleXZSplit, Action<float> progressCallback = null)
	{
	}

	public static FFixAABBTreeMesh Create(FixVector3[] vertices, int[] indices, int maxLeafTriangles, int maxTreeDepth, bool simpleXZSplit, Action<float> progressCallback = null)
	{
		return null;
	}

	public static FFixAABBTreeMesh CreateFromHeightfield(FixVector3[] vertices, int vertexCountX, int vertexCountZ, Fix64 tileSizeX, Fix64 tileSizeZ, int maxLeafTriangles, int maxTreeDepth, Action<float> progressCallback = null)
	{
		return null;
	}

	public void InitHeightfield(FixVector3[] vertices, int vertexCountX, int vertexCountZ, Fix64 tileSizeX, Fix64 tileSizeZ, int maxLeafTriangles, int maxTreeDepth, Action<float> progressCallback = null)
	{
	}

	public static FFixAABBTreeMesh CreateFromBinReader(FBinReader reader)
	{
		return null;
	}

	public void BinWrite(FBinWriter writer)
	{
	}

	public void BinRead(FBinReader reader)
	{
	}

	public bool RayIntersect(FixVector3 pos, FixQuaternion rotation, FixVector3 scale, FixVector3 rayOrigin, FixVector3 rayDirectionNormalized, ref FixVector3 intersectionPoint)
	{
		return false;
	}

	public bool RayIntersect(FFixTransform transform, FixVector3 rayOrigin, FixVector3 rayDirectionNormalized, ref FixVector3 intersectionPoint)
	{
		return false;
	}

	public bool RayIntersect(FixMatrix4x4 localToWorld, FixVector3 rayOrigin, FixVector3 rayDirectionNormalized, ref FixVector3 intersectionPoint)
	{
		return false;
	}

	public bool RayIntersect(FixVector3 rayOrigin, FixVector3 rayDirectionNormalized, ref FixVector3 intersectionPoint)
	{
		return false;
	}

	public bool RayIntersect(FixVector3 rayOrigin, FixVector3 rayDirectionNormalized, ref Fix64 t)
	{
		return false;
	}

	public void NodeProgress(int indexCount)
	{
	}
}
