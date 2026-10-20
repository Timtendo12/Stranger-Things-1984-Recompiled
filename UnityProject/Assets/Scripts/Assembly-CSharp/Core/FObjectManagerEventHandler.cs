using System.Runtime.CompilerServices;
using UnityEngine;

public class FObjectManagerEventHandler : MonoBehaviour
{
	public delegate void EventHandler();

	[CompilerGenerated]
	private EventHandler m_GetObject;

	[CompilerGenerated]
	private EventHandler m_ReleaseObject;

	public event EventHandler GetObject;

	public event EventHandler ReleaseObject;

	public void OnGetObject()
	{
	}

	public void OnReleaseObject()
	{
	}
}
