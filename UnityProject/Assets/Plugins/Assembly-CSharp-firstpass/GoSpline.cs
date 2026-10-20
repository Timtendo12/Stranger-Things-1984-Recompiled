using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoSpline @ 0x16d1a7c..0x16d9080, Ghidra vaddr).
public class GoSpline
{
	private bool _isReversed;

	private AbstractGoSplineSolver _solver;

	public int currentSegment { get; private set; }

	public bool isClosed { get; private set; }

	public GoSplineType splineType { get; private set; }

	public List<Vector3> nodes => _solver.nodes;

	public GoSpline(List<Vector3> nodes, bool useStraightLines = false)
	{
		if (useStraightLines || nodes.Count == 2)
		{
			splineType = GoSplineType.StraightLine;
			_solver = new GoSplineStraightLineSolver(nodes);
		}
		else if (nodes.Count == 3)
		{
			splineType = GoSplineType.QuadraticBezier;
			_solver = new GoSplineQuadraticBezierSolver(nodes);
		}
		else if (nodes.Count == 4)
		{
			splineType = GoSplineType.CubicBezier;
			_solver = new GoSplineCubicBezierSolver(nodes);
		}
		else
		{
			splineType = GoSplineType.CatmullRom;
			_solver = new GoSplineCatmullRomSolver(nodes);
		}
	}

	public GoSpline(Vector3[] nodes, bool useStraightLines = false)
		: this(new List<Vector3>(nodes), useStraightLines)
	{
	}

	public GoSpline(string pathAssetName, bool useStraightLines = false)
		: this(nodeListFromAsset(pathAssetName), useStraightLines)
	{
	}

	private static List<Vector3> nodeListFromAsset(string pathAssetName)
	{
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			FLog.UnityError("The Web Player does not support loading files from disk.");
			return null;
		}
		if (!pathAssetName.EndsWith(".asset"))
		{
			pathAssetName += ".asset";
		}
		byte[] bytes;
		if (Application.platform == RuntimePlatform.Android)
		{
			// TODO(port): shipped Android path reads "jar:file://<dataPath>!/assets/<name>" through WWW; on PC the StreamingAssets file path below is used (0x16d8b90) [left: Android-only file access path]
			bytes = File.ReadAllBytes(Path.Combine(Path.Combine(Application.dataPath, "StreamingAssets"), pathAssetName));
		}
		else
		{
			string path = ((Application.platform == RuntimePlatform.IPhonePlayer) ? "Raw" : "StreamingAssets");
			bytes = File.ReadAllBytes(Path.Combine(Path.Combine(Application.dataPath, path), pathAssetName));
		}
		return bytesToVector3List(bytes);
	}

	public static List<Vector3> bytesToVector3List(byte[] bytes)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < bytes.Length; i += 12)
		{
			float x = BitConverter.ToSingle(bytes, i);
			float y = BitConverter.ToSingle(bytes, i + 4);
			float z = BitConverter.ToSingle(bytes, i + 8);
			list.Add(new Vector3(x, y, z));
		}
		return list;
	}

	public Vector3 getLastNode()
	{
		return _solver.nodes[_solver.nodes.Count - 1];
	}

	public void buildPath()
	{
		_solver.buildPath();
	}

	private Vector3 getPoint(float t)
	{
		return _solver.getPoint(t);
	}

	public Vector3 getPointOnPath(float t)
	{
		if (t < 0f || t > 1f)
		{
			if (isClosed)
			{
				t += ((t < 0f) ? 1f : (-1f));
			}
			else
			{
				t = Mathf.Clamp01(t);
			}
		}
		return _solver.getPointOnPath(t);
	}

	public void closePath()
	{
		if (!isClosed)
		{
			isClosed = true;
			_solver.closePath();
		}
	}

	public void reverseNodes()
	{
		if (!_isReversed)
		{
			_solver.reverseNodes();
			_isReversed = true;
		}
	}

	public void unreverseNodes()
	{
		if (_isReversed)
		{
			_solver.reverseNodes();
			_isReversed = false;
		}
	}

	public void drawGizmos(float resolution)
	{
		_solver.buildPath();
		_solver.drawGizmos();
		Vector3 from = _solver.getPoint(0f);
		float num = (float)_solver.nodes.Count * resolution;
		for (int i = 1; (float)i <= num; i++)
		{
			Vector3 point = _solver.getPoint((float)i / num);
			Gizmos.DrawLine(from, point);
			from = point;
		}
	}

	public static void drawGizmos(Vector3[] path, float resolution = 50f)
	{
		new GoSpline(path).drawGizmos(resolution);
	}
}
