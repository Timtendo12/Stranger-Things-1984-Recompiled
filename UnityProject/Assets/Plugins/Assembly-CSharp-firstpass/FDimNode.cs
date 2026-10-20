// Rewritten from libil2cpp.so 1.0.605 (FDimNode @ 0x168abe4..0x168acf4, Ghidra vaddr).
public class FDimNode : FNode
{
	public override float Width
	{
		get
		{
			SafeUpdateConstraints();
			return Extents.width;
		}
	}

	public override float Height
	{
		get
		{
			SafeUpdateConstraints();
			return Extents.height;
		}
	}

	public FDimNode()
	{
	}

	public FDimNode(string name)
		: base(name)
	{
	}

	public override void SelfResizeExtents()
	{
		if (HasConstraints() && CalculateExtents())
		{
			UnityEngine.Rect extents = Extents;
			Position = new UnityEngine.Vector2(extents.x + extents.width * 0.5f, extents.y + extents.height * 0.5f);
		}
	}
}
