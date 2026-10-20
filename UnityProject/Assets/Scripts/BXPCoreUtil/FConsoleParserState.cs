public enum FConsoleParserState
{
	Command = 0,
	WaitForParams = 1,
	Param = 2,
	WaitForNextParam = 3,
	WaitForEnd = 4
}
