using System.Collections.Generic;
using UnityEngine;

public class FFontMesh : MonoBehaviour
{
	public float TargetHeight;

	protected List<FFontMeshTextLine> mLines;

	protected string mText;

	[SerializeField]
	[HideInInspector]
	protected Color mBaseColor;

	[SerializeField]
	[HideInInspector]
	protected string mBaseFontStyle;

	[SerializeField]
	[HideInInspector]
	protected int mBasePointSize;

	protected List<FTextSpan> mSpans;

	protected FFont mFont;

	public string Text
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public Color BaseColor
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public string BaseFontStyle
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public int BasePointSize
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public void Awake()
	{
	}

	public void UpdateMesh()
	{
	}

	protected void UpdateSpans()
	{
	}
}
