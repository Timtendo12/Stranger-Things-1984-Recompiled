public enum FLineBreakStatus : byte
{
	MustBreakAndReverse = 0,
	MustBreak = 1,
	AllowBreak = 2,
	NoBreak = 3,
	InsideAChar = 4
}
