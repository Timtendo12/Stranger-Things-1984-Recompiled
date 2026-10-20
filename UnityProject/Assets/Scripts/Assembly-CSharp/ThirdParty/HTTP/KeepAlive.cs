using System.Collections;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (KeepAlive @ 0x1776950..0x1776e6c, Ghidra vaddr).
// Developer HTTP stress test against a local server; the HTTP library is not part of the PC port, so the request loop is a no-op.
public class KeepAlive : MonoBehaviour
{
	private string url = "http://gamma.local:1337/";

	public int count = 30;

	private IEnumerator Start()
	{
		// TODO(port): shipped code fires `count` HTTP.Request GETs at `url` and logs the responses (0x1776a48) [left: HTTP keep-alive test, network]
		Debug.Log("Finished.");
		yield break;
	}
}
