using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPlayerData @ 0x17b3234..0x17be3d0, Ghidra vaddr).
// PC port: the cloud-save branches are kept, but CloudSaveManager never delivers data, so they stay inert.
public class FPlayerData
{
	public enum Currency
	{
		None = 0,
		Money = 1,
		Credits = 2
	}

	public const string cDefaultFilenameSuffix = "Online";

	public const string cDefaultFilenameFormat = "PlayerData_{0}.xml";

	public static int CurrentVersion = 18;

	public static float DefaultEffectVolume = 0.5f;

	public static float DefaultMusicVolume = 0.2f;

	public static float DefaultDoubleTapTimeout = 0.5f;

	public static float DefaultPopupTimeout = 0.4f;

	public static float DefaultPopupDistanceLimit = 40f;

	public static float DefaultPushScrollAreaSize = 2f;

	public static bool DefaultPushScroll = true;

	public static bool DefaultPushScrollDuringDragSelect = false;

	public static float DefaultKeyScrollSpeed = 0.5f;

	public static float DefaultKeyScrollAcceleration = 0.5f;

	public static float DefaultMouseScrollSpeed = 0.5f;

	public static float DefaultMouseScrollAcceleration = 0.5f;

	public static bool DefaultForceDrag = true;

	public static bool DefaultAllowZoom = false;

	public static int DefaultShadowQuality = 4;

	public static int DefaultResolutionWidth = 1080;

	public static int DefaultResolutionHeight = 1920;

	public static int DefaultRefreshRate = 60;

	public static int cConsoleCommandLimit = 25;

	protected int mVersion;

	protected FSaveFile mSaveFile;

	protected string mFilename;

	public bool PreferKeychain;

	public bool HadCorruptedSaveData;

	protected internal bool mUseCloudSave;

	protected int mShadowQuality = DefaultShadowQuality;

	protected int mResolutionWidth;

	protected int mResolutionHeight = DefaultResolutionHeight;

	protected int mRefreshRate;

	protected bool mFullScreen = true;

	protected bool mConstrainCursor = true;

	protected bool mEnableAA = true;

	protected bool mEnableBloom = true;

	protected bool mEnableSSAO = true;

	protected bool mEnableGibs = true;

	protected Dictionary<string, bool> mNewThings = new Dictionary<string, bool>();

	protected List<string> mConsoleCommands = new List<string>();

	public bool BlockSaves;

	protected internal bool mIsSavingToCS;

	public static int cBugReportNameMaxLength = 32;

	protected XmlDocument mLoadedXmlDoc;

	public int Version => mVersion;

	public string ProfileId { get; set; }

	public string Filename
	{
		get
		{
			return mFilename;
		}
		set
		{
			mFilename = value;
			mSaveFile = new FSaveFile(mFilename, true, true, PreferKeychain, true, true);
		}
	}

	private static string DefaultFilename => MakeFilename("Online");

	public bool HasLoadedCloudSaveData
	{
		get
		{
			if (!mUseCloudSave)
			{
				return false;
			}
			return CloudSaveManager.Instance.CSPlayerData != null;
		}
	}

	public float EffectVolume { get; set; }

	public float MusicVolume { get; set; }

	public float DoubleTapTimeout { get; set; }

	public float PopupTimeout { get; set; }

	public DateTime InstalledDateTime { get; set; }

	public Guid PlayerGuid { get; set; }

	public int TipIndex { get; set; }

	public int LastNewsItemSeen { get; set; }

	public int HighestBuildNumberSeen { get; set; }

	public float PushScrollAreaSize { get; set; }

	public bool PushScroll { get; set; }

	public float KeyScrollSpeed { get; set; }

	public float KeyScrollAcceleration { get; set; }

	public float MouseScrollSpeed { get; set; }

	public float MouseScrollAcceleration { get; set; }

	public bool ForceDrag { get; set; }

	public bool AllowZoom { get; set; }

	public string[] PermanentUnlocks { get; protected set; }

	public bool HasCheckedForLostSaves { get; set; }

	public int ShadowQuality
	{
		get
		{
			return mShadowQuality;
		}
		set
		{
			mShadowQuality = value;
			FApp.UpdateQualitySettings();
		}
	}

	public int ResolutionWidth => mResolutionWidth;

	public int ResolutionHeight => mResolutionHeight;

	public int RefreshRate => mRefreshRate;

	public bool FullScreen => mFullScreen;

	public bool ConstrainCursor => mConstrainCursor;

	public bool EnableAA => mEnableAA;

	public bool EnableBloom => mEnableBloom;

	public bool EnableSSAO => mEnableSSAO;

	public bool EnableGibs
	{
		get
		{
			return mEnableGibs;
		}
		set
		{
			mEnableGibs = value;
		}
	}

	public Dictionary<string, bool> NewThings => mNewThings;

	public List<string> ConsoleCommands => mConsoleCommands;

	public bool HasLoaded { get; set; }

	public bool NeedSave { get; set; }

	public bool QueuedSave { get; set; }

	public bool IsSavingToCS
	{
		get
		{
			if (!mUseCloudSave)
			{
				return false;
			}
			return mIsSavingToCS;
		}
	}

	public string BugReportName { get; set; }

	public string EmailAddress { get; set; }

	public XmlDocument LoadedXmlDoc => mLoadedXmlDoc;

	protected event Action mNewThingsChangedEvent;

	public FPlayerData()
	{
	}

	public void AddNewThingsChangedEvent(Action action)
	{
		RemoveNewThingsChangedEvent(action);
		mNewThingsChangedEvent += action;
	}

	public void RemoveNewThingsChangedEvent(Action action)
	{
		mNewThingsChangedEvent -= action;
	}

	private void PreInit()
	{
		InstalledDateTime = DateTime.MinValue;
		PlayerGuid = Guid.Empty;
		Filename = MakeFilename(ProfileId);
		mUseCloudSave = true;
	}

	private static string MakeFilename(string suffix)
	{
		return string.Format("PlayerData_{0}.xml", suffix);
	}

	private void Init(bool fromLoad)
	{
		mVersion = CurrentVersion;
		if (!fromLoad)
		{
			PreInit();
		}
		BugReportName = "[Enter Name Please]";
		EffectVolume = DefaultEffectVolume;
		MusicVolume = DefaultMusicVolume;
		HighestBuildNumberSeen = -1;
		TipIndex = 0;
		LastNewsItemSeen = -1;
		DoubleTapTimeout = DefaultDoubleTapTimeout;
		PopupTimeout = DefaultPopupTimeout;
		mShadowQuality = DefaultShadowQuality;
		mResolutionWidth = DefaultResolutionWidth;
		mResolutionHeight = DefaultResolutionHeight;
		mRefreshRate = DefaultRefreshRate;
		PushScroll = DefaultPushScroll;
		PushScrollAreaSize = DefaultPushScrollAreaSize;
		mFullScreen = true;
		mEnableAA = true;
		mEnableBloom = true;
		mEnableSSAO = true;
		mEnableGibs = true;
		KeyScrollSpeed = DefaultKeyScrollSpeed;
		KeyScrollAcceleration = DefaultKeyScrollAcceleration;
		MouseScrollSpeed = DefaultMouseScrollSpeed;
		MouseScrollAcceleration = DefaultMouseScrollAcceleration;
		ForceDrag = DefaultForceDrag;
		AllowZoom = DefaultAllowZoom;
		PermanentUnlocks = new string[0];
		HasCheckedForLostSaves = false;
		ResetNewThings();
		mConsoleCommands.Clear();
		EmailAddress = string.Empty;
		if (!fromLoad)
		{
			Save();
		}
	}

	public void Save()
	{
		FLog.UnityLog("FPlayerData::Save() called");
		NeedSave = true;
	}

	public async void RealSave(string toBackupFilename = null, bool blockUntilDone = false)
	{
		if (!HasLoaded || BlockSaves)
		{
			return;
		}
		if (toBackupFilename == null && mSaveFile.SaveInProgess)
		{
			QueuedSave = true;
			return;
		}
		QueuedSave = false;
		FLog.Log("SAVEDATALOGGING: Starting RealSave.", true);
		if (mUseCloudSave)
		{
			mIsSavingToCS = true;
			await CloudSaveManager.Instance.SavePlayerDataToCloudSaveData(this);
			mIsSavingToCS = false;
		}
		RealSaveXMLToLocal(toBackupFilename, blockUntilDone);
		FLog.Log("SAVEDATALOGGING: RealSave complete.", true);
		if (toBackupFilename == null)
		{
			NeedSave = false;
		}
	}

	private void RealSaveXMLToLocal(string toBackupFilename = null, bool blockUntilDone = false)
	{
		XmlElement xmlElement = FCoreUtil.CreateXmlDoc("LegendSave", "Version", CurrentVersion);
		xmlElement.AddChild("EffectVolume", EffectVolume);
		xmlElement.AddChild("MusicVolume", MusicVolume);
		xmlElement.AddChild("BugReportName", BugReportName);
		xmlElement.AddChild("EmailAddress", EmailAddress);
		xmlElement.AddChild("ResolutionWidth", mResolutionWidth);
		xmlElement.AddChild("ResolutionHeight", mResolutionHeight);
		xmlElement.AddChild("RefreshRate", mRefreshRate);
		xmlElement.AddChild("FullScreen", mFullScreen);
		xmlElement.AddChild("ConstrainCursor", mConstrainCursor);
		xmlElement.AddChild("InstalledDateTime", InstalledDateTime);
		xmlElement.AddChild("DoubleTapTimeout", DoubleTapTimeout);
		xmlElement.AddChild("PopupTimeout", PopupTimeout);
		xmlElement.AddChild("HighestBuildNumberSeen", HighestBuildNumberSeen);
		xmlElement.AddChild("PushScroll", PushScroll);
		xmlElement.AddChild("PushScrollAreaSize", PushScrollAreaSize);
		xmlElement.AddChild("KeyScrollSpeed", KeyScrollSpeed);
		xmlElement.AddChild("KeyScrollAcceleration", KeyScrollAcceleration);
		xmlElement.AddChild("MouseScrollSpeed", MouseScrollSpeed);
		xmlElement.AddChild("MouseScrollAcceleration", MouseScrollAcceleration);
		xmlElement.AddChild("SavedState", FApp.PlayerStats.Serialize());
		xmlElement.AddChild("PermanentUnlocks", string.Join("|", PermanentUnlocks));
		xmlElement.AddChild("HasCheckedForLostSaves", HasCheckedForLostSaves);
		SaveConsoleCommands(xmlElement);
		if (toBackupFilename == null)
		{
			mSaveFile.Save(xmlElement, blockUntilDone);
		}
		else
		{
			FSaveFile fSaveFile = new FSaveFile(toBackupFilename, false, false, false, false, false);
			fSaveFile.Save(xmlElement.ConvertDocToString(true), blockUntilDone);
		}
		mSaveFile.SavePlayerPrefsBackup();
	}

	protected void SaveConsoleCommands(XmlElement root)
	{
		XmlElement element = root.AddChild("ConsoleCommands", (string)null);
		for (int i = 0; i < mConsoleCommands.Count; i++)
		{
			element.AddChild("Command", (string)null, "Name", mConsoleCommands[i]);
		}
	}

	public async Task<int> PreLoad(bool dumpData = false, bool shouldPreInit = true)
	{
		if (shouldPreInit)
		{
			PreInit();
		}
		int xmlVersion = LoadVersionFromXMLSaveFile(dumpData);
		if (!mUseCloudSave)
		{
			return xmlVersion;
		}
		CloudSaveManager csManager = CloudSaveManager.Instance;
		await csManager.PreLoadPlayerData();
		CSPlayerData csData = csManager.CSPlayerData;
		if (csData == null)
		{
			return xmlVersion;
		}
		if (csData.PlayerGuid != null)
		{
			PlayerGuid = new Guid(csData.PlayerGuid);
		}
		InstalledDateTime = csData.InstalledDateTime;
		if (InstalledDateTime == DateTime.MinValue || PlayerGuid == Guid.Empty)
		{
			HandleInstall();
		}
		HasLoaded = false;
		FLog.UnityLog(string.Format("PreLoad Cloud Save Player Data {0}", csData.Version));
		return csData.Version;
	}

	private int LoadVersionFromXMLSaveFile(bool dumpData = false)
	{
		int num;
		if (mSaveFile.CycleCount == 0)
		{
			num = -1;
		}
		else
		{
			byte b = 0;
			do
			{
				bool flag = mSaveFile.DoesSaveFileExist();
				num = TryLoad();
				if (!dumpData)
				{
					if (num >= 0)
					{
						FLog.Log(string.Format("PreLoad with {0}", num));
						return num;
					}
					mSaveFile.RewindCyclePosition();
					FLog.Error("SAVEDATALOGGING: PlayerData invalid! Trying cycle position: " + mSaveFile.CyclePosition);
					if (b == 0 && flag)
					{
						HadCorruptedSaveData = true;
						FLog.Error("SAVEDATALOGGING: Data invalid but file does exist!");
					}
				}
				else
				{
					FLog.Warning("SAVEDATALOGGING: START Player data dump for cycle position : " + mSaveFile.CyclePosition, true);
					DebugLogLoadedXmlDoc();
					FLog.Warning("SAVEDATALOGGING: END Player data dump for cycle position : " + mSaveFile.CyclePosition, true);
					mSaveFile.RewindCyclePosition();
					if (b == 0 && flag)
					{
						HadCorruptedSaveData = true;
					}
				}
				b++;
			}
			while (b < mSaveFile.CycleCount);
		}
		if (dumpData || num == -1)
		{
			if (!dumpData)
			{
				FLog.Log("SAVEDATALOGGING: No valid PlayerData found. Trying playerprefs backup.");
				num = TryLoad(true);
				if (num != -1)
				{
					FLog.Log("SAVEDATALOGGING: Playerprefs backup valid.");
					FLog.Log(string.Format("PreLoad with {0}", num));
					return num;
				}
				FLog.Log("SAVEDATALOGGING: No valid PlayerData found. Starting fresh.");
			}
			else
			{
				num = TryLoad(true);
				FLog.Warning("SAVEDATALOGGING: START Player data dump from PlayerPrefs", true);
				DebugLogLoadedXmlDoc();
				FLog.Warning("SAVEDATALOGGING: END Player data dump from PlayerPrefs", true);
				if (num != -1)
				{
					FLog.Log(string.Format("PreLoad with {0}", num));
					return num;
				}
			}
			HandleInstall();
			num = -1;
		}
		FLog.Log(string.Format("PreLoad with {0}", num));
		return num;
	}

	protected int TryLoad(bool fromPlayerPrefs = false)
	{
		try
		{
			mSaveFile.Load(fromPlayerPrefs);
			string dataAsUTF8String = mSaveFile.DataAsUTF8String;
			if (!dataAsUTF8String.Valid())
			{
				mLoadedXmlDoc = null;
			}
			else
			{
				mLoadedXmlDoc = new XmlDocument();
				mLoadedXmlDoc.LoadXml(dataAsUTF8String);
				if (mLoadedXmlDoc.DocumentElement != null)
				{
					return PreLoadXml(mLoadedXmlDoc.DocumentElement);
				}
				FLog.Error("SAVEDATALOGGING: Failed to load/parse player data");
			}
		}
		catch (Exception ex)
		{
			// The decompiled body is wrapped in a try/catch that falls through to -1.
			FLog.Error("SAVEDATALOGGING: Failed to load/parse player data: " + ex.Message);
			mLoadedXmlDoc = null;
		}
		return -1;
	}

	protected void DebugLogLoadedXmlDoc()
	{
		if (mLoadedXmlDoc == null)
		{
			FLog.Warning("mLoadedXmlDoc is null!!", true);
			return;
		}
		using (StringWriter stringWriter = new StringWriter())
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
			{
				mLoadedXmlDoc.WriteTo(xmlWriter);
				xmlWriter.Flush();
				FLog.Warning(stringWriter.GetStringBuilder().ToString(), true);
			}
		}
	}

	public async void Load(bool split)
	{
		if (!split)
		{
			await PreLoad(false, true);
		}
		Init(mLoadedXmlDoc != null || HasLoadedCloudSaveData);
		HasLoaded = true;
		if (!HasLoadedCloudSaveData)
		{
			if (mLoadedXmlDoc != null && mLoadedXmlDoc.DocumentElement != null)
			{
				LoadXml(mLoadedXmlDoc.DocumentElement);
				UnityEngine.Debug.Log("Load with XML " + mSaveFile.GetFileName() + " :: Data = " + mSaveFile.DataAsUTF8String);
			}
		}
		else
		{
			UnityEngine.Debug.Log(string.Format("Load with CS: {0}", CloudSaveManager.Instance.CSPlayerData));
			LoadCloudSaveData(CloudSaveManager.Instance.CSPlayerData);
		}
		ApplyVolumes();
		UpdateResolutionFromCurrent();
		FApp.UpdateQualitySettings();
	}

	public void LoadCloudSaveData(CSPlayerData data)
	{
		mVersion = data.Version;
		EffectVolume = data.EffectVolume;
		MusicVolume = data.MusicVolume;
		InstalledDateTime = data.InstalledDateTime;
		if (data.PlayerGuid != null)
		{
			PlayerGuid = new Guid(data.PlayerGuid);
		}
		if (InstalledDateTime == DateTime.MinValue || PlayerGuid == Guid.Empty)
		{
			HandleInstall();
		}
		EmailAddress = data.EmailAddress;
		BugReportName = data.BugReportName;
		mNewThings = JsonUtility.FromJson<Dictionary<string, bool>>(data.NewThingsData);
		DoubleTapTimeout = data.DoubleTapTimeout;
		PopupTimeout = data.PopupTimeout;
		TipIndex = data.TipIndex;
		LastNewsItemSeen = data.LastNewsItemSeen;
		HighestBuildNumberSeen = data.HighestBuildNumberSeen;
		PushScroll = data.PushScroll;
		PushScrollAreaSize = ((mVersion < 6) ? DefaultPushScrollAreaSize : data.PushScrollAreaSize);
		KeyScrollSpeed = data.KeyScrollSpeed;
		KeyScrollAcceleration = data.KeyScrollAcceleration;
		MouseScrollSpeed = data.MouseScrollSpeed;
		MouseScrollAcceleration = data.MouseScrollAcceleration;
		mShadowQuality = data.ShadowQuality;
		mResolutionWidth = data.ResolutionWidth;
		mResolutionHeight = data.ResolutionHeight;
		mRefreshRate = data.RefreshRate;
		mFullScreen = data.FullScreen;
		mConstrainCursor = data.ConstrainCursor;
		mEnableAA = data.EnableAA;
		mEnableBloom = data.EnableBloom;
		mEnableSSAO = data.EnableSSAO;
		mEnableGibs = data.EnableGibs;
		ForceDrag = data.ForceDrag;
		AllowZoom = data.AllowZoom;
		string permanentUnlocksData = data.PermanentUnlocksData;
		if (permanentUnlocksData.Valid())
		{
			PermanentUnlocks = permanentUnlocksData.Split('|');
		}
		HasCheckedForLostSaves = data.HasCheckedForLostSaves;
		mConsoleCommands = new List<string>();
		List<string> list = JsonUtility.FromJson<List<string>>(data.ConsoleCommands);
		if (list != null)
		{
			foreach (string item in list)
			{
				// TODO(port): decompile adds the entry when Valid() is false (0x17bace8); kept sensible (add valid ones). Unreachable on PC. [left: unreachable on PC]
				if (item.Valid())
				{
					mConsoleCommands.Add(item);
				}
			}
		}
		if (FApp.PlayerStats != null && data.PlayerSavedStateData.Valid())
		{
			CloudSaveManager.Instance.DeserializePlayerSavedStats(data.PlayerSavedStateData);
		}
		FLog.UnityLog("Load player data from Cloud Save data done.");
	}

	public int PreLoadXml(XmlElement root)
	{
		int attrib = root.GetAttrib("Version", 0);
		InstalledDateTime = root.GetChildValue("InstalledDateTime", DateTime.MinValue);
		string childValue = root.GetChildValue("PlayerGuid", (string)null);
		if (childValue != null)
		{
			PlayerGuid = new Guid(childValue);
		}
		if (InstalledDateTime == DateTime.MinValue || PlayerGuid == Guid.Empty)
		{
			HandleInstall();
		}
		return attrib;
	}

	public int LoadXml(XmlElement root)
	{
		mVersion = root.GetAttrib("Version", CurrentVersion);
		EffectVolume = root.GetChildValue("EffectVolume", DefaultEffectVolume);
		MusicVolume = root.GetChildValue("MusicVolume", DefaultMusicVolume);
		InstalledDateTime = root.GetChildValue("InstalledDateTime", DateTime.MinValue);
		string childValue = root.GetChildValue("PlayerGuid", (string)null);
		if (childValue != null)
		{
			PlayerGuid = new Guid(childValue);
		}
		if (InstalledDateTime == DateTime.MinValue || PlayerGuid == Guid.Empty)
		{
			HandleInstall();
		}
		EmailAddress = root.GetChildValue("EmailAddress", EmailAddress);
		BugReportName = root.GetChildValue("BugReportName", BugReportName);
		mNewThings = root.GetChildDictBool("NewThings", "Thing", "New");
		DoubleTapTimeout = root.GetChildValue("DoubleTapTimeout", DefaultDoubleTapTimeout);
		PopupTimeout = root.GetChildValue("PopupTimeout", DefaultPopupTimeout);
		TipIndex = root.GetChildValue("TipIndex", 0);
		LastNewsItemSeen = root.GetChildValue("LastNewsItemSeen", -1);
		HighestBuildNumberSeen = root.GetChildValue("HighestBuildNumberSeen", -1);
		PushScroll = root.GetChildValue("PushScroll", true);
		PushScrollAreaSize = ((mVersion < 6) ? DefaultPushScrollAreaSize : root.GetChildValue("PushScrollAreaSize", DefaultPushScrollAreaSize));
		KeyScrollSpeed = root.GetChildValue("KeyScrollSpeed", DefaultKeyScrollSpeed);
		KeyScrollAcceleration = root.GetChildValue("KeyScrollAcceleration", DefaultKeyScrollAcceleration);
		MouseScrollSpeed = root.GetChildValue("MouseScrollSpeed", DefaultMouseScrollSpeed);
		MouseScrollAcceleration = root.GetChildValue("MouseScrollAcceleration", DefaultMouseScrollAcceleration);
		mShadowQuality = root.GetChildValue("ShadowQuality", DefaultShadowQuality);
		mResolutionWidth = root.GetChildValue("ResolutionWidth", DefaultResolutionWidth);
		mResolutionHeight = root.GetChildValue("ResolutionHeight", DefaultResolutionHeight);
		mRefreshRate = root.GetChildValue("RefreshRate", DefaultRefreshRate);
		mFullScreen = root.GetChildValue("FullScreen", false);
		mConstrainCursor = root.GetChildValue("ConstrainCursor", false);
		mEnableAA = root.GetChildValue("EnableAA", true);
		mEnableBloom = root.GetChildValue("EnableBloom", true);
		mEnableSSAO = root.GetChildValue("EnableSSAO", true);
		mEnableGibs = root.GetChildValue("EnableGibs", true);
		ForceDrag = root.GetChildValue("ForceDrag", DefaultForceDrag);
		AllowZoom = root.GetChildValue("AllowZoom", DefaultAllowZoom);
		string childValue2 = root.GetChildValue("PermanentUnlocks", string.Empty);
		if (childValue2.Valid())
		{
			PermanentUnlocks = childValue2.Split('|');
		}
		HasCheckedForLostSaves = root.GetChildValue("HasCheckedForLostSaves", HasCheckedForLostSaves);
		LoadConsoleCommands(root);
		if (FApp.PlayerStats != null)
		{
			FApp.PlayerStats.Deserialize(root.GetChildValue("SavedState", string.Empty));
		}
		return mVersion;
	}

	protected void LoadConsoleCommands(XmlElement root)
	{
		XmlElement child = root.GetChild("ConsoleCommands");
		if (child == null)
		{
			return;
		}
		foreach (XmlNode childNode in child.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				string attrib = (childNode as XmlElement).GetAttrib("Name");
				if (attrib != null)
				{
					mConsoleCommands.Add(attrib);
				}
			}
		}
	}

	public void AddPermanentUnlock(string name)
	{
		if (PermanentUnlocks == null)
		{
			PermanentUnlocks = new string[0];
		}
		List<string> list = PermanentUnlocks.ToList();
		if (!list.Contains(name))
		{
			list.Add(name);
			PermanentUnlocks = list.ToArray();
		}
	}

	public void SaveGenericKey(string name, string value)
	{
		PreviewLabs.PlayerPrefs.SetString(name, value);
	}

	public string LoadGenericKey(string name, string defaultValue)
	{
		return PreviewLabs.PlayerPrefs.GetString(name, defaultValue);
	}

	public void ForceOptions()
	{
		ApplyVolumes();
	}

	public void ApplyVolumes()
	{
		if (FApp.SoundManager != null)
		{
			FApp.SoundManager.EffectVolume = EffectVolume;
			FApp.SoundManager.MusicVolume = MusicVolume;
		}
	}

	public void ClearAllExceptPurchases()
	{
		PreviewLabs.PlayerPrefs.DeleteAll();
		DateTime installedDateTime = InstalledDateTime;
		Guid playerGuid = PlayerGuid;
		Init(false);
		ApplyVolumes();
		FApp.ResetUpgradeableData();
		InstalledDateTime = installedDateTime;
		PlayerGuid = playerGuid;
		Save();
	}

	public bool IsNewThing(string thingName)
	{
		bool value;
		if (thingName != null && mNewThings.TryGetValue(thingName, out value))
		{
			return value;
		}
		return false;
	}

	public int GetNumberNewThings(string subString)
	{
		int num = 0;
		foreach (KeyValuePair<string, bool> mNewThing in mNewThings)
		{
			if (mNewThing.Key.Contains(subString))
			{
				num++;
			}
		}
		return num;
	}

	public void ResetNewThings(bool clear = true)
	{
		if (mNewThings != null)
		{
			if (clear)
			{
				mNewThings.Clear();
			}
		}
		else
		{
			mNewThings = new Dictionary<string, bool>();
		}
	}

	public void SetNewThing(string name, bool v)
	{
		bool flag = IsNewThing(name);
		if (!v)
		{
			if (!flag)
			{
				return;
			}
			mNewThings.Remove(name);
		}
		else
		{
			if (flag)
			{
				return;
			}
			mNewThings[name] = true;
		}
		if (this.mNewThingsChangedEvent != null)
		{
			this.mNewThingsChangedEvent();
		}
	}

	protected string BuildJSONUnlockSnippet(Dictionary<string, bool> unlockDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		bool flag = false;
		foreach (KeyValuePair<string, bool> item in unlockDict)
		{
			if (item.Value)
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(item.Key);
				flag = true;
				stringBuilder.Append("\",");
			}
		}
		if (flag)
		{
			stringBuilder.Length--;
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	protected void HandleInstall()
	{
		if (PlayerGuid == Guid.Empty)
		{
			PlayerGuid = FCoreUtil.GenerateRandomGuid();
		}
		if (InstalledDateTime == DateTime.MinValue)
		{
			InstalledDateTime = DateTime.UtcNow;
		}
		FGameUtil.StartCoroutine(WaitToReportInstall());
	}

	public IEnumerator WaitToReportInstall()
	{
		int failsafeCount = 10;
		while (FVersionInfo.CoreBuildNumber < 0 && failsafeCount > 0)
		{
			yield return null;
			failsafeCount--;
		}
		while (!FApp.InitComplete)
		{
			yield return null;
		}
		// PC port: the original reports the install to analytics here; nothing to do.
	}

	public float GetRandomSubsetScore(byte[] randomizer = null)
	{
		byte[] array = PlayerGuid.ToByteArray();
		if (randomizer != null)
		{
			array = FCoreUtil.SHA1Hash(array.Concat(randomizer));
		}
		return (float)BitConverter.ToUInt32(array, 0) / 4.2949673E+09f;
	}

	public bool IsInRandomSubset(float percent, byte[] randomizer = null)
	{
		return GetRandomSubsetScore(randomizer) < percent * 0.01f;
	}

	[Conditional("DEBUG_PLAYER_SAVE_DATA")]
	public void PDLog(string str)
	{
		if (!BlockSaves)
		{
			FLog.Log("PLAYER SAVE DATA: " + str);
		}
	}

	public void AddConsoleCommand(string command)
	{
		if (mConsoleCommands.Count > 0 && mConsoleCommands[mConsoleCommands.Count - 1] == command)
		{
			return;
		}
		mConsoleCommands.Add(command);
		if (mConsoleCommands.Count > cConsoleCommandLimit)
		{
			mConsoleCommands.RemoveAt(0);
		}
	}

	public void SetFullScreen(bool fs, bool apply)
	{
		mFullScreen = fs;
		if (apply)
		{
			FApp.UpdateVideoMode();
		}
	}

	public void SetConstrainCursor(bool v, bool apply)
	{
		mConstrainCursor = v;
		if (apply)
		{
			FApp.UpdateVideoMode();
		}
	}

	public void SetResolution(int width, int height, int refreshRate, bool fullScreen, bool apply)
	{
		mResolutionWidth = width;
		mResolutionHeight = height;
		mRefreshRate = refreshRate;
		mFullScreen = fullScreen;
		if (apply)
		{
			FApp.UpdateVideoMode();
		}
	}

	public void UpdateResolutionFromCurrent()
	{
		mResolutionWidth = Screen.width;
		mResolutionHeight = Screen.height;
		mRefreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
		mFullScreen = Screen.fullScreen;
	}

	public bool DoesDesiredVideoModeDiffer()
	{
		if (Screen.fullScreen == mFullScreen && Screen.width == mResolutionWidth && Screen.height == mResolutionHeight)
		{
			return Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value) != mRefreshRate;
		}
		return true;
	}

	public void SetEnableAA(bool aa, bool apply)
	{
		mEnableAA = aa;
		if (apply)
		{
			FApp.UpdateQualitySettings();
		}
	}

	public void SetEnableBloom(bool bloom, bool apply)
	{
		mEnableBloom = bloom;
		if (apply)
		{
			FApp.UpdateQualitySettings();
		}
	}

	public void SetShadowQuality(int quality, bool apply)
	{
		mShadowQuality = quality;
		if (apply)
		{
			FApp.UpdateQualitySettings();
		}
	}

	public void SetEnableSSAO(bool ssao, bool apply)
	{
		mEnableSSAO = ssao;
		if (apply)
		{
			FApp.UpdateQualitySettings();
		}
	}

	public static async Task<FPlayerData[]> DebugLoadAllSavedPlayerData(bool includeDefault = true)
	{
		List<FPlayerData> retVal = new List<FPlayerData>();
		List<string> list = new List<string>();
		list.Add("PlayerData.xml");
		if (includeDefault)
		{
			list.Add(DefaultFilename);
		}
		foreach (string item in list)
		{
			FPlayerData data = new FPlayerData();
			data.InstalledDateTime = DateTime.MinValue;
			data.PlayerGuid = Guid.Empty;
			data.PreferKeychain = false;
			data.Filename = item;
			if (await data.PreLoad(false, false) >= 0)
			{
				retVal.Add(data);
			}
		}
		return retVal.ToArray();
	}
}
