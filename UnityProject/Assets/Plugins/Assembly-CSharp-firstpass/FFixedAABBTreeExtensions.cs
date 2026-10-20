using System;
using UnityEngine;

public static class FFixedAABBTreeExtensions
{
	public static void DrawGizmos(this FFixAABBTreeMesh m)
	{
	}

	public static void DrawGizmos(this FFixAABBNode n, FixVector3[] vertices, int maxDepth, int depth, FFixAABBNode parent)
	{
	}

	public static FFixAABBTreeMesh CreateFromMesh(Mesh mesh, int maxLeafTriangles, int maxTreeDepth, Action<float> progressCallback = null)
	{
		return null;
	}
}
