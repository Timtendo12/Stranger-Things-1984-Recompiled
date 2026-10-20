using System;
using System.Runtime.CompilerServices;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FStatItem @ 0x17cf7c4..0x17cf930, Ghidra vaddr).
public abstract class FStatItem
{
	[CompilerGenerated]
	private Action m_ChangeEvent;

	public string Name { get; set; }

	public event Action ChangeEvent
	{
		add
		{
			m_ChangeEvent = (Action)Delegate.Combine(m_ChangeEvent, value);
		}
		remove
		{
			m_ChangeEvent = (Action)Delegate.Remove(m_ChangeEvent, value);
		}
	}

	public abstract void LevelStart();

	public abstract void SessionStart();

	public abstract void SaveXml(XmlElement root);

	public abstract void LoadXml(XmlElement root);

	public abstract void Log();

	protected void FireChangeEvent()
	{
		if (m_ChangeEvent != null)
		{
			m_ChangeEvent();
		}
	}
}
