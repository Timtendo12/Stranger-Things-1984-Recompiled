using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using Newtonsoft.Json;

// Rewritten from libil2cpp.so 1.0.605 (FStat @ 0x17d0318..0x17d1a20, Ghidra vaddr).
public class FStat
{
	[CompilerGenerated]
	private Action m_StatChangeEvent;

	public FStatItemCounterInt ServerRequests = new FStatItemCounterInt();

	public FStatItemCounterInt WinGame = new FStatItemCounterInt();

	public FStatItemCounterInt WinCoOp = new FStatItemCounterInt();

	public FStatItemCounterInt WinPvP = new FStatItemCounterInt();

	public FStatItemCounterInt RabbitSummons = new FStatItemCounterInt();

	public FStatItemCounterInt PirateSummons = new FStatItemCounterInt();

	public FStatItemCounterInt PeasantKills = new FStatItemCounterInt();

	public FStatItemCounterInt HeroKills = new FStatItemCounterInt();

	public FStatItemCounterInt TotemKills = new FStatItemCounterInt();

	public FStatItemCounterInt CardsPlayed = new FStatItemCounterInt();

	public FStatItemCounterInt CheapCardsPlayed = new FStatItemCounterInt();

	public FStatItemCounterInt ExpensiveCardsPlayed = new FStatItemCounterInt();

	public FStatItemCounterInt SpellsPlayed = new FStatItemCounterInt();

	public FStatItemCounterInt StepsWalked = new FStatItemCounterInt();

	public FStatItemCounterInt DamageDone = new FStatItemCounterInt();

	public FStatItemCounterInt DamageReceived = new FStatItemCounterInt();

	protected Dictionary<string, FStatItem> mItems = new Dictionary<string, FStatItem>();

	protected int mSessionCount;

	protected int mLifetimeSessionTime;

	[JsonProperty]
	public int SessionCount
	{
		get
		{
			return mSessionCount;
		}
		protected set
		{
			mSessionCount = value;
			if (m_StatChangeEvent != null)
			{
				m_StatChangeEvent();
			}
		}
	}

	[JsonProperty]
	public int LifetimeSessionTime
	{
		get
		{
			return mLifetimeSessionTime;
		}
		protected set
		{
			mLifetimeSessionTime = value;
			if (m_StatChangeEvent != null)
			{
				m_StatChangeEvent();
			}
		}
	}

	public event Action StatChangeEvent
	{
		add
		{
			m_StatChangeEvent = (Action)Delegate.Combine(m_StatChangeEvent, value);
		}
		remove
		{
			m_StatChangeEvent = (Action)Delegate.Remove(m_StatChangeEvent, value);
		}
	}

	public void PreInit()
	{
		mItems.Clear();
		mSessionCount = 0;
		Init();
	}

	public void Init()
	{
		FieldInfo[] fields = typeof(FStat).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
		if (fields == null)
		{
			return;
		}
		for (int i = 0; i < fields.Length; i++)
		{
			FieldInfo fieldInfo = fields[i];
			if (typeof(FStatItem).IsAssignableFrom(fieldInfo.FieldType))
			{
				FStatItem fStatItem = fieldInfo.GetValue(this) as FStatItem;
				mItems[fieldInfo.Name] = fStatItem;
				fStatItem.Name = fieldInfo.Name;
				fStatItem.ChangeEvent -= OnStatChange;
				fStatItem.ChangeEvent += OnStatChange;
			}
		}
	}

	public FStatItem GetOrCreateStat<T>(string name) where T : FStatItem, new()
	{
		FStatItem stat = GetStat(name);
		if (stat == null)
		{
			stat = new T();
			mItems[name] = stat;
			stat.Name = name;
		}
		return stat;
	}

	public FStatItem GetStat(string name)
	{
		FStatItem value = null;
		if (mItems != null)
		{
			mItems.TryGetValue(name, out value);
		}
		return value;
	}

	public void LevelStart()
	{
		foreach (KeyValuePair<string, FStatItem> mItem in mItems)
		{
			mItem.Value.LevelStart();
		}
	}

	public void SessionStart()
	{
		FLog.UnityLog("FStat.SessionStart");
		mSessionCount++;
		if (m_StatChangeEvent != null)
		{
			m_StatChangeEvent();
		}
		foreach (KeyValuePair<string, FStatItem> mItem in mItems)
		{
			mItem.Value.SessionStart();
		}
	}

	public void Log()
	{
		foreach (KeyValuePair<string, FStatItem> mItem in mItems)
		{
			mItem.Value.Log();
		}
	}

	public void SaveXml(XmlElement root)
	{
		XmlElement xmlElement = root.AddChild("Stats", string.Empty);
		xmlElement.AddAttribute("SessionCount", mSessionCount);
		xmlElement.AddAttribute("LifetimeSessionTime", mLifetimeSessionTime);
		foreach (KeyValuePair<string, FStatItem> mItem in mItems)
		{
			XmlElement xmlElement2 = xmlElement.AddChild("Stat", string.Empty, "Name", mItem.Key);
			xmlElement2.AddAttribute("Type", mItem.Value.GetType().ToString());
			mItem.Value.SaveXml(xmlElement2);
		}
	}

	public Type GetTypeFromName(string typeName)
	{
		if (typeName == "FStatItemCounterInt")
		{
			return typeof(FStatItemCounterInt);
		}
		return null;
	}

	public void LoadXml(XmlElement root)
	{
		XmlElement child = root.GetChild("Stats");
		if (child == null)
		{
			return;
		}
		mSessionCount = child.GetAttrib("SessionCount", mSessionCount);
		mLifetimeSessionTime = child.GetAttrib("LifetimeSessionTime", mLifetimeSessionTime);
		foreach (XmlNode childNode in child.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement xmlElement = childNode as XmlElement;
			string attrib = xmlElement.GetAttrib("Name");
			if (attrib == null)
			{
				continue;
			}
			string attrib2 = xmlElement.GetAttrib("Type");
			if (attrib2 == null)
			{
				continue;
			}
			Type typeFromName = GetTypeFromName(attrib2);
			if (typeFromName == null)
			{
				FLog.UnityError("unknown type " + attrib2 + " for " + attrib);
				continue;
			}
			FStatItem fStatItem = GetStat(attrib);
			if (fStatItem == null)
			{
				fStatItem = Activator.CreateInstance(typeFromName) as FStatItem;
				mItems[attrib] = fStatItem;
				fStatItem.Name = attrib;
			}
			else if (typeFromName != fStatItem.GetType())
			{
				FLog.UnityError("stat item " + attrib + " has mismatched type");
				continue;
			}
			fStatItem.LoadXml(xmlElement);
		}
	}

	protected void OnStatChange()
	{
		if (m_StatChangeEvent != null)
		{
			m_StatChangeEvent();
		}
	}

	public void TrackCardPlayed(string cardName)
	{
	}

	public void TrackStepsWalked(int count)
	{
		if (StepsWalked != null)
		{
			StepsWalked.Add(count);
		}
	}

	public void TrackDamageDone(int count)
	{
		if (DamageDone != null)
		{
			DamageDone.Add(count);
		}
	}

	public void TrackDamageReceived(int count)
	{
		if (DamageReceived != null)
		{
			DamageReceived.Add(count);
		}
	}
}
