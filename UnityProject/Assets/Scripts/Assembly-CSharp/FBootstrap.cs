using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Rewritten from libil2cpp.so 1.0.605 (FBootstrap @ 0x16f0558..0x16f1284, Ghidra vaddr).
public class FBootstrap : MonoBehaviour
{
	public enum StartModes
	{
		None = 0,
		Game = 1
	}

	public Canvas LoadingCanvas;

	public Text PercentInitializedText;

	public RectTransform ProgressBar;

	public StartModes StartMode;

	public List<GameObject> PreloadPrefabs = new List<GameObject>();

	protected float mMaxWidth;

	// Map to load directly instead of the main menu. Empty in the shipped build.
	private const string StartMapName = "";

	private void Awake()
	{
		// PC port: hide Unity's on-screen development console (shows up in development builds on any logged error).
		Debug.developerConsoleVisible = false;
		Debug.developerConsoleEnabled = false;
		if (LoadingCanvas != null && FApp.NeedsBootstrap)
		{
			if (LoadingCanvas.gameObject != null)
			{
				LoadingCanvas.gameObject.SetActive(true);
			}
			LoadingCanvas.enabled = true;
		}
		if (PercentInitializedText != null)
		{
			PercentInitializedText.text = string.Empty;
			PercentInitializedText.enabled = true;
		}
		if (ProgressBar != null)
		{
			mMaxWidth = ProgressBar.sizeDelta.x;
			SetProgressBarFill(0f);
		}
		StartCoroutine(UpdateLoadingScreen());
	}

	protected void SetProgressBarFill(float fill)
	{
		if (ProgressBar == null)
		{
			return;
		}
		Vector2 size = ProgressBar.sizeDelta;
		ProgressBar.sizeDelta = new Vector2(mMaxWidth * fill, size.y);
		ProgressBar.localPosition = new Vector3((ProgressBar.sizeDelta.x - mMaxWidth) * 0.5f, 0f, 0f);
	}

	private IEnumerator UpdateLoadingScreen()
	{
		float previousInitPercentage = -1f;
		while (true)
		{
			float percentage = FApp.InitPercentage;
			if (previousInitPercentage != percentage)
			{
				previousInitPercentage = percentage;
				if (PercentInitializedText != null)
				{
					PercentInitializedText.text = string.Format("{0}%", (int)percentage);
				}
				if (ProgressBar != null)
				{
					SetProgressBarFill(percentage / 100f);
				}
			}
			yield return null;
		}
	}

	private void Start()
	{
		if (FApp.NeedsBootstrap)
		{
			FApp.NeedsBootstrap = false;
			StartCoroutine(Bootstrap());
		}
		else
		{
			Destroy(this);
		}
	}

	private IEnumerator Bootstrap()
	{
		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(FApp.Init());

		// Wait for the HUD to exist and finish initializing.
		while (FApp.HUD == null || !FApp.HUD.Initialized)
		{
			yield return null;
		}

		if (!string.IsNullOrEmpty(StartMapName))
		{
			bool loadComplete = false;
			FApp.HUD.LoadLevel(StartMapName, () => loadComplete = true, () => loadComplete = true, false);
			while (!loadComplete)
			{
				yield return null;
			}
		}
		else
		{
			FLog.UnityLog("FBootstrap::Bootstrap::PushMainMenu");
			FApp.HUD.PushPage(FApp.HUD.PageLegendMainMenu, true, false);
		}

		FApp.BootstrapComplete();
		gameObject.SetActive(false);
		Destroy(this);
	}
}
