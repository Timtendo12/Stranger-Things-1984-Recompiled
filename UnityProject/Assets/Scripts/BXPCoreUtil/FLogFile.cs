using System.Collections.Generic;
using System.IO;

public class FLogFile
{
	public delegate void UpdateLog(ref string text);

	protected StreamWriter mStream;

	protected static object mLock;

	protected List<UpdateLog> mUpdateLogHandlers;

	public string Filename => null;

	public FLogFile(string filename)
	{
	}

	public void Output(string text)
	{
	}

	public void Close()
	{
	}

	public void AddUpdateLogHandler(UpdateLog handler, bool endOfList)
	{
	}

	public void RemoveUpdateLogHandler(UpdateLog handler)
	{
	}

	protected void UpdateLogMessage(ref string text)
	{
	}
}
