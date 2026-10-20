using UnityEngine;
using UnityEngine.UI;

// Rewritten from libil2cpp.so 1.0.605 (HUDFPS @ 0x17e7d70..0x17e8178, Ghidra vaddr).
public class HUDFPS : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private Text mText;

	private float accum2;

	private int frames2;

	private void Start()
	{
		mText = GetComponent<Text>();
		if (mText == null)
		{
			Debug.Log("UtilityFramesPerSecond needs a Text component!");
			base.enabled = false;
		}
		else
		{
			timeleft = updateInterval;
		}
	}

	public void ResetTotal()
	{
		accum2 = 0f;
	}

	public void DebugLog()
	{
		// The shipped body is empty.
	}

	private void OnEnable()
	{
		accum2 = 0f;
	}

	private void OnDisable()
	{
		if (mText != null)
		{
			mText.text = string.Empty;
		}
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		float num = Time.timeScale / Time.deltaTime;
		frames++;
		frames2++;
		accum += num;
		accum2 += num;
		if (!(timeleft > 0f))
		{
			float num2 = accum / (float)frames;
			float num3 = (float)FMemDebug.GetMemUsage() / 1048576f;
			string text = ((!(num3 > 0f)) ? string.Format("{0:F2}", num2) : string.Format("{0:F2} ({1:F1}MB)", num2, num3));
			mText.text = text;
			// the shipped code nests a (10 fps) red branch inside the >= 30 fps case, which can never be reached
			if (num2 >= 30f)
			{
				mText.color = Color.green;
			}
			else
			{
				mText.color = new Color(1f, 0.92156863f, 0.015686275f, 1f);
			}
			accum = 0f;
			timeleft = updateInterval;
		}
	}
}
