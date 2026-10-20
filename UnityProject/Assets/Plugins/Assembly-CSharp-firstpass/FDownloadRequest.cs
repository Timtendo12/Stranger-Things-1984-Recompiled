public class FDownloadRequest
{
	protected int mResponseCode;

	protected int mLastDownloadedBytes;

	public string Url { get; protected set; }

	public string DestFilename { get; protected set; }

	public bool IsError { get; protected set; }

	public string ErrorMessage { get; protected set; }

	public bool IsSuccess { get; protected set; }

	public bool WasAborted { get; protected set; }

	public bool IsDone => false;

	public int InstanceID { get; protected set; }

	public int ResponseCode => 0;

	public int DownloadedBytes => 0;

	public FDownloadRequest Download(string url, string destFilename)
	{
		return null;
	}

	public FDownloadRequest(string url, string destFilename)
	{
	}

	public void Start()
	{
	}

	public void Abort()
	{
	}

	public void SetError(string msg)
	{
	}

	public void SetSuccess()
	{
	}
}
