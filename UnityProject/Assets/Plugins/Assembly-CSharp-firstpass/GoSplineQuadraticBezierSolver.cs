using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoSplineQuadraticBezierSolver @ 0x16d8a28..0x16da210, Ghidra vaddr).
public class GoSplineQuadraticBezierSolver : AbstractGoSplineSolver
{
	public GoSplineQuadraticBezierSolver(List<Vector3> nodes)
	{
		_nodes = nodes;
	}

	protected float quadBezierLength(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint)
	{
		Vector3[] array = new Vector3[2];
		array[0] = controlPoint - startPoint;
		array[1] = startPoint - 2f * controlPoint + endPoint;
		if (array[1] != Vector3.zero)
		{
			float num = 4f * Vector3.Dot(array[0], array[0]);
			float num2 = 4f * Vector3.Dot(array[1], array[1]);
			float num3 = 8f * Vector3.Dot(array[1], array[0]);
			float num4 = 2f * num2 + num3;
			float num5 = num + num2 + num3;
			float num6 = Mathf.Sqrt(num5);
			float num7 = Mathf.Pow(num2, 1.5f);
			float num8 = Mathf.Sqrt(num2 * num5);
			float num9 = Mathf.Log(num4 + 2f * num8);
			float num10 = Mathf.Log(num3 + 2f * Mathf.Sqrt(num2 * num));
			return 0.25f / num2 * (num4 * num6 - num3 * Mathf.Sqrt(num)) + (num2 * num * 4f - num3 * num3) / (8f * num7) * (num9 - num10);
		}
		return 2f * array[0].magnitude;
	}

	public override void closePath()
	{
	}

	public override Vector3 getPoint(float t)
	{
		float num = 1f - t;
		return num * num * _nodes[0] + 2f * num * t * _nodes[1] + t * t * _nodes[2];
	}

	public override void drawGizmos()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(_nodes[0], _nodes[1]);
		Gizmos.DrawLine(_nodes[1], _nodes[2]);
		Gizmos.color = color;
	}
}
