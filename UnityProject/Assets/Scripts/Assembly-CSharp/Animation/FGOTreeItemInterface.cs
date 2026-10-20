using UnityEngine;

public interface FGOTreeItemInterface
{
	Color PlayerColor { get; }

	Color PlayerColorB { get; }

	Relationship Relationship { get; }

	bool IsLeftFacing { get; }
}
