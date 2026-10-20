using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FWebRequest
{
	public enum RequestTypes
	{
		GET = 0,
		POST = 1
	}

	public enum Statuses
	{
		Unsent = 0,
		Waiting = 1,
		Success = 2,
		Canceled = 3,
		TimedOut = 4,
		Error = 5
	}

	protected RequestTypes mRequestType;

	public static long DefaultTimeoutMS;

	protected long mTimeoutMS;

	public Action<FWebRequest> OnComplete;

	protected Dictionary<string, string> mParameters;

	protected WWW mWWW;

	protected Stopwatch mStopwatch;

	protected Statuses mStatus;

	public string BaseUrl { get; set; }

	public RequestTypes RequestType
	{
		get
		{
			return RequestTypes.GET;
		}
		set
		{
		}
	}

	public long TimeoutMS
	{
		get
		{
			return 0L;
		}
		set
		{
		}
	}

	public long ElapsedTimeMS => 0L;

	public bool TimedOut => false;

	public Statuses Status => Statuses.Unsent;

	public bool Success => false;

	public string ResponseString => null;

	public string ErrorString => null;

	public FWebRequest(string baseUrl, params string[] keysAndValues)
	{
	}

	public static FWebRequest Get(string baseUrl, params string[] keysAndValues)
	{
		return null;
	}

	public static FWebRequest Post(string baseUrl, params string[] keysAndValues)
	{
		return null;
	}

	public void AddParameter(string key, string val)
	{
	}

	public void AddParameters(params string[] keysAndValues)
	{
	}

	public void Send(Action<FWebRequest> onComplete)
	{
	}

	public void Send()
	{
	}

	public bool Update()
	{
		return false;
	}

	public void Cancel()
	{
	}
}
