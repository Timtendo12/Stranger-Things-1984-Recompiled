public class FFixAABB
{
	protected FixVector3 mMin;

	protected FixVector3 mMax;

	public static readonly Fix64 cRayAABBEpsilon;

	public static readonly FixVector3 cMinusOneVec;

	public FixVector3 Min
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public FixVector3 Max
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public FFixAABB()
	{
	}

	public FFixAABB(FixVector3 min, FixVector3 max)
	{
	}

	public FFixAABB(FFixAABB a, FFixAABB b)
	{
	}

	public FFixAABB(FixVector3 v)
	{
	}

	public FixVector3 GetCenter()
	{
		return default;
	}

	public FixVector3 GetSize()
	{
		return default;
	}

	public bool Intersects(FFixAABB b)
	{
		return false;
	}

	public void Include(FixVector3 v)
	{
	}

	public Fix64 GetVolume()
	{
		return default;
	}

	public Fix64 GetOverlapVolume(FFixAABB b)
	{
		return default;
	}

	public Fix64 GetMaxOverlapArea(FFixAABB b)
	{
		return default;
	}

	public Fix64 GetOverlapArea(FAxis axis, FFixAABB b)
	{
		return default;
	}

	public bool Contains(FixVector3 p)
	{
		return false;
	}

	public bool ContainsTriangleApprox(FixVector3 p1, FixVector3 p2, FixVector3 p3)
	{
		return false;
	}

	public bool ContainsTriangleExact(FixVector3 p1, FixVector3 p2, FixVector3 p3, out FClipCode orCode)
	{
		orCode = default;
		return false;
	}

	public FClipCode GetClipCode(FixVector3 p)
	{
		return FClipCode.None;
	}

	public FClipCode GetClipCode(FixVector3 p1, FixVector3 p2, FixVector3 p3, out FClipCode andCode)
	{
		andCode = default;
		return FClipCode.None;
	}

	public void Clamp(FFixAABB aabb)
	{
	}

	public bool RayIntersect(FixVector3 rayOrigin, FixVector3 rayDirectionNormalized, ref FixVector3 intersectionPoint)
	{
		return false;
	}

	public bool RayIntersect(FixVector3 rayOrigin, FixVector3 rayDirectionNormalized)
	{
		return false;
	}

	public bool RayIntersect(FixVector3 rayOrigin, FixVector3 rayDirectionNormalized, out Fix64 tNear)
	{
		tNear = default;
		return false;
	}

	public bool IntersectLineSegmentPointDir(FixVector3 p1, FixVector3 directionNormalized, Fix64 dist, ref FixVector3 intersectionPoint)
	{
		return false;
	}

	public FAxis GetLongestAxis(out Fix64 length)
	{
		length = default;
		return FAxis.X;
	}

	public void Split(FAxis axis, out FFixAABB b1, out FFixAABB b2)
	{
		b1 = null;
		b2 = null;
	}

	public static FFixAABB CreateFromBinReader(FBinReader reader)
	{
		return null;
	}

	public void BinWrite(FBinWriter writer)
	{
	}

	public void BinRead(FBinReader reader)
	{
	}
}
