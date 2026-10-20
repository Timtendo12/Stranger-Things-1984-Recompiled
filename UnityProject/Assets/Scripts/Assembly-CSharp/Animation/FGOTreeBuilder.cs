public class FGOTreeBuilder
{
	public delegate void BuildMethodDelegate(FGOTreeItemInterface item);

	protected BuildMethodDelegate mBuildMethod;

	public BuildMethodDelegate BuildMethod => null;

	public FGOTreeBuilder(BuildMethodDelegate buildMethod)
	{
	}
}
