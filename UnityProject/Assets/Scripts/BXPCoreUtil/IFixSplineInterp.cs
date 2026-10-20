public interface IFixSplineInterp<T>
{
	T Add(T b);

	T Subtract(T b);

	T MultiplyScalar(Fix64 a);

	void BinWrite(FBinWriter writer);

	void BinRead(FBinReader reader);
}
