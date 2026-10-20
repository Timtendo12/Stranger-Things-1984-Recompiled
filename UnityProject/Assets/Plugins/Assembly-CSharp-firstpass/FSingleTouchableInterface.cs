public interface FSingleTouchableInterface
{
	string LogName { get; }

	int TouchPriority { get; }

	FNode SingleTouchableNode { get; }

	bool WantAdditionalTouches { get; }

	bool HandleSingleTouchBegan(FTouch touch, FSingleTouchableInterface ignoreTouchable = null);

	void HandleSingleTouchMoved(FTouch touch);

	void HandleSingleTouchEnded(FTouch touch);

	void HandleSingleTouchCanceled(FTouch touch);

	void HandleAdditionalTouch(FTouch touch);

	bool CanHandleTouch(FTouch touch, FSingleTouchableInterface ignoreTouchable = null);
}
