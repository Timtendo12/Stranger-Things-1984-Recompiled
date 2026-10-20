using System;
using System.IO;
using System.Runtime.CompilerServices;

// Rewritten from libil2cpp.so 1.0.605 (FFileWatcherEntry @ 0x16fe220..0x16fe440, Ghidra vaddr). Debug-only hot-reload helper.
public class FFileWatcherEntry
{
	public string Filename;

	public DateTime LastWriteTime;

	[CompilerGenerated]
	private Action m_OnChanged;

	public event Action OnChanged
	{
		add
		{
			m_OnChanged = (Action)Delegate.Combine(m_OnChanged, value);
		}
		remove
		{
			m_OnChanged = (Action)Delegate.Remove(m_OnChanged, value);
		}
	}

	public bool CheckLastWriteTime(bool initial = false)
	{
		if (!File.Exists(Filename))
		{
			return false;
		}
		DateTime lastWrite = File.GetLastWriteTime(Filename);
		bool changed = !initial && lastWrite != LastWriteTime;
		LastWriteTime = lastWrite;
		return changed;
	}

	public void FireChangedEvent()
	{
		if (m_OnChanged != null)
		{
			m_OnChanged();
		}
	}
}
