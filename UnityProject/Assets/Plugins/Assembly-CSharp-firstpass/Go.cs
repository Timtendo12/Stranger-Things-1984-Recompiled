using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (Go @ 0x016cc334..0x016d19a0, Ghidra vaddr).
public class Go : MonoBehaviour
{
	public static GoEaseType defaultEaseType;

	public static GoLoopType defaultLoopType;

	public static GoUpdateType defaultUpdateType;

	public static GoDuplicatePropertyRuleType duplicatePropertyRule;

	public static GoLogLevel logLevel = GoLogLevel.Warn;

	public static bool validateTargetObjectsEachTick = true;

	private static List<AbstractGoTween> _tweens = new List<AbstractGoTween>();

	private bool _timeScaleIndependentUpdateIsRunning;

	private static Go _instance;

	public static Go instance
	{
		get
		{
			if (!_instance)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(Go)) as Go;
				if (!_instance)
				{
					GameObject gameObject = new GameObject("GoKit");
					_instance = gameObject.AddComponent<Go>();
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
			}
			return _instance;
		}
	}

	private void handleUpdateOfType(GoUpdateType updateType, float deltaTime)
	{
		for (int i = _tweens.Count - 1; i >= 0; i--)
		{
			if (i >= _tweens.Count)
			{
				continue;
			}
			AbstractGoTween tween = _tweens[i];
			if (tween.updateType == updateType && tween.state == GoTweenState.Running && tween.update(deltaTime * tween.timeScale) && (tween.state == GoTweenState.Destroyed || tween.autoRemoveOnComplete))
			{
				removeTween(tween);
				tween.destroy();
			}
		}
	}

	public void HandleFWorldUpdate(float deltaTime)
	{
		// The shipped code ignores the parameter and uses Time.deltaTime.
		if (_tweens.Count != 0)
		{
			handleUpdateOfType(GoUpdateType.FWorldUpdate, Time.deltaTime);
		}
	}

	private void Update()
	{
		if (_tweens.Count != 0)
		{
			handleUpdateOfType(GoUpdateType.Update, Time.deltaTime);
		}
	}

	private void LateUpdate()
	{
		if (_tweens.Count != 0)
		{
			handleUpdateOfType(GoUpdateType.LateUpdate, Time.deltaTime);
		}
	}

	private void FixedUpdate()
	{
		if (_tweens.Count != 0)
		{
			handleUpdateOfType(GoUpdateType.FixedUpdate, Time.deltaTime);
		}
	}

	private void OnApplicationQuit()
	{
		_instance = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator timeScaleIndependentUpdate()
	{
		_timeScaleIndependentUpdateIsRunning = true;
		float time = Time.realtimeSinceStartup;
		while (_tweens.Count > 0)
		{
			float elapsed = Time.realtimeSinceStartup - time;
			time = Time.realtimeSinceStartup;
			handleUpdateOfType(GoUpdateType.TimeScaleIndependentUpdate, elapsed);
			yield return null;
		}
		_timeScaleIndependentUpdateIsRunning = false;
	}

	private static bool handleDuplicatePropertiesInTween(GoTween tween)
	{
		List<GoTween> list = tweensWithTarget(tween.target);
		List<AbstractTweenProperty> list2 = tween.allTweenProperties();
		foreach (GoTween item in list)
		{
			foreach (AbstractTweenProperty item2 in list2)
			{
				if (item.containsTweenProperty(item2))
				{
					if (duplicatePropertyRule == GoDuplicatePropertyRuleType.DontAddCurrentProperty)
					{
						return true;
					}
					if (duplicatePropertyRule == GoDuplicatePropertyRuleType.RemoveRunningProperty)
					{
						item.removeTweenProperty(item2);
					}
					return false;
				}
			}
		}
		return false;
	}

	[Conditional("UNITY_EDITOR")]
	private static void log(object format, params object[] paramList)
	{
		if (format is string)
		{
			FLog.UnityLog(string.Format(format as string, paramList));
		}
		else
		{
			FLog.UnityLog(format.ToString());
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void warn(object format, params object[] paramList)
	{
		if (logLevel == GoLogLevel.None || logLevel == GoLogLevel.Info)
		{
			return;
		}
		if (format is string)
		{
			FLog.UnityWarning(string.Format(format as string, paramList));
		}
		else
		{
			FLog.UnityWarning(format.ToString());
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void error(object format, params object[] paramList)
	{
		if (logLevel == GoLogLevel.None || logLevel == GoLogLevel.Info || logLevel == GoLogLevel.Warn)
		{
			return;
		}
		if (format is string)
		{
			FLog.UnityError(string.Format(format as string, paramList));
		}
		else
		{
			FLog.UnityError(format.ToString());
		}
	}

	public static GoTween to(object target, float duration, GoTweenConfig config)
	{
		GoTween goTween = new GoTween(target, duration, config);
		if (config.id == -1)
		{
			goTween.setAutoID();
		}
		addTween(goTween);
		return goTween;
	}

	public static GoTween from(object target, float duration, GoTweenConfig config)
	{
		config.isFrom = true;
		GoTween goTween = new GoTween(target, duration, config);
		if (config.id == -1)
		{
			goTween.setAutoID();
		}
		addTween(goTween);
		return goTween;
	}

	public static void addTween(AbstractGoTween tween)
	{
		if (!tween.isValid() || _tweens.Contains(tween))
		{
			return;
		}
		if (duplicatePropertyRule != GoDuplicatePropertyRuleType.None && tween is GoTween)
		{
			if (handleDuplicatePropertiesInTween(tween as GoTween))
			{
				return;
			}
			if (!tween.isValid())
			{
				return;
			}
		}
		_tweens.Add(tween);
		if (!instance.enabled)
		{
			_instance.enabled = true;
		}
		if (tween is GoTween && ((GoTween)tween).isFrom && tween.state != GoTweenState.Paused)
		{
			tween.update(0f);
		}
		if (!_instance._timeScaleIndependentUpdateIsRunning && tween.updateType == GoUpdateType.TimeScaleIndependentUpdate)
		{
			_instance.StartCoroutine(_instance.timeScaleIndependentUpdate());
		}
	}

	public static bool removeTween(AbstractGoTween tween)
	{
		if (_tweens.Contains(tween))
		{
			_tweens.Remove(tween);
			if (_tweens.Count == 0)
			{
				instance.enabled = false;
			}
			return true;
		}
		return false;
	}

	public static List<AbstractGoTween> tweensWithId(int id)
	{
		List<AbstractGoTween> list = null;
		foreach (AbstractGoTween tween in _tweens)
		{
			if (tween.id == id)
			{
				if (list == null)
				{
					list = new List<AbstractGoTween>();
				}
				list.Add(tween);
			}
		}
		return list;
	}

	public static List<GoTween> tweensWithTarget(object target, bool traverseCollections = false)
	{
		List<GoTween> list = new List<GoTween>();
		foreach (AbstractGoTween tween in _tweens)
		{
			if (tween is GoTween goTween)
			{
				if (goTween.target == target)
				{
					list.Add(goTween);
				}
			}
			else if (traverseCollections && tween is AbstractGoTweenCollection abstractGoTweenCollection)
			{
				List<GoTween> list2 = abstractGoTweenCollection.tweensWithTarget(target);
				if (list2.Count > 0)
				{
					list.AddRange(list2);
				}
			}
		}
		return list;
	}

	public static List<AbstractGoTween> abstractTweensWithTarget(object target, bool traverseCollections = false)
	{
		List<AbstractGoTween> list = new List<AbstractGoTween>();
		foreach (AbstractGoTween tween in _tweens)
		{
			if (tween is GoTween goTween)
			{
				if (goTween.target == target)
				{
					list.Add(goTween);
				}
			}
			else if (traverseCollections && tween is AbstractGoTweenCollection abstractGoTweenCollection)
			{
				List<GoTween> list2 = abstractGoTweenCollection.tweensWithTarget(target);
				if (list2.Count > 0)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						if (!list.Contains(list2[i]))
						{
							list.Add(list2[i]);
						}
					}
					list.Add(tween);
				}
			}
		}
		return list;
	}

	public static void killAllTweensWithTarget(object target)
	{
		foreach (GoTween item in tweensWithTarget(target, true))
		{
			item.destroy();
		}
	}

	public static void killAllAbstractTweensWithTarget(object target)
	{
		foreach (AbstractGoTween item in abstractTweensWithTarget(target, true))
		{
			item.destroy();
		}
	}
}
