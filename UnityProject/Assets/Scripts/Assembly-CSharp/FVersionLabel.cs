using UnityEngine;

public class FVersionLabel : FLabel
{
	public static Vector2 cVersionLabelSize;

	public FVersionLabel()
		: base(null, null, null, 0f, shrinkToFitContainer: false, usesMarkup: false)
	{
	}

	public override void HandleAddedToStage()
	{
	}

	public static string GetVersionString()
	{
		return null;
	}
}
