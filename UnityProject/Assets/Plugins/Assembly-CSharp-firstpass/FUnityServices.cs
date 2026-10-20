using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

// Rewritten from libil2cpp.so 1.0.605 (FUnityServices @ 0x1680f74..0x1681530, Ghidra vaddr).
public class FUnityServices : MonoBehaviour
{
	public static FUnityServices Instance;

	protected static FDispatcher mDispatcher = new FDispatcher();

	protected static FWorkQueue mWorkQueue = new FWorkQueue(4);

	public static FDispatcher Dispatcher => mDispatcher;

	public static FWorkQueue WorkQueue => mWorkQueue;

	public static void Init()
	{
		if (Application.isPlaying)
		{
			GameObject gameObject = new GameObject("zPluginUnityServices");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			Instance = gameObject.AddComponent<FUnityServices>();
		}
	}

	public void Update()
	{
		mDispatcher.ExecutePendingActions(true);
		mWorkQueue.Update();
	}

	public static void ExecuteOnMainThread(Action action)
	{
		mDispatcher.Invoke(action);
	}

	public static void WaitForUnityWebRequest(UnityWebRequest uwr, Action<UnityWebRequest> onFinished)
	{
		Instance.StartCoroutine(DoWaitForUnityWebRequest(uwr, onFinished));
	}

	protected static IEnumerator DoWaitForUnityWebRequest(UnityWebRequest uwr, Action<UnityWebRequest> onFinished)
	{
		uwr.SendWebRequest();
		while (!uwr.isDone && uwr.result == UnityWebRequest.Result.InProgress)
		{
			yield return null;
		}
		onFinished(uwr);
	}
}
