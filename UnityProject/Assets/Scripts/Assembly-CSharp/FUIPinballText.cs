using UnityEngine;
// Rewritten from libil2cpp.so 1.0.605 (FUIPinballText @ 0x17d6bf0..0x17d7474, Ghidra vaddr).
public class FUIPinballText : FLabelDropshadow
{
	public enum Modes
	{
		None = 0,
		Pause = 1,
		Up = 2,
		Down = 3
	}

	public float MinimumVelocityUp;

	public float MinimumVelocityDown;

	public bool TruncateHighValue;

	protected float mRealNumber;

	protected int mNumber;

	protected int mGoal;

	protected float mVelocity;

	protected float mTimeTaken;

	protected string mFormatString;

	protected bool mFormatStringDirty;

	public Modes Mode { get; set; }

	public int Number => mNumber;

	public int Goal => mGoal;

	public bool Done => Mode == Modes.None;

	public bool Paused { get; set; }

	public string FormatString
	{
		get
		{
			return mFormatString;
		}
		set
		{
			mFormatString = value;
			mFormatStringDirty = true;
		}
	}

	public FUIPinballText(string name, string fontName, string text, float pointSize, bool shrinkToFitContainer)
		: base(name, fontName, text, pointSize, shrinkToFitContainer)
	{
		MinimumVelocityUp = 10f;
		InitPinball();
	}

	public FUIPinballText(string fontName, string text, float pointSize)
		: base(string.Empty, fontName, text, pointSize)
	{
		MinimumVelocityUp = 10f;
		InitPinball();
	}

	protected void InitPinball()
	{
		mRealNumber = 0f;
		Paused = false;
		mNumber = 0;
		Text = "0";
		mFormatString = "{0:0}";
		mFormatStringDirty = false;
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
		Futile.Instance.AddSignalUpdate(HandleUpdate);
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		if (Mode == Modes.None || Paused)
		{
			return;
		}
		mTimeTaken += Time.deltaTime;
		int mNumber2 = mNumber;
		mRealNumber += mVelocity * Time.deltaTime;
		mNumber = (int)mRealNumber;
		if ((Mode == Modes.Up) ? (mNumber >= mGoal) : (mGoal >= mNumber))
		{
			Mode = Modes.None;
			mVelocity = 0f;
			mNumber = mGoal;
			mRealNumber = mGoal;
		}
		SyncNumber(mNumber2);
	}

	protected void SyncNumber(int oldNumber)
	{
		if (mNumber == oldNumber && !mFormatStringDirty)
		{
			return;
		}
		mFormatStringDirty = false;
		string text;
		if (mNumber <= 1000000 || !TruncateHighValue)
		{
			if (string.IsNullOrEmpty(mFormatString))
			{
				Text = mNumber.ToString();
				return;
			}
			text = FLoc.FormatRaw(mFormatString, mNumber);
		}
		else
		{
			text = string.Format("{0:0.000}", (float)mNumber / 1000000f) + "M";
		}
		Text = text;
	}

	protected void Stop()
	{
		Mode = Modes.None;
		mVelocity = 0f;
		mNumber = mGoal;
		mRealNumber = mGoal;
	}

	public void AddNumber(int number, float length = 1f, bool immediate = false, string soundSet = "")
	{
		SetNumber(mGoal + number, length, immediate, soundSet);
	}

	public void SetNumber(int n, float length = 1f, bool immediate = false, string soundSet = "")
	{
		if (!immediate && !(Mathf.Abs(length) < FMath.Epsilon) && !(mFormatStringDirty && mNumber == n))
		{
			Modes mode = Mode;
			mGoal = n;
			if (mNumber < n)
			{
				Mode = Modes.Up;
			}
			else
			{
				if (mNumber <= n)
				{
					Mode = Modes.None;
					mVelocity = 0f;
					return;
				}
				Mode = Modes.Down;
			}
			if (!string.IsNullOrEmpty(soundSet))
			{
				FApp.SoundManager.PlaySoundSet(soundSet);
			}
			if (Mode == Modes.Up || Mode == Modes.Down)
			{
				if (Mode != mode)
				{
					mTimeTaken = 0f;
				}
				mVelocity = (float)(mGoal - mNumber) / length;
			}
			return;
		}
		mGoal = n;
		int oldNumber = mNumber;
		Mode = Modes.None;
		mVelocity = 0f;
		mNumber = n;
		mRealNumber = n;
		SyncNumber(oldNumber);
	}

	public void Finish()
	{
		Mode = Modes.None;
		mVelocity = 0f;
		mNumber = mGoal;
		mRealNumber = mGoal;
	}
}
