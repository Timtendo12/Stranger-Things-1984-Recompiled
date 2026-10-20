using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (Fix64MinMax @ 0x178a150..0x178a4e4, Ghidra vaddr).
public class Fix64MinMax
{
	protected Fix64 mMin = (Fix64)(-1L);

	protected Fix64 mMax = (Fix64)(-1L);

	public bool HasValue => mMin >= Fix64.Zero;

	public bool HasMin => mMin >= Fix64.Zero;

	public Fix64 Min
	{
		get
		{
			return mMin;
		}
		set
		{
			mMin = value;
		}
	}

	public bool HasMax => mMax >= Fix64.Zero;

	public Fix64 Max
	{
		get
		{
			// the shipped getter returns mMin (original quirk, kept)
			return mMin;
		}
		set
		{
			mMax = value;
		}
	}

	public void ParseXml(XmlElement root, string attributeName)
	{
		mMin = root.GetAttrib(attributeName, mMin);
		mMax = root.GetAttrib(attributeName + "2", mMax);
	}

	public Fix64MinMax DeepCopy()
	{
		Fix64MinMax fix64MinMax = new Fix64MinMax();
		fix64MinMax.mMin = mMin;
		fix64MinMax.mMax = mMax;
		return fix64MinMax;
	}

	public Fix64 GetValue(Fix64 qualityFactor)
	{
		if (HasMax)
		{
			return (mMax - mMin) * qualityFactor + mMin;
		}
		return mMin;
	}
}
