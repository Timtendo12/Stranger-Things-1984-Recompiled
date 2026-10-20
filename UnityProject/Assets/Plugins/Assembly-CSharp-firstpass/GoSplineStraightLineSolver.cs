using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoSplineStraightLineSolver @ 0x16d89f0..0x16da8c4, Ghidra vaddr).
public class GoSplineStraightLineSolver : AbstractGoSplineSolver
{
	private Dictionary<int, float> _segmentStartLocations;

	private Dictionary<int, float> _segmentDistances;

	private int _currentSegment;

	public GoSplineStraightLineSolver(List<Vector3> nodes)
	{
		_nodes = nodes;
	}

	public override void buildPath()
	{
		if (_nodes.Count < 3)
		{
			return;
		}
		_segmentStartLocations = new Dictionary<int, float>(_nodes.Count - 2);
		_segmentDistances = new Dictionary<int, float>(_nodes.Count - 1);
		int i;
		for (i = 0; i < _nodes.Count - 1; i++)
		{
			float num = Vector3.Distance(_nodes[i], _nodes[i + 1]);
			_segmentDistances.Add(i, num);
			_pathLength += num;
		}
		float num2 = 0f;
		for (i = 0; i < _segmentDistances.Count - 1; i++)
		{
			num2 += _segmentDistances[i];
			_segmentStartLocations.Add(i + 1, num2 / _pathLength);
		}
	}

	public override void closePath()
	{
		if (!((_nodes[0] - _nodes[_nodes.Count - 1]).sqrMagnitude < 1E-10f))
		{
			_nodes.Add(_nodes[0]);
		}
	}

	public override Vector3 getPoint(float t)
	{
		return getPointOnPath(t);
	}

	public override Vector3 getPointOnPath(float t)
	{
		if (_nodes.Count < 3)
		{
			return Vector3.Lerp(_nodes[0], _nodes[1], t);
		}
		_currentSegment = 0;
		foreach (KeyValuePair<int, float> segmentStartLocation in _segmentStartLocations)
		{
			if (segmentStartLocation.Value < t)
			{
				_currentSegment = segmentStartLocation.Key;
				continue;
			}
			break;
		}
		float num = _pathLength * t;
		for (int num2 = _currentSegment; num2 > 0; num2--)
		{
			num -= _segmentDistances[num2 - 1];
		}
		return Vector3.Lerp(_nodes[_currentSegment], _nodes[_currentSegment + 1], Mathf.Clamp01(num / _segmentDistances[_currentSegment]));
	}
}
