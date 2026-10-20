public class FFixAABBNode
{
	public FFixAABB AABB;

	public FFixAABBNode Left;

	public FFixAABBNode Right;

	public int[] Indices;

	public FFixAABBNode()
	{
	}

	public FFixAABBNode(FFixAABBTreeMesh mesh, FixVector3[] vertices, int[] indices, int maxLeafTriangles, int maxTreeDepth, int depth, FFixAABB aabb = null)
	{
	}

	public void InitHeightfield(FFixAABBTreeMesh mesh, FixVector3[] vertices, int vertexCountX, int vertexCountZ, Fix64 tileSizeX, Fix64 tileSizeZ, int startX, int startZ, int endX, int endZ, int maxLeafTriangles, int maxTreeDepth, int depth)
	{
	}

	protected static FFixAABB GetAABB(FixVector3[] vertices, int[] indices)
	{
		return null;
	}

	protected void GetSplitForAxis(FAxis splitAxis, int[] triList, FixVector3[] vertices, int[] indices, out int[] leftIndices, out int[] rightIndices, out FFixAABB leftAABB, out FFixAABB rightAABB)
	{
		leftIndices = null;
		rightIndices = null;
		leftAABB = null;
		rightAABB = null;
	}

	public int GetMaxDepth(int depth = 0)
	{
		return 0;
	}

	public bool RayIntersect(FixVector3[] vertices, FixVector3 rayOrigin, FixVector3 rayDirectionNormalized, ref Fix64 t)
	{
		return false;
	}

	public static FFixAABBNode CreateFromBinReader(FBinReader reader)
	{
		return null;
	}

	public void BinWrite(FBinWriter writer)
	{
	}

	public void BinRead(FBinReader reader)
	{
	}
}
