// Rewritten from libil2cpp.so 1.0.605 (FEntityActionSetAnimatorParam<T> @ 0x1f65f18..0x1f66104, Ghidra vaddr).
public abstract class FEntityActionSetAnimatorParam<T> : FEntityAction
{
	protected string mParameterName;

	protected T mParameterValue;

	protected bool mRecursive;

	public string ParameterName => mParameterName;

	public T ParameterValue => mParameterValue;

	public bool Recursive => mRecursive;

	public FEntityActionSetAnimatorParam(string parameterName, T parameterValue, bool recursive = true)
	{
		mParameterName = parameterName;
		mParameterValue = parameterValue;
		mRecursive = recursive;
	}
}
