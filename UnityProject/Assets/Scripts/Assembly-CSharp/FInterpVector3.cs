using UnityEngine;

[SaveVersion(0)]
public class FInterpVector3
{
	protected Vector3 mLast;

	protected FixVector3 mCurrent;

	public FixVector3 Current
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public Vector3 Last => default;

	public Fix64 x
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public Fix64 y
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public Fix64 z
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public FInterpVector3()
	{
	}

	public FInterpVector3(Vector3 last, FixVector3 current)
	{
	}

	public Vector3 GetInterpolated(float t)
	{
		return default;
	}

	public static implicit operator FixVector3(FInterpVector3 v)
	{
		return default;
	}

	public void SetImmediate(FixVector3 v)
	{
	}

	public void Set(FixVector3 v)
	{
	}

	public void Advance()
	{
	}

	public void Save(FBinWriter writer)
	{
	}

	public bool Load(FBinReader reader)
	{
		return false;
	}
}
