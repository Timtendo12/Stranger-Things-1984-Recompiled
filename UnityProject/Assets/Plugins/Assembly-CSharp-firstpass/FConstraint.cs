public class FConstraint
{
	public FNode.Sides Side { get; set; }

	public FNode Constraint { get; set; }

	public FNode.Sides ConstraintSide { get; set; }

	public float ConstraintOffset { get; set; }

	public FConstraint(FNode.Sides _side, FNode _constraint, FNode.Sides _constraintSide, float _constraintOffset)
	{
	}
}
