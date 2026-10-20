using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FTextParams @ 0x1688450, 0x16886b8, 0x168b9cc..0x168ba54, Ghidra vaddr).
public class FTextParams
{
	public float scaledLineHeightOffset;

	public float scaledKerningOffset;

	protected float mPointSize = 1f;

	protected float mClipWidth = -1f;

	protected float mClipHeight = -1f;

	protected bool mWrap;

	protected int mWrapAddSpaces;

	private float _lineHeightOffset;

	private float _kerningOffset;

	public float kerningOffset
	{
		get
		{
			return _kerningOffset;
		}
		set
		{
			_kerningOffset = value;
			scaledKerningOffset = value;
		}
	}

	public float lineHeightOffset
	{
		get
		{
			return _lineHeightOffset;
		}
		set
		{
			_lineHeightOffset = value;
			scaledLineHeightOffset = value;
		}
	}

	public float PointSize
	{
		get
		{
			return mPointSize;
		}
		set
		{
			mPointSize = value;
		}
	}

	public float ClipWidth
	{
		get
		{
			return mClipWidth;
		}
		set
		{
			mClipWidth = value;
		}
	}

	public float ClipHeight
	{
		get
		{
			return mClipHeight;
		}
		set
		{
			mClipHeight = value;
		}
	}

	public bool Wrap
	{
		get
		{
			return mWrap;
		}
		set
		{
			mWrap = value;
		}
	}

	public int WrapAddSpaces
	{
		get
		{
			return mWrapAddSpaces;
		}
		set
		{
			mWrapAddSpaces = value;
		}
	}

	public FHermiteSpline<Vector2> Spline { get; set; }

	public FTextParams()
	{
	}

	public FTextParams(float pointSize, float lho = 0f)
	{
		mPointSize = pointSize;
		_lineHeightOffset = lho;
		scaledLineHeightOffset = lho;
	}
}
