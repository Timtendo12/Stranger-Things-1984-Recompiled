public interface IAnimTagResponder
{
	void FireTag(FAnimTag tag, int actionID);

	void FireLoopTag(int actionID);

	void FireEndTag(int actionID);
}
