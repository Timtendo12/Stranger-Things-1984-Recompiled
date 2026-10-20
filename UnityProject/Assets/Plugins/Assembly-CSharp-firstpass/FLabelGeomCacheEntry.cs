using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLabelGeomCacheEntry @ 0x16ad224..0x16ad45c, Ghidra vaddr).
public class FLabelGeomCacheEntry
{
	public List<Vector3> Vertices = new List<Vector3>();

	public List<Color> Colors = new List<Color>();

	public List<Vector4> Colors2 = new List<Vector4>();

	public List<Vector2> UVs = new List<Vector2>();

	public List<Vector2> UV2 = new List<Vector2>();

	public List<Vector2> UV3 = new List<Vector2>();

	public List<Vector4> UV4 = new List<Vector4>();

	public int VertexCount => Vertices.Count;
}
