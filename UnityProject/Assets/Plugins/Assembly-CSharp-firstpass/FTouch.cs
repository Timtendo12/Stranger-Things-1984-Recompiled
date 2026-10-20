using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FTouch @ 0x16a0540..0x16a0728, Ghidra vaddr).
public struct FTouch
{
	public int FingerId;

	public int ButtonID;

	public Vector2 Position;

	public Vector2 RawPosition;

	public int TapCount;

	public TouchPhase Phase;

	public FTouch(Event e)
	{
		float inverse = 1f / Futile.DisplayScale;
		Vector2 mouse = e.mousePosition;
		float rawY = (float)PCDisplay.Height - mouse.y;
		FingerId = 0;
		RawPosition = new Vector2(mouse.x, rawY);
		Position = new Vector2((mouse.x - Futile.Screen.PixelWidth * Futile.Screen.OriginX) * inverse, (rawY - Futile.Screen.PixelHeight * Futile.Screen.OriginY) * inverse);
		ButtonID = e.button;
		TapCount = 1;
		switch (e.type)
		{
		case EventType.MouseDrag:
			Phase = TouchPhase.Moved;
			break;
		case EventType.MouseUp:
			Phase = TouchPhase.Ended;
			break;
		case EventType.MouseDown:
			Phase = TouchPhase.Began;
			break;
		default:
			FAssert.Fail("unexpected mouse event type: " + e.type.ToString());
			Phase = TouchPhase.Moved;
			break;
		}
	}

	public override string ToString()
	{
		return "finger=" + FingerId.ToString() + ", Button=" + ButtonID.ToString() + ", phase=" + Phase.ToString() + ", Position=" + Position.ToString() + ", RawPos=" + RawPosition.ToString() + ", TC=" + TapCount.ToString();
	}
}
