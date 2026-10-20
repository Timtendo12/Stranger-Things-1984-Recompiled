using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoDummyPath @ 0x016d19e8..0x016d1eac, Ghidra vaddr).
public class GoDummyPath : MonoBehaviour
{
	public string pathName = string.Empty;

	public Color pathColor = Color.magenta;

	public List<Vector3> nodes = new List<Vector3>
	{
		Vector3.zero,
		Vector3.zero
	};

	public bool useStandardHandles;

	public bool forceStraightLinePath;

	public int pathResolution = 50;

	public void OnDrawGizmos()
	{
		if (!forceStraightLinePath)
		{
			GoSpline goSpline = new GoSpline(nodes);
			Gizmos.color = pathColor;
			goSpline.drawGizmos(pathResolution);
		}
	}
}
