// Rewritten from libil2cpp.so 1.0.605 (FNotification, Ghidra vaddr).
// PC port: no local push notifications on Windows; all entry points are no-ops.
public class FNotification
{
	public static void Create(string message, float secondsFromNow, int badgeNumber)
	{
	}

	public static void ClearAll()
	{
	}

	public static void SetBadgeNumber(int badgeNumber)
	{
	}
}
