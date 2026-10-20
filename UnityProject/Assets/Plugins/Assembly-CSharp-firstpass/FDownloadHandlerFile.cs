using System;
using System.IO;
using UnityEngine.Networking;

public class FDownloadHandlerFile : DownloadHandlerScript
{
	[ThreadStatic]
	protected static byte[] mReceiveBuffer;

	protected string mFilename;

	protected int mDownloadedBytes;

	protected FileStream mFileStream;

	protected int mContentLength;

	public int DownloadedBytes => 0;

	public static FDownloadHandlerFile Create(string filename)
	{
		return null;
	}

	protected FDownloadHandlerFile(string filename)
		: base(null)
	{
	}

	protected override bool ReceiveData(byte[] data, int dataLength)
	{
		return false;
	}

	protected override void CompleteContent()
	{
	}

	public new void Dispose()
	{
	}

	protected override void ReceiveContentLength(int contentLength)
	{
	}

	protected override float GetProgress()
	{
		return 0f;
	}
}
