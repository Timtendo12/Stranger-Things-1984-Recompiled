public class FFixTransform
{
	protected FixMatrix4x4 mMatrix;

	protected FixVector3 mPosition;

	protected FixQuaternion mRotation;

	protected FixVector3 mScale;

	protected bool mDecomposed;

	public FixMatrix4x4 Matrix
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public FixVector3 Position => default;

	public FixQuaternion Rotation => default;

	public FixVector3 Scale => default;

	public FFixTransform()
	{
	}

	public FFixTransform(FixMatrix4x4 matrix)
	{
	}

	public static FFixTransform CreateFromBinReader(FBinReader reader)
	{
		return null;
	}

	public void BinWrite(FBinWriter writer)
	{
	}

	public void BinRead(FBinReader reader)
	{
	}

	public void Decompose()
	{
	}
}
