using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLabel @ 0x16a4a8c..0x16b3ebc, Ghidra vaddr).
public class FLabel : FNode
{
	protected const float cPointSizeIncrement = 2f;

	protected FFont mBaseFont;

	protected FFont mFallBackFont;

	protected bool mMaxFallBackReached;

	protected string mFontStyleName;

	protected string mRawText;

	protected bool mUsesMarkup = true;

	protected List<FTextSpan> mSpans = new List<FTextSpan>();

	protected XmlDocument mMarkupDoc;

	protected List<FLetterQuadLine> mLetterQuadLines;

	protected bool mMeshIsDirty;

	protected Vector2 mLocalScale;

	protected FMatrix mLocalConcatenatedMatrix;

	protected Sides mJustificationX = Sides.CenterX;

	protected Sides mJustificationY = Sides.CenterY;

	protected float mLineHeightDelta;

	protected float mLetterSpacingDelta;

	protected bool mDoesTextNeedUpdate;

	protected bool mDoesLocalPositionNeedUpdate;

	protected bool mDoesJustificationNeedUpdate;

	protected bool mDoSpansNeedUpdate;

	protected bool mClipX;

	protected bool mClipY;

	protected Rect mTextRect;

	protected FTextParams mTextParams;

	protected bool mShrinkToFitContainer = true;

	protected bool mConvertToEllipses = true;

	protected bool mClampToFontPointSize = true;

	protected float mDesiredPtSize = -1f;

	protected float mMinimumPointSize = -1f;

	protected bool mTryExpand = true;

	protected float mWrapScale = 1f;

	protected FShader mShader;

	protected int mTotalNumberOfFacetsNeeded;

	protected int mDynamicFontSize = 32;

	protected FontStyle mDynamicFontStyle;

	private const float cDynamicFontMinimumPointSize = 6f;

	protected Vector2 mCursorPosition = Vector2.zero;

	private Action CreateTextQuadsFunc;

	protected bool mIsDynamic;

	protected FLabelGeomCache mGeomCache = new FLabelGeomCache();

	protected Vector2 mDefaultCursorPosition = Vector2.zero;

	private Vector3 mPreCalculatedRootMotionPosition = Vector3.zero;

	protected List<FRenderLayer> mLastRenderLayers = new List<FRenderLayer>();

	private Vector4 aTuneVisualAdjustmentsLast = Vector4.zero;

	protected int mMaxQuads = int.MaxValue;

	private bool mFailureToGetChar;

	private List<FRenderLayerSpriteAtlas> tempRenderLayers = new List<FRenderLayerSpriteAtlas>();

	private bool mLabelMaking3D;

	private List<FRenderLayer> prevLayers = new List<FRenderLayer>();

	private int[] mPretendFontSizes = new int[17]
	{
		6, 7, 8, 9, 10, 11, 12, 16, 20, 24,
		28, 32, 36, 48, 64, 72, 96
	};

	public List<FLetterQuadLine> LetterQuadLines => mLetterQuadLines;

	public Vector2 DefaultCursorPosition
	{
		get
		{
			UpdateJustification();
			return mDefaultCursorPosition;
		}
	}

	public Vector2 CursorPosition => mCursorPosition;

	public float MinPointSize
	{
		get
		{
			return mMinimumPointSize;
		}
		set
		{
			if (mMinimumPointSize != value)
			{
				mMinimumPointSize = value;
				ResetText();
			}
		}
	}

	public bool UsesMarkup
	{
		get
		{
			return mUsesMarkup;
		}
		set
		{
			if (mUsesMarkup != value)
			{
				mUsesMarkup = value;
				Invalidate();
			}
		}
	}

	public FontStyle Style
	{
		get
		{
			return mDynamicFontStyle;
		}
		set
		{
			if (mDynamicFontStyle != value)
			{
				mDynamicFontStyle = value;
				Invalidate();
			}
		}
	}

	public virtual string Text
	{
		get
		{
			return mRawText;
		}
		set
		{
			if (mRawText != value)
			{
				mRawText = value;
				mMarkupDoc = null;
				mDoSpansNeedUpdate = true;
				Invalidate();
				MarkDirtyRecursive();
				mDoesJustificationNeedUpdate = true;
			}
		}
	}

	public Sides JustificationX
	{
		get
		{
			return mJustificationX;
		}
		set
		{
			if (mJustificationX != value)
			{
				mJustificationX = value;
				mDoesJustificationNeedUpdate = true;
			}
		}
	}

	public Sides JustificationY
	{
		get
		{
			return mJustificationY;
		}
		set
		{
			if (mJustificationY != value)
			{
				mJustificationY = value;
				mDoesJustificationNeedUpdate = true;
			}
		}
	}

	public virtual Vector2 LocalScale
	{
		get
		{
			return mLocalScale;
		}
		set
		{
			float dx = mLocalScale.x - value.x;
			float dy = mLocalScale.y - value.y;
			if (1E-10f <= dx * dx + dy * dy)
			{
				mLocalScale = value;
				mMatrixIsDirty = true;
			}
		}
	}

	public FFont Font => mBaseFont;

	public float TextWidth
	{
		get
		{
			if (mDoesTextNeedUpdate)
			{
				CreateTextQuads();
			}
			return mTextRect.width * mScaleX * mLocalScale.x;
		}
	}

	public float TextHeight
	{
		get
		{
			if (mDoesTextNeedUpdate)
			{
				CreateTextQuads();
			}
			return mTextRect.height * mScaleY * mLocalScale.y;
		}
	}

	public FTextParams TextParams
	{
		get
		{
			return mTextParams;
		}
		set
		{
			mTextParams = value;
		}
	}

	public bool Wrap
	{
		get
		{
			return mTextParams.Wrap;
		}
		set
		{
			if (mTextParams.Wrap != value)
			{
				mTextParams.Wrap = value;
				Invalidate();
			}
		}
	}

	public float ClipWidth
	{
		get
		{
			return mTextParams.ClipWidth;
		}
		set
		{
			if (mTextParams.ClipWidth != value)
			{
				mTextParams.ClipWidth = value;
				Invalidate();
			}
		}
	}

	public int WrapAddSpaces
	{
		get
		{
			return mTextParams.WrapAddSpaces;
		}
		set
		{
			if (mTextParams.WrapAddSpaces != value)
			{
				mTextParams.WrapAddSpaces = value;
				Invalidate();
			}
		}
	}

	public bool ClipX
	{
		get
		{
			return mClipX;
		}
		set
		{
			if (mClipX != value)
			{
				mClipX = value;
				Invalidate();
			}
		}
	}

	public bool ClipY
	{
		get
		{
			return mClipY;
		}
		set
		{
			if (mClipY != value)
			{
				mClipY = value;
				Invalidate();
			}
		}
	}

	public override Color Color
	{
		get
		{
			return mColor;
		}
		set
		{
			float dr = mColor.r - value.r;
			float dg = mColor.g - value.g;
			float db = mColor.b - value.b;
			float da = mColor.a - value.a;
			if (1E-10f <= da * da + db * db + dr * dr + dg * dg)
			{
				Invalidate();
				mColor = value;
				mAlphaIsDirty = true;
				mDoSpansNeedUpdate = true;
			}
		}
	}

	public float PointSize
	{
		get
		{
			if (mBaseFont.mIsDynamic)
			{
				return mDynamicFontSize;
			}
			return mLocalScale.x * mBaseFont.PointSize;
		}
	}

	public Vector4 aTuneVisualAdjustments { get; set; }

	public int MaxQuads
	{
		get
		{
			return mMaxQuads;
		}
		set
		{
			mMaxQuads = value;
			Invalidate();
		}
	}

	public int TotalQuads => mTotalNumberOfFacetsNeeded;

	public void ClearMaxQuads()
	{
		mMaxQuads = int.MaxValue;
		Invalidate();
	}

	public FLabel(string name, string fontStyleName, string text, float pointSize, bool shrinkToFitContainer = true, bool usesMarkup = true, bool convertToEllipses = false)
		: this(name, fontStyleName, text, new FTextParams(), pointSize, shrinkToFitContainer, usesMarkup, convertToEllipses)
	{
	}

	public FLabel(string fontStyleName, string text, float pointSize, bool shrinkToFitContainer = true, bool usesMarkup = true, bool convertToEllipses = false)
		: this(string.Empty, fontStyleName, text, new FTextParams(), pointSize, shrinkToFitContainer, usesMarkup, convertToEllipses)
	{
	}

	protected FLabel(string name, string fontStyleName, string text, FTextParams textParams, float pointSize, bool shrinkToFitContainer = true, bool usesMarkup = true, bool convertToEllipses = false)
		: base(name)
	{
		mUsesMarkup = usesMarkup;
		mShrinkToFitContainer = shrinkToFitContainer;
		aTuneVisualAdjustments = Vector4.zero;
		mConvertToEllipses = convertToEllipses && !shrinkToFitContainer;
		mLocalScale = Vector2.one;
		mLocalConcatenatedMatrix = new FMatrix();
		Color = new Color(1f, 1f, 1f, 1f);
		mTextParams = textParams;
		Text = text;
		SetFont(fontStyleName, pointSize);
		UpdateShader();
	}

	public void SetPointSize(float value)
	{
		float oldPointSize = PointSize;
		float resourceScaleInverse = 1f;
		FFont font = mBaseFont;
		bool needClampBranch = false;
		if (font == null || !font.mIsDynamic)
		{
			mBaseFont = Futile.AtlasManager.GetBaseFontWithStyleAndSize(mFontStyleName, (int)Math.Round(value), out resourceScaleInverse);
			font = mBaseFont;
			if (font == null)
			{
				if (!mClampToFontPointSize)
				{
					throw new NullReferenceException();
				}
				needClampBranch = true;
			}
		}
		if (!needClampBranch)
		{
			if (font.mIsDynamic)
			{
				mDynamicFontSize = (int)Mathf.Max(6f, value);
			}
			if (!mClampToFontPointSize)
			{
				float scale = 1f / font.PointSize * value;
				mLocalScale = new Vector2(scale, scale);
				mWrapScale = scale / resourceScaleInverse;
				if (mDesiredPtSize < 0f)
				{
					mDesiredPtSize = value;
				}
				if (oldPointSize != value)
				{
					Invalidate();
				}
				return;
			}
		}
		mWrapScale = 1f;
		mLocalScale = new Vector2(resourceScaleInverse, resourceScaleInverse);
		if (mIsDynamic)
		{
			mDynamicFontSize = ClampDynamicFontSize(mDynamicFontSize);
		}
		if (mDesiredPtSize < 0f)
		{
			mDesiredPtSize = value;
		}
		if (oldPointSize != value)
		{
			Invalidate();
		}
	}

	public void SetFont(string fontStyleName, float pointSize, bool allowAutoDynamic = true)
	{
		float resourceScaleInverse = 0f;
		if (mFontStyleName == fontStyleName && Mathf.Abs(mDesiredPtSize - pointSize) < Mathf.Epsilon)
		{
			return;
		}
		bool dynamicFonts = FConfigCore.DynamicFonts;
		if (!allowAutoDynamic)
		{
			if (!dynamicFonts)
			{
				fontStyleName = FFont.ConvertToDynamicFontToSpriteFont(fontStyleName);
			}
		}
		else if (FConfigCore.HybridDynamicFonts && FFont.cConvertBitmapFontsToDynamicFontsWhenPossible)
		{
			fontStyleName = FFont.ConvertToDynamicFontHybridList(fontStyleName);
			if (!dynamicFonts)
			{
				fontStyleName = FFont.ConvertToDynamicFontToSpriteFont(fontStyleName);
			}
		}
		else if (!dynamicFonts || !FFont.cConvertBitmapFontsToDynamicFontsWhenPossible)
		{
			if (!dynamicFonts)
			{
				fontStyleName = FFont.ConvertToDynamicFontToSpriteFont(fontStyleName);
			}
		}
		else
		{
			fontStyleName = FFont.ConvertToDynamicFont(fontStyleName);
		}
		mFontStyleName = fontStyleName;
		if (FFont.DynamicFonts.ContainsKey(fontStyleName))
		{
			mBaseFont = FFont.DynamicFonts[fontStyleName];
			mDesiredPtSize = pointSize;
			mMinimumPointSize = 6f;
			mDynamicFontSize = (int)pointSize;
			SetPointSize(pointSize);
			mDynamicFontStyle = FontStyle.Normal;
			mBaseFont.mDFontLabels.Add(new WeakReference(this));
			CreateTextQuadsFunc = CreateTextQuadsDynamic;
			mIsDynamic = true;
		}
		else
		{
			FFontStyle fontStyle = Futile.AtlasManager.GetFontStyle(mFontStyleName);
			if (fontStyle == null)
			{
				mFontStyleName = FFont.DefaultStyleStroke;
				fontStyle = Futile.AtlasManager.GetFontStyle(mFontStyleName);
			}
			if (fontStyle != null)
			{
				mMinimumPointSize = fontStyle.SmallestPointSize;
			}
			mDesiredPtSize = pointSize;
			resourceScaleInverse = 1f;
			mBaseFont = Futile.AtlasManager.GetBaseFontWithStyleAndSize(mFontStyleName, (int)Math.Round(pointSize), out resourceScaleInverse);
			SetPointSize(pointSize);
			CreateTextQuadsFunc = CreateTextQuadsInternal;
			mIsDynamic = false;
		}
		UpdateShader();
	}

	public void ShrinkToFitContainer(bool which)
	{
		if (mShrinkToFitContainer != which)
		{
			mShrinkToFitContainer = which;
			if (which)
			{
				mConvertToEllipses = false;
			}
			ResetText();
		}
	}

	public void ConvertToEllipses(bool which)
	{
		if (mConvertToEllipses != which && !mShrinkToFitContainer)
		{
			mConvertToEllipses = which;
			ResetText();
		}
	}

	public void ClampToFontPointSize(bool clamp)
	{
		if (mClampToFontPointSize != clamp)
		{
			mClampToFontPointSize = clamp;
			ResetText();
		}
	}

	protected void ResetText()
	{
		SetPointSize(mDesiredPtSize);
		mTryExpand = true;
		Invalidate();
		MarkDirtyRecursive();
	}

	public void ShrinkToFitTests()
	{
	}

	public override void Invalidate()
	{
		mDoesTextNeedUpdate = true;
		mDoesJustificationNeedUpdate = true;
		mMeshIsDirty = true;
		mGeomCache.IsDirty = true;
		mInValidationCounter = 5;
	}

	public override void DelayedDispose()
	{
		base.DelayedDispose();
		mShader = null;
		ReleaseQuadData();
	}

	~FLabel()
	{
		ReleaseQuadData();
	}

	protected virtual void UpdateShader()
	{
		mShader = ((mBaseFont == null) ? FShader.DefaultShader : mBaseFont.FontShader);
	}

	protected void ReleaseShader()
	{
		mShader = null;
	}

	private void DoTextUpdate()
	{
		if (mDoSpansNeedUpdate)
		{
			if (!mUsesMarkup)
			{
				mSpans.Clear();
				FTextSpanString span = new FTextSpanString();
				span.Text = mRawText;
				span.Font = mBaseFont;
				span.Color = mColor;
				span.PointSize = mDesiredPtSize;
				mSpans.Add(span);
			}
			else if (mMarkupDoc == null)
			{
				mSpans = FTextBuilder.GetSpansFromMarkup(mRawText, mBaseFont, Color, out mMarkupDoc);
			}
			else
			{
				mSpans = FTextBuilder.GetSpansFromMarkup(mMarkupDoc, mBaseFont, Color);
			}
			mDoSpansNeedUpdate = false;
		}
		mDoesTextNeedUpdate = false;
	}

	private void GetShrinkWrapSettings(out bool allowShrink, out float wrapScale)
	{
		allowShrink = false;
		wrapScale = 1f;
		Rect extents = Extents;
		if (mShrinkToFitContainer && 0f < extents.width && 0f < extents.height)
		{
			if (CanDecPointSize(out float _))
			{
				allowShrink = true;
			}
			wrapScale = mWrapScale;
		}
	}

	private void ReleaseQuadData()
	{
		if (FConfigCore.PoolFLetterQuad && mLetterQuadLines != null)
		{
			for (int i = 0; i < mLetterQuadLines.Count; i++)
			{
				mLetterQuadLines[i].ReleaseData(mIsDynamic);
			}
			mLetterQuadLines.Clear();
		}
	}

	protected void CreateTextQuadsInternal()
	{
		if (mDoesTextNeedUpdate)
		{
			DoTextUpdate();
		}
		if (mBaseFont == null || mSpans.Count < 1)
		{
			ReleaseQuadData();
			mLetterQuadLines = null;
			mTotalNumberOfFacetsNeeded = 0;
			return;
		}
		GetShrinkWrapSettings(out bool allowShrink, out float wrapScale);
		FTextParams textParams = mTextParams;
		int previousTotal = mTotalNumberOfFacetsNeeded;
		if ((textParams.Wrap || mConvertToEllipses) && textParams.ClipWidth <= 0f)
		{
			textParams.ClipWidth = Extents.width;
		}
		ReleaseQuadData();
		mLetterQuadLines = FTextBuilder.GetQuadInfoForText(mSpans, mTextParams, allowShrink, wrapScale, mConvertToEllipses, mBaseFont, out bool fontFailed, mMaxFallBackReached, 24, FontStyle.Normal);
		mTotalNumberOfFacetsNeeded = 0;
		if (mLetterQuadLines == null)
		{
			ProcessFontFailure(mSpans);
			mTotalNumberOfFacetsNeeded = 0;
			mFailureToGetChar = true;
			return;
		}
		int total = 0;
		for (int i = 0; i < mLetterQuadLines.Count; i++)
		{
			total += mLetterQuadLines[i].GetTotalQuadCount();
			mTotalNumberOfFacetsNeeded = total;
		}
		if (mOnStage && total != previousTotal)
		{
			mStage.HandleFacetsChanged();
		}
		UpdateLocalPosition();
	}

	private void ProcessFontFailure(List<FTextSpan> spans)
	{
		FFont font = mBaseFont;
		if (mFallBackFont != null)
		{
			font = mFallBackFont;
		}
		for (int i = 0; i < spans.Count; i++)
		{
			FTextSpanString spanString = spans[i] as FTextSpanString;
			if (spanString != null && font.HasMissingChars(spanString.Text))
			{
				string fallback = FFont.GetFallBackFont(font, mFontStyleName, spanString.Text, out bool isMaxFallback);
				SetFont(fallback, PointSize, false);
				mFallBackFont = mBaseFont;
				mMaxFallBackReached = isMaxFallback;
				return;
			}
		}
	}

	public void UpdateLocalPosition()
	{
		mDoesLocalPositionNeedUpdate = false;
		if (mLetterQuadLines == null || mLetterQuadLines.Count < 1)
		{
			mMeshIsDirty = true;
			mCursorPosition = mDefaultCursorPosition;
			mTextRect = default(Rect);
			return;
		}
		int lineCount = mLetterQuadLines.Count;
		float minY = float.MaxValue;
		float maxY = float.MinValue;
		for (int i = 0; i < lineCount; i++)
		{
			FLetterQuadLine line = mLetterQuadLines[i];
			minY = Math.Min(line.Bounds.y, minY);
			maxY = Math.Max(mLetterQuadLines[i].Bounds.height + mLetterQuadLines[i].Bounds.y, maxY);
		}
		float offsetY = GetOffsetForJustificationY(minY, maxY);
		float minX = float.MaxValue;
		float maxX = float.MinValue;
		for (int j = 0; j < lineCount; j++)
		{
			FLetterQuadLine line2 = mLetterQuadLines[j];
			// debug logging (mDebugMe) omitted
			float offsetX = GetOffsetForJustificationX(line2.Bounds.width);
			minX = Math.Min(offsetX, minX);
			maxX = Math.Max(offsetX + line2.Bounds.width, maxX);
			line2.CalculateVectors(offsetX, offsetY, mScaleX * mLocalScale.x);
			if (j == lineCount - 1)
			{
				float inverse = Futile.ResourceScaleInverse;
				mCursorPosition = new Vector2((offsetX + line2.Bounds.width) * inverse, inverse * (offsetY + line2.Bounds.y + line2.Bounds.height * 0.5f));
			}
		}
		mTextRect.x = minX;
		mMeshIsDirty = true;
		mTextRect.y = minY + offsetY;
		mTextRect.width = maxX - minX;
		mTextRect.height = maxY - minY;
	}

	public void ClearQuads()
	{
		ReleaseQuadData();
		mLetterQuadLines = null;
		mTotalNumberOfFacetsNeeded = 0;
	}

	public void CreateTextQuadsDynamic()
	{
		GetShrinkWrapSettings(out bool allowShrink, out float wrapScale);
		if (mDoesTextNeedUpdate)
		{
			DoTextUpdate();
		}
		FTextParams textParams = mTextParams;
		int previousTotal = mTotalNumberOfFacetsNeeded;
		if ((textParams.Wrap || mConvertToEllipses) && textParams.ClipWidth <= 0f)
		{
			textParams.ClipWidth = Extents.width;
		}
		ReleaseQuadData();
		mLetterQuadLines = FTextBuilder.GetQuadInfoForText(mSpans, mTextParams, allowShrink, wrapScale, mConvertToEllipses, mBaseFont, out bool fontFailed, mMaxFallBackReached, mDynamicFontSize, mDynamicFontStyle);
		if (mLetterQuadLines == null)
		{
			ProcessFontFailure(mSpans);
			mTotalNumberOfFacetsNeeded = 0;
			mFailureToGetChar = true;
			return;
		}
		int total = 0;
		mTotalNumberOfFacetsNeeded = 0;
		for (int i = 0; i < mLetterQuadLines.Count; i++)
		{
			total += mLetterQuadLines[i].GetTotalQuadCount();
			mTotalNumberOfFacetsNeeded = total;
		}
		if (mOnStage && total != previousTotal)
		{
			mStage.HandleFacetsChanged();
		}
		UpdateLocalPosition();
	}

	private void DoShrinkToFitLoop()
	{
	}

	public void ForceUpdateUVs()
	{
		if (mIsDynamic)
		{
			CreateTextQuads();
			PopulateRenderLayerDirect();
		}
	}

	public void CreateTextQuads()
	{
		FAssert.That(mBaseFont != null, "Font == null!  Impossible");
		FAssert.That(mTextParams != null, "mTextParams == null!  Impossible!");
		if (!string.IsNullOrEmpty(mRawText))
		{
			CreateTextQuadsFunc();
			if (mFailureToGetChar)
			{
				return;
			}
			Rect extents = Extents;
			// debug logging (mDebugMe) omitted
			if (!mShrinkToFitContainer || !(0f < extents.width) || !(0f < extents.height))
			{
				return;
			}
			int iterations = 11;
			do
			{
				iterations--;
				if (iterations == 0)
				{
					return;
				}
				bool shrinkCheck = !mTryExpand;
				if (mTryExpand)
				{
					Rect e = Extents;
					if (!(e.width * 0.9f <= TextWidth) || !(Extents.height * 0.9f <= TextHeight))
					{
						if (IncPointSize())
						{
							CreateTextQuadsFunc();
							continue;
						}
						mTryExpand = false;
						continue;
					}
					shrinkCheck = true;
				}
				if (shrinkCheck)
				{
					if (TextWidth - Extents.width <= 3f && TextHeight - Extents.height <= 3f)
					{
						return;
					}
					mTryExpand = false;
					if (!DecPointSize())
					{
						return;
					}
					CreateTextQuadsFunc();
				}
			}
			while (!mFailureToGetChar);
		}
		else
		{
			ReleaseQuadData();
			mLetterQuadLines = null;
			mTotalNumberOfFacetsNeeded = 0;
			mSpans.Clear();
			mDoesTextNeedUpdate = false;
		}
	}

	protected override void UpdateLocalMatrix()
	{
		mLocalConcatenatedMatrix.PreScale(mLocalScale.x, mLocalScale.y, mConcatenatedMatrix);
	}

	protected int GetQuadLineCount()
	{
		if (mLetterQuadLines == null)
		{
			return 0;
		}
		return mLetterQuadLines.Count;
	}

	protected float GetOffsetForJustificationX(float lineWidth)
	{
		switch (mJustificationX)
		{
		case Sides.CenterX:
			return (float)Math.Round(lineWidth * -0.5f);
		case Sides.Right:
		{
			Rect extents = Extents;
			return (float)Math.Round(0f - lineWidth) + extents.width * 0.5f;
		}
		case Sides.Left:
		{
			Rect extents2 = Extents;
			return (float)Math.Round(lineWidth * -0f) + extents2.width * -0.5f;
		}
		default:
			return 0f;
		}
	}

	protected float GetOffsetForJustificationY(float minY, float maxY)
	{
		float result;
		switch (mJustificationY)
		{
		case Sides.CenterY:
			result = mBaseFont.VerticalCenteringOffset * 0.5f - ((maxY - minY) * 0.5f + minY);
			break;
		case Sides.Bottom:
		{
			Rect extents = Extents;
			result = extents.height * -0.5f - ((maxY - minY) * 0f + minY);
			break;
		}
		case Sides.Top:
		{
			Rect extents2 = Extents;
			result = extents2.height * 0.5f - (maxY - minY + minY) + mBaseFont.VerticalCenteringOffset;
			break;
		}
		default:
			return 0f;
		}
		return (float)Math.Round(result);
	}

	public void UpdateJustification()
	{
		mDoesJustificationNeedUpdate = false;
		Rect extents = Extents;
		if (mClipX)
		{
			mTextParams.ClipWidth = extents.width * (1f / (mLocalScale.x * mScaleX));
		}
		if (mClipY)
		{
			mTextParams.ClipHeight = extents.height * (1f / (mLocalScale.y * mScaleY));
		}
		mDoesLocalPositionNeedUpdate = true;
		float offsetY = GetOffsetForJustificationY(0f - PointSize, 0f);
		float offsetX = GetOffsetForJustificationX(1f);
		float inverse = Futile.ResourceScaleInverse;
		mDefaultCursorPosition = new Vector2((offsetX + 1f) * inverse, (offsetY + PointSize * -0.5f) * inverse);
	}

	public override void RedrawPrep(bool shouldForceDirty, bool shouldUpdateDepth, bool wasMatrixDirty, bool wasAlphaDirty)
	{
		if (mInValidationCounter > 0)
		{
			// debug logging (mDebugMe) omitted
			mInValidationCounter--;
			mMeshIsDirty = true;
			if (mParent != null)
			{
				mParent.ReComputeLabelFormatting();
			}
		}
		Vector4 current = aTuneVisualAdjustments;
		Vector4 diff = current - aTuneVisualAdjustmentsLast;
		if (1E-10f <= diff.x * diff.x + diff.y * diff.y + diff.z * diff.z + diff.w * diff.w)
		{
			mMeshIsDirty = true;
			aTuneVisualAdjustmentsLast = current;
		}
		if (mFailureToGetChar)
		{
			mFailureToGetChar = false;
			mMeshIsDirty = true;
		}
		if (mShader == null)
		{
			UpdateShader();
		}
		if (mMeshIsDirty)
		{
			if (mBaseFont != null && mBaseFont.mIsDynamic)
			{
				mBaseFont.WrappedRequestCharactersInTexture(Text, mDynamicFontSize, mDynamicFontStyle);
			}
			CreateTextQuads();
		}
		if (shouldForceDirty && wasMatrixDirty)
		{
			if (!UseRootMotion)
			{
				mMeshIsDirty = true;
			}
		}
		else if (shouldUpdateDepth || wasAlphaDirty || shouldForceDirty)
		{
			mMeshIsDirty = true;
		}
		if (wasMatrixDirty)
		{
			if (!UseRootMotion)
			{
				mMeshIsDirty = true;
			}
			else
			{
				RootPositionDirty = true;
			}
		}
		if (mDoesJustificationNeedUpdate)
		{
			UpdateJustification();
		}
		if (mDoesLocalPositionNeedUpdate)
		{
			UpdateLocalPosition();
		}
	}

	public override void RedrawInternal(bool shouldForceDirty, bool shouldUpdateDepth, bool wasMatrixDirty, bool wasAlphaDirty)
	{
		if (wasMatrixDirty)
		{
			if (!UseRootMotion)
			{
				mMeshIsDirty = true;
			}
			else
			{
				RootPositionDirty = true;
			}
		}
		Vector3 rootPosition = CalcRootPosition();
		if (UseRootMotion && mLastRenderLayer != null)
		{
			int frameCount = Time.frameCount;
			FRenderLayer layer = mLastRenderLayer;
			if (layer.mRenderLayerTracking.Count > 1 && RootPositionDirty && layer.LastFrameAccessed == frameCount && layer.RootMotionPosition != rootPosition)
			{
				mMeshIsDirty = true;
			}
			layer.LastFrameAccessed = frameCount;
		}
		mPreCalculatedRootMotionPosition = rootPosition;
		if (!mMeshIsDirty && !mRenderLayerDirty && (mLastRenderLayers.Count <= 0 || !mLastRenderLayers[0].NeedsRerender()))
		{
			for (int i = 0; i < mLastRenderLayers.Count; i++)
			{
				mStage.Renderer.HackUpdateCameraRenderLayer(mLastRenderLayers[i]);
			}
			ApplyRootPositionToLayers();
			return;
		}
		mRenderLayerDirty = false;
		if (UseRootMotion)
		{
			for (int j = 0; j < mLastRenderLayers.Count; j++)
			{
				mLastRenderLayers[j].SetRootPosition(Vector3.zero);
			}
			RootPositionDirty = true;
			RootInitialPos = mPosition;
			if (mMotionParentRef != null)
			{
				ParentRootInitialPos = mMotionParentRef.Position;
			}
		}
		PopulateRenderLayer();
		if (UseRootMotion)
		{
			for (int k = 0; k < mLastRenderLayers.Count; k++)
			{
				mLastRenderLayers[k].UpdateLayerMeshIfNeeded();
			}
		}
		ApplyRootPositionToLayers();
	}

	// Shared tail of RedrawInternal (the original jumps to one label from both paths).
	private void ApplyRootPositionToLayers()
	{
		if (RootInitialPosSet && RootPositionDirty)
		{
			if (mLastRenderLayers.Count < 1)
			{
				return;
			}
			RootPositionDirty = false;
			for (int i = 0; i < mLastRenderLayers.Count; i++)
			{
				mLastRenderLayers[i].SetRootPosition(mPreCalculatedRootMotionPosition);
			}
			mLastRootMotionPosition = mPreCalculatedRootMotionPosition;
		}
	}

	protected override GameObject Make3DInternal(F3DTracker tracker)
	{
		RecycleCounter = 0;
		tempRenderLayers.Clear();
		mLabelMaking3D = true;
		PopulateRenderLayer();
		mLabelMaking3D = false;
		for (int i = 0; i < tempRenderLayers.Count; i++)
		{
			FRenderLayerSpriteAtlas layer = tempRenderLayers[i];
			GameObject renderGO = layer.GetRenderGO();
			layer.BakeMesh3D();
			MeshFilter meshFilter = renderGO.GetOrCreateComponent<MeshFilter>();
			meshFilter.mesh = layer.GetMesh();
			renderGO.SetActive(true);
			tracker.AddRenderLayer(renderGO, layer);
		}
		return null;
	}

	public override void PopulateRenderLayer()
	{
		if (!mOnStage || mTotalNumberOfFacetsNeeded < 1 || mBaseFont == null || mBaseFont.element == null)
		{
			return;
		}
		if (FConfigCore.LabelGeomCache)
		{
			if (!mGeomCache.IsDirty)
			{
				PopulateFromGeomCache();
				return;
			}
			UpdateGeomCache();
		}
		PopulateRenderLayerDirect();
	}

	protected void PopulateRenderLayerDirect()
	{
		float pointSize = PointSize;
		FFont font = mBaseFont;
		Vector4 visual = default(Vector4);
		float offsetX;
		float offsetY;
		float visualZ;
		float visualW;
		if (!font.mIsDynamic)
		{
			visualZ = 1f;
			visualW = 1f;
			offsetX = Vector2.zero.x;
			offsetY = Vector2.zero.y;
		}
		else
		{
			if (!font.TryGetVisualAdjustmentsBySize(PointSize, out visual))
			{
				visual = mBaseFont.mDynamicOffsetAdjustment;
			}
			Vector4 tune = aTuneVisualAdjustments;
			float scale = pointSize / 48f;
			if (tune.x * tune.x + tune.y * tune.y + tune.z * tune.z + tune.w * tune.w < 1E-10f)
			{
				tune = visual;
			}
			else
			{
				if (tune.w <= 0f)
				{
					aTuneVisualAdjustments = visual;
					tune = visual;
				}
				visual = tune;
			}
			float tuneY = tune.y * (1f / 48f);
			offsetX = scale * tune.x * (1f / 48f);
			offsetY = scale * tuneY;
			visualZ = tune.z;
			visualW = tune.w;
		}
		prevLayers.Clear();
		prevLayers.AddRange(mLastRenderLayers);
		mLastRenderLayers.Clear();
		ClearRenderLayerTracking();
		if (!mOnStage || mTotalNumberOfFacetsNeeded < 1 || mBaseFont == null || mBaseFont.element == null)
		{
			return;
		}
		mMeshIsDirty = false;
		Color additiveTint = ConcatenatedAdditiveTintColor;
		Color tint = ((mParent == null) ? new Color(1f, 1f, 1f, mConcatenatedAlpha) : mParent.ConcatenatedColor.CloneWithMultipliedAlpha(mConcatenatedAlpha));
		int quadsLeft = mMaxQuads;
		if (mLetterQuadLines != null && mLetterQuadLines.Count > 0 && quadsLeft > 0)
		{
			float outlinePad = visualW * visualZ * 3f;
			for (int lineIndex = 0; lineIndex < mLetterQuadLines.Count && quadsLeft > 0; lineIndex++)
			{
				List<FLetterQuadGroup> groups = mLetterQuadLines[lineIndex].LetterQuadGroups;
				for (int groupIndex = 0; groupIndex < groups.Count && quadsLeft > 0; groupIndex++)
				{
					FLetterQuadGroup group = groups[groupIndex];
					List<FLetterQuad> quads = group.Quads;
					if (FLoc.IsRightToLeft)
					{
						quads.Reverse();
					}
					if (quads.Count < 1)
					{
						continue;
					}
					Color groupColor = group.Color;
					int take = Math.Min(quads.Count, quadsLeft);
					FRenderLayerSpriteAtlas layer;
					int startVertex;
					if (!mLabelMaking3D)
					{
						mStage.Renderer.GetRenderLayerSpriteAtlas(out layer, NoRenderLayerSharing, group.Atlas, group.Shader, take, mIsDynamic, mShouldSortByZ);
						mLastRenderLayers.Add(layer);
						prevLayers.Remove(layer);
						LabelBookKeepingPrePass(layer);
						startVertex = layer.vertices.Count;
						NewRLTracking(layer, startVertex, layer.indexCount, take * 2, take * 4);
					}
					else
					{
						mStage.Renderer.Get3DFacetRenderLayer(out layer, group.Atlas, group.Shader, take, mIsDynamic, RecycleRef, RecycleCounter);
						RecycleCounter++;
						tempRenderLayers.Add(layer);
						startVertex = layer.vertices.Count;
					}
					layer.LogMaterialValue("_ShadowColor", font.ShadowColor);
					layer.LogMaterialValue("_OutlineColor", font.OutlineColor);
					Material sharedMaterial = layer.GetSharedMaterial();
					sharedMaterial.SetColor("_ShadowColor", font.ShadowColor);
					sharedMaterial.SetColor("_OutlineColor", font.OutlineColor);
					layer.HackSharedTexture();
					List<Vector2> uvs = layer.uvs;
					List<Color> colors = layer.colors;
					List<Vector2> uv2 = layer.uv2;
					List<Vector2> uv3 = layer.uv3;
					List<Vector4> uv4 = layer.uv4;
					List<Vector4> colors2 = layer.colors2;
					List<Vector3> vertices = layer.vertices;
					float r = tint.r * groupColor.r;
					float g = tint.g * groupColor.g;
					float b = tint.b * groupColor.b;
					float a = tint.a * groupColor.a;
					for (int k = 0; k < take; k++)
					{
						FLetterQuad quad = quads[k];
						bool dynamicGlyph = mIsDynamic && !quad.IsImage;
						FMatrix matrix = mLocalConcatenatedMatrix;
						if (quad.rotation != 0f)
						{
							matrix = matrix.Clone();
							FMatrix rotation = new FMatrix();
							float centerY = quad.charInfo.offsetY + (quad.bottomLeft.y + quad.bottomRight.y) * 0.5f;
							float sumX = quad.bottomLeft.x + quad.bottomRight.x;
							rotation.Translate(0f - sumX * 0.5f, 0f - centerY);
							rotation.Rotate(quad.rotation);
							rotation.Translate(sumX * 0.5f, centerY);
							matrix.ConcatOther(rotation);
						}
						if (!dynamicGlyph)
						{
							vertices.Add(matrix.GetVector3FromLocalVector2(new Vector2(offsetX + quad.topLeft.x, offsetY + quad.topLeft.y), 0f));
							vertices.Add(matrix.GetVector3FromLocalVector2(new Vector2(offsetX + quad.topRight.x, offsetY + quad.topRight.y), 0f));
							vertices.Add(matrix.GetVector3FromLocalVector2(new Vector2(offsetX + quad.bottomRight.x, offsetY + quad.bottomRight.y), 0f));
							vertices.Add(matrix.GetVector3FromLocalVector2(new Vector2(offsetX + quad.bottomLeft.x, offsetY + quad.bottomLeft.y), 0f));
							uvs.Add(quad.uvTopLeft);
							uvs.Add(quad.uvTopRight);
							uvs.Add(quad.uvBottomRight);
							uvs.Add(quad.uvBottomLeft);
						}
						Color vertexColor = new Color(r, g, b, a);
						colors.Add(vertexColor);
						colors.Add(vertexColor);
						colors.Add(vertexColor);
						colors.Add(vertexColor);
						Vector4 additive = new Vector4(additiveTint.r, additiveTint.g, additiveTint.b, additiveTint.a);
						colors2.Add(additive);
						colors2.Add(additive);
						colors2.Add(additive);
						colors2.Add(additive);
						if (dynamicGlyph)
						{
							Vector2 uvTL = quad.uvTopLeft;
							Vector2 uvTR = quad.uvTopRight;
							Vector2 uvBR = quad.uvBottomRight;
							Vector2 uvBL = quad.uvBottomLeft;
							float maxU = uvTL.x;
							if (uvTL.x == uvBR.x || uvBR.x > uvTL.x)
							{
								maxU = uvBR.x;
							}
							float minU = uvTL.x;
							if (uvBR.x <= uvTL.x)
							{
								minU = uvBR.x;
							}
							float minV = uvTL.y;
							if (uvBR.y <= uvTL.y)
							{
								minV = uvBR.y;
							}
							float maxV = uvTL.y;
							if (uvTL.y <= uvBR.y)
							{
								maxV = uvBR.y;
							}
							float pad = (maxU - minU) * 0.0009f * 12.5f;
							Vector2 uvMin = new Vector2(minU - pad, minV - pad);
							uv2.Add(uvMin);
							uv2.Add(uvMin);
							uv2.Add(uvMin);
							uv2.Add(uvMin);
							Vector2 uvMax = new Vector2(maxU + pad, maxV + pad);
							uv3.Add(uvMax);
							uv3.Add(uvMax);
							uv3.Add(uvMax);
							uv3.Add(uvMax);
							Vector2 dirA = (uvTL - uvTR).normalized;
							Vector2 dirB = (uvTR - uvBR).normalized;
							vertices.Add(matrix.GetVector3FromLocalVector2(new Vector2(offsetX + quad.topLeft.x - outlinePad, offsetY + quad.topLeft.y + outlinePad), 0f));
							vertices.Add(matrix.GetVector3FromLocalVector2(new Vector2(offsetX + quad.topRight.x + outlinePad, offsetY + quad.topRight.y + outlinePad), 0f));
							vertices.Add(matrix.GetVector3FromLocalVector2(new Vector2(offsetX + quad.bottomRight.x + outlinePad, offsetY + quad.bottomRight.y - outlinePad), 0f));
							vertices.Add(matrix.GetVector3FromLocalVector2(new Vector2(offsetX + quad.bottomLeft.x - outlinePad, offsetY + quad.bottomLeft.y - outlinePad), 0f));
							float aspect = 1f;
							float quadWidth = quad.topRight.x - quad.topLeft.x;
							float quadHeight = quad.topRight.y - quad.bottomLeft.y;
							float uPerPixel = (uvTR.x - uvTL.x) / quadWidth;
							if (0f < uPerPixel)
							{
								aspect = Mathf.Abs((uvTR.y - uvBL.y) / quadHeight / uPerPixel);
							}
							float deltaV = uvTR.y - uvTL.y;
							if (deltaV < 0f)
							{
								float ratioW = Mathf.Abs(deltaV) / quadWidth;
								uPerPixel = Mathf.Abs(uvTR.x - uvBR.x) / quadHeight;
								if (0f < ratioW)
								{
									aspect = Mathf.Abs(ratioW / uPerPixel);
								}
							}
							float outlineScale = visualW * quad.charInfo.mFont.mOutlineMult;
							float inverseScale = 1f / quad.charInfo.DynamicScaleMult;
							float scaleFactor = inverseScale * outlineScale;
							Vector4 direction = new Vector4(dirA.x * scaleFactor, dirA.y * scaleFactor, dirB.x * scaleFactor, dirB.y * scaleFactor);
							uv4.Add(direction);
							uv4.Add(direction);
							uv4.Add(direction);
							uv4.Add(direction);
							float negAspect = 0f - aspect;
							float centerU = (uvTL.x + uvTR.x + uvBR.x + uvBL.x) * 0.25f;
							float centerV = (uvTL.y + uvTR.y + uvBR.y + uvBL.y) * 0.25f;
							Vector2[] corners = new Vector2[4] { uvTL, uvTR, uvBR, uvBL };
							for (int c = 0; c < 4; c++)
							{
								float signY = (0f <= corners[c].y - centerV) ? aspect : negAspect;
								float signX = (0f <= corners[c].x - centerU) ? outlinePad : (0f - outlinePad);
								uvs.Add(new Vector2(corners[c].x + uPerPixel * signX, corners[c].y + uPerPixel * outlinePad * signY));
							}
						}
					}
					layer.AddSimpleQuadList(startVertex);
					layer.SetLayerMeshDirty();
					quadsLeft -= take;
				}
			}
		}
		for (int p = 0; p < prevLayers.Count; p++)
		{
			LabelBookKeepingPostPass(prevLayers[p]);
		}
	}

	protected bool IncPointSize()
	{
		float newPointSize = -1f;
		if (CanIncPointSize(out newPointSize) && newPointSize > 0f)
		{
			SetPointSize(newPointSize);
			return true;
		}
		return false;
	}

	protected bool DecPointSize()
	{
		float newPointSize = -1f;
		if (CanDecPointSize(out newPointSize) && newPointSize > 0f)
		{
			SetPointSize(newPointSize);
			return true;
		}
		return false;
	}

	protected void PopulateFromGeomCache()
	{
		prevLayers.Clear();
		prevLayers.AddRange(mLastRenderLayers);
		mMeshIsDirty = false;
		mLastRenderLayers.Clear();
		ClearRenderLayerTracking();
		foreach (KeyValuePair<FTuple<FAtlas, FShader>, FLabelGeomCacheEntry> pair in mGeomCache.Entries)
		{
			FLabelGeomCacheEntry entry = pair.Value;
			int quadCount = entry.Vertices.Count / 4;
			FRenderLayerSpriteAtlas layer;
			if (!mLabelMaking3D)
			{
				mStage.Renderer.GetRenderLayerSpriteAtlas(out layer, NoRenderLayerSharing, pair.Key.Item1, pair.Key.Item2, quadCount, mIsDynamic, mShouldSortByZ);
				mLastRenderLayers.Add(layer);
				prevLayers.Remove(layer);
				LabelBookKeepingPrePass(layer);
				NewRLTracking(layer, layer.vertices.Count, layer.indexCount, quadCount * 2, quadCount * 4);
			}
			else
			{
				mStage.Renderer.Get3DFacetRenderLayer(out layer, pair.Key.Item1, pair.Key.Item2, quadCount, mIsDynamic, RecycleRef, RecycleCounter);
				tempRenderLayers.Add(layer);
			}
			int startVertex = layer.vertices.Count;
			layer.LogMaterialValue("_ShadowColor", mBaseFont.ShadowColor);
			layer.LogMaterialValue("_OutlineColor", mBaseFont.OutlineColor);
			Material sharedMaterial = layer.GetSharedMaterial();
			sharedMaterial.SetColor("_ShadowColor", mBaseFont.ShadowColor);
			sharedMaterial.SetColor("_OutlineColor", mBaseFont.OutlineColor);
			layer.HackSharedTexture();
			for (int i = 0; i < entry.Vertices.Count; i++)
			{
				Vector3 v = entry.Vertices[i];
				layer.vertices.Add(mLocalConcatenatedMatrix.GetVector3FromLocalVector2(v, 0f));
			}
			layer.uvs.AddRange(entry.UVs);
			layer.colors.AddRange(entry.Colors);
			layer.colors2.AddRange(entry.Colors2);
			layer.uv2.AddRange(entry.UV2);
			layer.uv3.AddRange(entry.UV3);
			layer.uv4.AddRange(entry.UV4);
			layer.AddSimpleQuadList(startVertex);
		}
		for (int j = 0; j < prevLayers.Count; j++)
		{
			LabelBookKeepingPostPass(prevLayers[j]);
		}
	}

	protected void LabelBookKeepingPrePass(FRenderLayer layer)
	{
		if (layer != null && layer.mBoundNodes != null)
		{
			if (!layer.mBoundNodes.Contains(this))
			{
				layer.mBoundNodes.Add(this);
			}
		}
	}

	protected void LabelBookKeepingPostPass(FRenderLayer layer)
	{
		if (layer != null && layer.mBoundNodes != null)
		{
			layer.mBoundNodes.Remove(this);
			if (layer.mBoundNodes.Count == 0)
			{
				mStage.Renderer.ReleaseLiveLayer(layer, false);
			}
		}
	}

	protected void UpdateGeomCache()
	{
		mMeshIsDirty = false;
		mGeomCache.Clear();
		throw new NotImplementedException();
	}

	public override void SelfResizeExtents()
	{
		if (HasConstraints() && CalculateExtents())
		{
			if (mShrinkToFitContainer)
			{
				SetPointSize(mDesiredPtSize);
				mTryExpand = true;
			}
			Rect extents = Extents;
			Position = new Vector2(extents.x + extents.width * 0.5f, extents.y + extents.height * 0.5f);
		}
	}

	public void Justify(Sides jx, Sides jy = Sides.CenterY)
	{
		if (mJustificationX != jx)
		{
			mJustificationX = jx;
			mDoesJustificationNeedUpdate = true;
		}
		if (mJustificationY != jy)
		{
			mJustificationY = jy;
			mDoesJustificationNeedUpdate = true;
		}
	}

	public void ClipAndWrap(bool v)
	{
		if (mClipX != v)
		{
			mClipX = v;
			Invalidate();
		}
		if (mClipY != v)
		{
			mClipY = v;
			Invalidate();
		}
		Wrap = v;
		Invalidate();
	}

	protected bool CanIncPointSize(out float newPointSize)
	{
		newPointSize = -1f;
		float pointSize = PointSize;
		float desired = mDesiredPtSize;
		if (desired + -0.1f <= pointSize)
		{
			return false;
		}
		float candidate;
		if (!mClampToFontPointSize)
		{
			candidate = pointSize + 2f;
			if (desired <= candidate)
			{
				candidate = desired;
			}
			if (!(0f <= candidate) || !(pointSize < candidate))
			{
				return false;
			}
			if (desired + FMath.Epsilon <= candidate)
			{
				return false;
			}
		}
		else
		{
			int rounded = (int)Math.Round(pointSize);
			int next = Futile.AtlasManager.GetNextLargerPointSize(mFontStyleName, rounded);
			if (next < 0)
			{
				return false;
			}
			if (next <= rounded)
			{
				return false;
			}
			candidate = next;
			if (mDesiredPtSize + FMath.Epsilon <= candidate)
			{
				return false;
			}
		}
		newPointSize = candidate;
		return true;
	}

	protected int ClampDynamicFontSize(int inputSize)
	{
		int[] sizes = mBaseFont.mAvailablePointSizes;
		int result = inputSize;
		if (sizes != null)
		{
			if (sizes.Length == 0)
			{
				throw new IndexOutOfRangeException();
			}
			result = sizes[0];
			if (sizes[0] <= inputSize)
			{
				result = sizes[sizes.Length - 1];
				if (inputSize <= result)
				{
					result = inputSize;
				}
			}
		}
		return result;
	}

	protected bool CanDecPointSize(out float newPointSize)
	{
		newPointSize = -1f;
		float pointSize = PointSize;
		float minimum = mMinimumPointSize;
		if (pointSize <= minimum + 0.1f)
		{
			return false;
		}
		FFont font = mBaseFont;
		if (mClampToFontPointSize && !font.mIsDynamic)
		{
			int rounded = (int)Math.Round(pointSize);
			int next = Futile.AtlasManager.GetNextSmallerPointSize(mFontStyleName, rounded);
			if (next < 0)
			{
				return false;
			}
			if (rounded <= next)
			{
				return false;
			}
			float nextSize = next;
			if (nextSize <= mMinimumPointSize - FMath.Epsilon)
			{
				return false;
			}
			newPointSize = nextSize;
			return true;
		}
		if (!font.mIsDynamic)
		{
			float candidate = pointSize - 2f;
			if (candidate <= minimum)
			{
				candidate = minimum;
			}
			if (0f <= candidate && candidate < pointSize && minimum - FMath.Epsilon < candidate)
			{
				newPointSize = candidate;
				return true;
			}
			return false;
		}
		int[] sizes = ((font.mAvailablePointSizes != null) ? font.mAvailablePointSizes : mPretendFontSizes);
		int best = 1000;
		for (int i = 0; i < sizes.Length; i++)
		{
			int size = sizes[i];
			if (PointSize <= (float)size)
			{
				size = best;
			}
			best = size;
		}
		if (best < 1000)
		{
			newPointSize = Math.Max(mMinimumPointSize, (float)best);
			return true;
		}
		return false;
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		if (!mOnStage)
		{
			mStage.HandleFacetsChanged();
		}
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		if (mOnStage)
		{
			mStage.HandleFacetsChanged();
		}
	}
}
