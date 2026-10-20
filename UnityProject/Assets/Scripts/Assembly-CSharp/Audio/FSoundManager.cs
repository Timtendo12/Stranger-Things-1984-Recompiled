using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FSoundManager @ 0x17cb98c..0x17cea20, Ghidra vaddr).
public class FSoundManager
{
	public Dictionary<string, FSoundSet> SoundSets = new Dictionary<string, FSoundSet>();

	// 0x17ccc4c: the constructor copies the two adjacent FPlayerData statics (music 0x8, effect 0x4) straight into these fields
	protected float mMusicVolume = FPlayerData.DefaultMusicVolume;

	protected float mEffectVolume = FPlayerData.DefaultEffectVolume;

	protected List<FAudioHelper> mFreeHelpers = new List<FAudioHelper>();

	protected LoopingSoundCategory mMusic = new LoopingSoundCategory();

	protected LoopingSoundCategory mAmbientBed = new LoopingSoundCategory();

	protected Dictionary<string, ExistSoundEntry> mExistSoundEntries = new Dictionary<string, ExistSoundEntry>();

	protected Dictionary<string, PositionalExistSoundEntry> mPositionalExistSoundEntries = new Dictionary<string, PositionalExistSoundEntry>();

	protected GameObject mAudioHelpersParent;

	[CompilerGenerated]
	private Action<float> m_mEffectVolumeChangedAction;

	public float MusicVolume
	{
		get
		{
			return mMusicVolume;
		}
		set
		{
			mMusicVolume = value;
			mMusic.SetVolume(value);
		}
	}

	public float EffectVolume
	{
		get
		{
			return mEffectVolume;
		}
		set
		{
			mEffectVolume = value;
			mAmbientBed.SetVolume(value);
			foreach (KeyValuePair<string, ExistSoundEntry> mExistSoundEntry in mExistSoundEntries)
			{
				mExistSoundEntry.Value.VolumeChange();
			}
			foreach (KeyValuePair<string, PositionalExistSoundEntry> mPositionalExistSoundEntry in mPositionalExistSoundEntries)
			{
				mPositionalExistSoundEntry.Value.VolumeChange();
			}
			if (this.mEffectVolumeChangedAction != null)
			{
				this.mEffectVolumeChangedAction(mEffectVolume);
			}
		}
	}

	protected event Action<float> mEffectVolumeChangedAction;

	public event Action<float> EffectVolumeChangedAction
	{
		add
		{
			mEffectVolumeChangedAction -= value;
			mEffectVolumeChangedAction += value;
		}
		remove
		{
			mEffectVolumeChangedAction -= value;
		}
	}

	public static FSoundManager CreateFromXmlFile(string filename)
	{
		FSoundManager fSoundManager = new FSoundManager();
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename);
		if (xmlElement != null)
		{
			fSoundManager.LoadXml(xmlElement);
		}
		return fSoundManager;
	}

	public bool LoadXml(XmlElement root)
	{
		if (!Application.isPlaying)
		{
			return true;
		}
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				FSoundSet fSoundSet = new FSoundSet();
				fSoundSet.LoadXml(childNode as XmlElement);
				if (!SoundSets.ContainsKey(fSoundSet.Name))
				{
					SoundSets.Add(fSoundSet.Name, fSoundSet);
				}
				else
				{
					FLog.UnityError("Duplicate sound set name: " + fSoundSet.Name);
				}
			}
		}
		if (Application.isPlaying)
		{
			GameObject gameObject = null;
			if (FApp.PrefabPool != null)
			{
				gameObject = FApp.PrefabPool.GetPrefab("AudioHelper");
			}
			if (gameObject == null)
			{
				// PC port: the lazy prefab pool may not hold the helper; load it straight from Resources, else build one.
				gameObject = Resources.Load<GameObject>("prefabs/system/zzzAudioHelper");
			}
			mAudioHelpersParent = new GameObject("zzzzzzAudioHelpers");
			UnityEngine.Object.DontDestroyOnLoad(mAudioHelpersParent);
			for (int i = 0; i < 64; i++)
			{
				GameObject gameObject2;
				if (gameObject != null)
				{
					gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				}
				else
				{
					gameObject2 = new GameObject("AudioHelper", typeof(AudioSource), typeof(FAudioHelper));
					gameObject2.GetComponent<AudioSource>().playOnAwake = false;
				}
				gameObject2.transform.parent = mAudioHelpersParent.transform;
				FAudioHelper component = gameObject2.GetComponent<FAudioHelper>();
				if (component == null)
				{
					FLog.UnityError("Sound helper missing FAudioHelper");
					return false;
				}
				ReleaseHelper(component);
			}
		}
		return true;
	}

	public FSoundSet GetSoundSet(string name)
	{
		if (SoundSets.TryGetValue(name, out var value))
		{
			return value;
		}
		return null;
	}

	public FAudioHelper PlaySoundSet(string soundSetName)
	{
		return PlaySoundSetAtPoint(soundSetName, Vector3.zero, 1f, 1f);
	}

	public FAudioHelper PlaySoundSet(string soundSetName, float volume, float pitch, bool looping = false, bool selfDestruct = true)
	{
		return PlaySoundSetAtPoint(soundSetName, Vector3.zero, volume, pitch, looping, selfDestruct);
	}

	public FAudioHelper PlaySoundSetAtPoint(string soundSetName, Vector3 position, float volume = 1f, float pitch = 1f, bool looping = false, bool selfDestruct = true)
	{
		if (soundSetName != null)
		{
			FSoundSet soundSet = GetSoundSet(soundSetName);
			if (soundSet != null)
			{
				return soundSet.PlayAtPoint(position, volume, pitch, looping, selfDestruct);
			}
		}
		return null;
	}

	public FAudioHelper PlaySoundSetAtGameObject(string soundSetName, GameObject parentGO, float volume = 1f, float pitch = 1f, bool looping = false, bool selfDestruct = true)
	{
		if (soundSetName != null)
		{
			FSoundSet soundSet = GetSoundSet(soundSetName);
			if (soundSet != null)
			{
				return soundSet.PlayAtGameObject(parentGO, volume, pitch, looping, selfDestruct);
			}
		}
		return null;
	}

	public FAudioHelper GetAudioHelper()
	{
		int num = mFreeHelpers.Count - 1;
		if (num < 0)
		{
			return null;
		}
		FAudioHelper fAudioHelper = mFreeHelpers[num];
		if (fAudioHelper.gameObject.activeInHierarchy || fAudioHelper.GetComponent<AudioSource>().clip != null)
		{
			AudioClip clip = fAudioHelper.GetComponent<AudioSource>().clip;
			FLog.UnityError("GetAudioHelper is returning a clip that is already playing: " + ((clip != null) ? clip.name : "") + " This shouldn't be possible!");
		}
		fAudioHelper.gameObject.SetActive(value: true);
		mFreeHelpers.RemoveAt(num);
		return fAudioHelper;
	}

	public FAudioHelper PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, float pitch, bool looping, bool selfDestruct, bool inWorld, float minAttenuateDistance, float maxAttenuateDistance)
	{
		FAudioHelper audioHelper = GetAudioHelper();
		if (audioHelper == null)
		{
			return null;
		}
		audioHelper.PlayAtPoint(clip, position, mEffectVolume * volume, pitch, looping, selfDestruct, inWorld, minAttenuateDistance, maxAttenuateDistance);
		return audioHelper;
	}

	public FAudioHelper PlayClipAtGameObject(AudioClip clip, GameObject parentGO, float volume, float pitch, bool looping, bool selfDestruct, bool inWorld, float minAttenuateDistance, float maxAttenuateDistance)
	{
		FAudioHelper audioHelper = GetAudioHelper();
		if (audioHelper == null)
		{
			return null;
		}
		audioHelper.PlayAtGameObject(clip, parentGO, mEffectVolume * volume, pitch, looping, selfDestruct, inWorld, minAttenuateDistance, maxAttenuateDistance);
		return audioHelper;
	}

	public void ReleaseHelper(FAudioHelper helper)
	{
		if (!(helper == null))
		{
			helper.transform.parent = mAudioHelpersParent.transform;
			helper.OnReleaseObject();
			helper.gameObject.SetActive(value: false);
			mFreeHelpers.Add(helper);
		}
	}

	public void PlayMusic(string soundsetName, float fadeOutTime = 1f, float fadeInTime = 0.25f, bool fadeIfNotCrossFading = false, float fadeInStartDelay = 1f)
	{
		if (FConfig.Music)
		{
			mMusic.Play(soundsetName, mMusicVolume, fadeOutTime, fadeInTime, fadeIfNotCrossFading, fadeInStartDelay);
		}
	}

	public bool FadeOutMusic(float fadeTime)
	{
		return mMusic.FadeOut(fadeTime);
	}

	public bool IsMusicPlaying()
	{
		return mMusic.IsPlaying();
	}

	public void PlayAmbientBed(string soundsetName, float fadeOutTime = 0.25f, float fadeInTime = 0.25f, bool fadeIfNotCrossFading = false, float fadeInStartDelay = 0f)
	{
		if (FConfig.Music)
		{
			mAmbientBed.Play(soundsetName, mEffectVolume, fadeOutTime, fadeInTime, fadeIfNotCrossFading, fadeInStartDelay);
		}
	}

	public bool FadeOutAmbientBed(float fadeTime)
	{
		return mAmbientBed.FadeOut(fadeTime);
	}

	public void PlayExistSound(string soundSetName)
	{
		if (!mExistSoundEntries.TryGetValue(soundSetName, out var value))
		{
			value = new ExistSoundEntry(soundSetName);
			mExistSoundEntries.Add(soundSetName, value);
		}
		value.AddRef();
	}

	public void StopExistSound(string soundSetName)
	{
		mExistSoundEntries.TryGetValue(soundSetName, out var value);
		if (value == null)
		{
			FLog.UnityError("Tried to remove ref on sound set that had none: " + soundSetName);
		}
		else
		{
			value.ReleaseRef();
		}
	}

	public void PlayExistSoundAtPoint(string soundSetName, Vector3 position)
	{
		if (!mPositionalExistSoundEntries.TryGetValue(soundSetName, out var value))
		{
			value = new PositionalExistSoundEntry(soundSetName);
			mPositionalExistSoundEntries.Add(soundSetName, value);
		}
		value.AddRefAtPos(position);
	}

	public void StopExistSoundAtPoint(string soundSetName, Vector3 position)
	{
		mPositionalExistSoundEntries.TryGetValue(soundSetName, out var value);
		if (value == null)
		{
			FLog.UnityError("Tried to remove ref on sound set that had none: " + soundSetName);
		}
		else
		{
			value.ReleaseRefAtPos(position);
		}
	}
}
