// Rewritten from libil2cpp.so 1.0.605 (FPageBlackScreen @ 0x17f0738..0x17f0888, Ghidra vaddr).
public class FPageBlackScreen : FUIPage
{
	private FNode mUIContainer;

	private FSprite mBGBackdrop;

	public FPageBlackScreen(string name)
		: base(name)
	{
		mUIContainer = new FNode("UIContainer");
		mUIContainer.SetParent(this);
		mUIContainer.ExpandToFill(this, 0f, 0f);
		mBGBackdrop = new FSprite("BlackSquare.png");
		mBGBackdrop.SetParent(mUIContainer);
		mBGBackdrop.ExpandToFill(mUIContainer, 0f, 0f);
	}
}
