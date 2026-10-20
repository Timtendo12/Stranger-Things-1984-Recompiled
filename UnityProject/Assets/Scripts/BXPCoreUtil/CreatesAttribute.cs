using System;

public class CreatesAttribute : Attribute
{
	protected Type mCreatesType;

	public Type CreatesType
	{
		get
		{
			return mCreatesType;
		}
	}

	public CreatesAttribute(Type createsType)
	{
		mCreatesType = createsType;
	}
}
