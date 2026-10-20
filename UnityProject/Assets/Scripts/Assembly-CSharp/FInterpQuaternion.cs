using UnityEngine;

[SaveVersion(0)]
public class FInterpQuaternion
{
	protected Quaternion mLast;

	protected FixQuaternion mCurrent;

	public FixQuaternion Current
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public Quaternion Last => default;

	public FInterpQuaternion()
	{
	}

	public FInterpQuaternion(Quaternion last, FixQuaternion current)
	{
	}

	public Quaternion GetInterpolated(float t)
	{
		return default;
	}

	public static implicit operator FixQuaternion(FInterpQuaternion v)
	{
		return default;
	}

	public void SetImmediate(FixQuaternion v)
	{
	}

	public void Set(FixQuaternion v)
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
