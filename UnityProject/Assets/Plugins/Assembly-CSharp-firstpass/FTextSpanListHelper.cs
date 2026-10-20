using System;
using System.Collections.Generic;
using System.Text;

// Rewritten from libil2cpp.so 1.0.605 (FTextSpanListHelper @ 0x169d1f0..0x169d5xx, Ghidra vaddr).
public class FTextSpanListHelper
{
	protected List<FTextSpan> mSpans;

	protected int mTotalLength;

	protected int mIndex;

	protected int mIndexWithinSpan;

	protected int mSpanIndex;

	protected FTextSpan mSpan;

	public int TotalLength => mTotalLength;

	public FTextSpan Span => mSpan;

	public int IndexWithinSpan => mIndexWithinSpan;

	public bool IsWhiteSpace => mSpan.IsWhiteSpaceAtIndex(mIndexWithinSpan);

	public FTextSpanListHelper(List<FTextSpan> spans)
	{
		mSpans = spans;
		for (int i = 0; i < spans.Count; i++)
		{
			mTotalLength += spans[i].Length;
		}
		if (mTotalLength > 0)
		{
			SetIndex(0);
		}
	}

	public void SetIndex(int index)
	{
		int offset = 0;
		for (int i = 0; ; i++)
		{
			if (i >= mSpans.Count)
			{
				throw new IndexOutOfRangeException();
			}
			FTextSpan span = mSpans[i];
			int length = span.Length;
			if (index - offset < length)
			{
				mSpan = span;
				mIndex = index;
				mIndexWithinSpan = index - offset;
				return;
			}
			offset += length;
		}
	}

	public string GetWrapString()
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < mSpans.Count; i++)
		{
			sb.Append(mSpans[i].WrapString);
		}
		return sb.ToString();
	}
}
