public interface ISplineHelper<T>
{
	T Add(T a, T b);

	T Subtract(T a, T b);

	T MultiplyScalar(float a, T b);
}
