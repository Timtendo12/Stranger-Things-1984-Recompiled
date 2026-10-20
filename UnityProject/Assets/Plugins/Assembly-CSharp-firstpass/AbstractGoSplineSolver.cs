using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (AbstractGoSplineSolver @ 0x016d8560..0x016d89a8, Ghidra vaddr).
public abstract class AbstractGoSplineSolver
{
	protected List<Vector3> _nodes;

	protected float _pathLength;

	protected int totalSubdivisionsPerNodeForLookupTable = 5;

	protected Dictionary<float, float> _segmentTimeForDistance;

	public List<Vector3> nodes => _nodes;

	public virtual void buildPath()
	{
		int totalSubdivisions = totalSubdivisionsPerNodeForLookupTable * _nodes.Count;
		_pathLength = 0f;
		_segmentTimeForDistance = new Dictionary<float, float>(totalSubdivisions);
		Vector3 previous = getPoint(0f);
		for (int i = 1; i < totalSubdivisions + 1; i++)
		{
			float t = 1f / (float)totalSubdivisions * (float)i;
			Vector3 current = getPoint(t);
			_pathLength += Vector3.Distance(current, previous);
			previous = current;
			_segmentTimeForDistance.Add(t, _pathLength);
		}
	}

	public virtual Vector3 getPointOnPath(float t)
	{
		float pathLength = _pathLength;
		float targetDistance = pathLength * t;
		float previousTime = 0f;
		float nextTime = 0f;
		float nextDistance = 0f;
		float previousDistance = 0f;
		foreach (KeyValuePair<float, float> pair in _segmentTimeForDistance)
		{
			float key = pair.Key;
			nextTime = key;
			nextDistance = pair.Value;
			if (!(nextDistance < targetDistance))
			{
				if (previousTime > 0f)
				{
					previousDistance = _segmentTimeForDistance[previousTime];
				}
				return getPoint(previousTime + (nextTime - previousTime) * ((targetDistance - previousDistance) / (nextDistance - previousDistance)));
			}
			previousTime = key;
		}
		// Dictionary exhausted without reaching the target distance (t > 1): shipped code falls through with zeros.
		return getPoint(previousTime + (0f - previousTime) * ((targetDistance - 0f) / (0f - 0f)));
	}

	public abstract void closePath();

	public abstract Vector3 getPoint(float t);

	public void reverseNodes()
	{
		_nodes.Reverse();
	}

	public virtual void drawGizmos()
	{
	}
}
