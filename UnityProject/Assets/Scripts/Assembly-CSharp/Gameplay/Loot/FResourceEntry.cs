[SaveVersion(0)]
public class FResourceEntry
{
	public int ID => 0;

	public FResourceType Type { get; set; }

	public Fix64 Amount { get; set; }

	public Fix64 Maximum { get; set; }

	public FResourceEntry DeepCopy()
	{
		return null;
	}
}
