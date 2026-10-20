// Rewritten from libil2cpp.so 1.0.605 (FFacetNode @ 0x16a99bc..0x16a9c3c, Ghidra vaddr).
public class FFacetNode : FNode
{
	protected FAtlasElement mElement;

	protected FShader mShader;

	protected int mNumberOfVerticesNeeded;

	public FAtlasElement Element
	{
		get
		{
			return mElement;
		}
		set
		{
			if (mElement != value)
			{
				FAtlas oldAtlas = mElement.Atlas;
				FAtlas newAtlas = value.Atlas;
				mElement = value;
				if (oldAtlas != newAtlas && mOnStage)
				{
					mStage.HandleFacetsChanged();
				}
				HandleElementChanged();
			}
		}
	}

	public string ElementName => mElement?.Name;

	public FFacetNode()
	{
	}

	public FFacetNode(string name)
		: base(name)
	{
	}

	protected virtual void Init(FAtlasElement element, int numberOfVerticesNeeded)
	{
		mElement = element;
		if (mShader == null)
		{
			mShader = FShader.DefaultShader;
		}
		mNumberOfVerticesNeeded = numberOfVerticesNeeded;
		HandleElementChanged();
	}

	public override void PopulateRenderLayer()
	{
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

	public virtual void HandleElementChanged()
	{
	}

	public void SetElementByName(string elementName)
	{
		Element = Futile.AtlasManager.GetElementWithName(elementName);
	}
}
